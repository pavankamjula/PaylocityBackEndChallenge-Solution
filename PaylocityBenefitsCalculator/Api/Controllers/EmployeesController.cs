using Api.DBContext;
using Api.Dtos.Dependent;
using Api.Dtos.Employee;
using Api.Dtos.EmployeePaycheck;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.Entity;

namespace Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class EmployeesController : ControllerBase
{

    // We are using DB Context with InMemory database due to absense of real database
    // StaticDbContext can be replaced with real DBContext class

    private readonly StaticDbContext _dbContext;

    public EmployeesController(StaticDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<GetEmployeeDto>>> Get(int id)
    {
        var employee = _dbContext.Employees
            .Where(e => e.Id == id)
            .Select(e => new GetEmployeeDto
            {
                Id = e.Id,
                FirstName = e.FirstName,
                LastName = e.LastName,
                Salary = e.Salary,
                DateOfBirth = e.DateOfBirth,
                Dependents = e.Dependents.Select(d => new GetDependentDto
                {
                    Id = d.Id,
                    FirstName = d.FirstName,
                    LastName = d.LastName,
                    DateOfBirth = d.DateOfBirth,
                    Relationship = d.Relationship,
                    Age = CalculateAge(d.DateOfBirth)
                }).ToList()
            })
            .AsEnumerable()
            .Select(e => new GetEmployeeDto
             {
                 Id = e.Id,
                 FirstName = e.FirstName,
                 LastName = e.LastName,
                 Salary = e.Salary,
                 DateOfBirth = e.DateOfBirth,
                 Dependents = e.Dependents
             })
             .FirstOrDefault();

        if (employee == null)
            return NotFound();

        return Ok(new ApiResponse<GetEmployeeDto> { Data = employee, Success = true });
    }

    [HttpGet("")]
    public async Task<ActionResult<ApiResponse<List<GetEmployeeDto>>>> GetAll()
    {
        var employees = _dbContext.Employees
         .Select(e => new
         {
             e.Id,
             e.FirstName,
             e.LastName,
             e.Salary,
             e.DateOfBirth,
             Dependents = e.Dependents.Select(d => new GetDependentDto
             {
                 Id = d.Id,
                 FirstName = d.FirstName,
                 LastName = d.LastName,
                 DateOfBirth = d.DateOfBirth,
                 Relationship = d.Relationship,
                 Age = CalculateAge(d.DateOfBirth)
             }).ToList()
         })
         .AsEnumerable()
         .Select(e => new GetEmployeeDto
         {
             Id = e.Id,
             FirstName = e.FirstName,
             LastName = e.LastName,
             Salary = e.Salary,
             DateOfBirth = e.DateOfBirth,
             Dependents = e.Dependents
         }).ToList();

        return Ok(new ApiResponse<List<GetEmployeeDto>> { Data = employees, Success = true });
    }



    // This method is created to calculate Employee Paycheck calcualations
    // This method is also Unit tested using Seeded data in APITests Project in this solution

    [HttpGet("{id}/paycheck")]
    public async Task<ActionResult<ApiResponse<EmployeePaycheckDto>>> GetEmployeePaycheck(int id)
    {
        var employee =  _dbContext.Employees
            .Include(e => e.Dependents)
            .FirstOrDefault(e => e.Id == id);

        if (employee == null)
            return NotFound();

        var paycheck = CalculateEmployeePaycheck(employee);

        return Ok(new ApiResponse<EmployeePaycheckDto> { Data = paycheck, Success = true });
    }

    private EmployeePaycheckDto CalculateEmployeePaycheck(Employee employee)
    {
        var yearlySalary = employee.Salary;
        var baseCost = 1000 * 12; // Base cost for benefits ($1,000 per month)
        var dependentCost = employee.Dependents.Count * 600 * 12; // Cost for dependents ($600 per month per dependent)
        var additionalCost = yearlySalary > 80000 ? yearlySalary * 0.02m : 0; // Additional cost for employees earning more than $80,000 per year
        var dependentOverAgeCost = employee.Dependents.Count(d => CalculateAge(d.DateOfBirth) > 50) * 200 * 12; // Cost for dependents over 50 years old ($200 per month per dependent)

        var totalDeductions = baseCost + dependentCost + additionalCost + dependentOverAgeCost;
        var paycheck = (yearlySalary - totalDeductions) / 26; // Paycheck amount after deductions

        var employeePaycheckDto = new EmployeePaycheckDto
        {
            EmployeeId = employee.Id,
            FullName = $"{employee.FirstName} {employee.LastName}",
            YearlySalary = yearlySalary,
            PaycheckAmount = paycheck,
            TotalDeductions = totalDeductions,
            DependentDeductions = employee.Dependents.Select(d => new DependentDeductionDto
            {
                DependentName = $"{d.FirstName} {d.LastName}",
                Deduction = d.Relationship == Relationship.Spouse || d.Relationship == Relationship.DomesticPartner ? 600 * 12 : 600 * 12 + (CalculateAge(d.DateOfBirth) > 50 ? 200 * 12 : 0)
            }).ToList()
        };

        return employeePaycheckDto;
    }

    private int CalculateAge(DateTime dateOfBirth)
    {
        var today = DateTime.Today;
        var age = today.Year - dateOfBirth.Year;

        if (dateOfBirth.Date > today.AddYears(-age))
            age--;

        return age;
    }
}
