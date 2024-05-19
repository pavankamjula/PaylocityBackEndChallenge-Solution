using Api.Controllers;
using Api.DBContext;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ApiTests
{

    // This controller test class is to test and validate the Cost Calculation logic
    // We have seeded data first in the main controller and then written a test case to validate the logic
    public class EmployeesControllerTests
    {
        private readonly StaticDbContext _context;
        private readonly EmployeesController _controller;

        public EmployeesControllerTests()
        {
            var options = new DbContextOptionsBuilder<StaticDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new StaticDbContext(options);
            _controller = new EmployeesController(_context);

            // Seed data
            var employee = new Employee
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Salary = 85000,
                DateOfBirth = new DateTime(1980, 1, 1),
                Dependents = new List<Dependent>
            {
                new Dependent
                {
                    FirstName = "Jane",
                    LastName = "Doe",
                    DateOfBirth = new DateTime(1982, 2, 2),
                    Relationship = Relationship.Spouse
                },
                new Dependent
                {
                    FirstName = "Jimmy",
                    LastName = "Doe",
                    DateOfBirth = new DateTime(1950, 3, 3),
                    Relationship = Relationship.Child
                }
            }
            };

            _context.Employees.Add(employee);
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetEmployeePaycheck_ShouldReturnCorrectPaycheck()
        {
            var result = await _controller.GetEmployeePaycheck(1);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var apiResponse = Assert.IsType<ApiResponse<EmployeePaycheckDto>>(okResult.Value);

            Assert.True(apiResponse.Success);
            Assert.Equal(1, apiResponse.Data.EmployeeId);
            Assert.Equal("John Doe", apiResponse.Data.FullName);
            Assert.Equal(85000, apiResponse.Data.YearlySalary);

            // Calculate expected values
            var baseCost = 1000 * 12;
            var dependentCost = 600 * 12 * 2;
            var additionalCost = 85000 * 0.02m;
            var dependentOverAgeCost = 200 * 12;
            var totalDeductions = baseCost + dependentCost + additionalCost + dependentOverAgeCost;
            var expectedPaycheck = (85000 - totalDeductions) / 26;

            Assert.Equal(totalDeductions, apiResponse.Data.TotalDeductions);
            Assert.Equal(expectedPaycheck, apiResponse.Data.PaycheckAmount);
            Assert.Equal(2, apiResponse.Data.DependentDeductions.Count);

            var firstDependentDeduction = apiResponse.Data.DependentDeductions.First(d => d.DependentName == "Jane Doe");
            Assert.Equal(600 * 12, firstDependentDeduction.Deduction);

            var secondDependentDeduction = apiResponse.Data.DependentDeductions.First(d => d.DependentName == "Jimmy Doe");
            Assert.Equal(600 * 12 + 200 * 12, secondDependentDeduction.Deduction);
        }
    }

}
