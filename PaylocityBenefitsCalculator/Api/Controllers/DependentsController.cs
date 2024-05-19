using Api.DBContext;
using Api.Dtos.Dependent;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class DependentsController : ControllerBase
{
    private readonly StaticDbContext _dbContext;

    public DependentsController(StaticDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [SwaggerOperation(Summary = "Get dependent by id")]
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<GetDependentDto>>> Get(int id)
    {
        var dependent = _dbContext.Dependents
           .Select(d => new GetDependentDto
           {
               Id = d.Id,
               FirstName = d.FirstName,
               LastName = d.LastName,
               DateOfBirth = d.DateOfBirth,
               Relationship = d.Relationship,
               Age = CalculateAge(d.DateOfBirth)
           })
           .FirstOrDefault(d => d.Id == id);

        if (dependent == null)
            return NotFound();

        return Ok(new ApiResponse<GetDependentDto> { Data = dependent, Success = true });

    }

    [SwaggerOperation(Summary = "Get all dependents")]
    [HttpGet("")]
    public async Task<ActionResult<ApiResponse<List<GetDependentDto>>>> GetAll()
    {
        var dependents = _dbContext.Dependents
            .Select(d => new GetDependentDto
            {
                Id = d.Id,
                FirstName = d.FirstName,
                LastName = d.LastName,
                DateOfBirth = d.DateOfBirth,
                Relationship = d.Relationship,
                Age = CalculateAge(d.DateOfBirth)
            })
            .ToList();

        return Ok(new ApiResponse<List<GetDependentDto>> { Data = dependents, Success = true });

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
