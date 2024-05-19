using Api.DBContext;
using Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.EnableAnnotations();
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "Employee Benefit Cost Calculation Api",
                Description = "Api to support employee benefit cost calculations"
            });
        });

        var allowLocalhost = "allow localhost";
        builder.Services.AddCors(options =>
        {
            options.AddPolicy(allowLocalhost,
                policy => { policy.WithOrigins("http://localhost:3000", "http://localhost"); });
        });

        builder.Services.AddDbContext<StaticDbContext>(options =>
            options.UseInMemoryDatabase("StaticData"));

        var app = builder.Build();

        //Seeding Context data
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<StaticDbContext>();

            // Seed the database
            context.Employees.AddRange(
             new Employee { Id = 1, FirstName = "John", LastName = "Doe", Salary = 90000, DateOfBirth = new DateTime(1980, 5, 15) },
                new Employee { Id = 2, FirstName = "Bob", LastName = "Smith", Salary = 60000, DateOfBirth = new DateTime(1985, 9, 25) }
            );

            context.Dependents.AddRange(
            new Dependent { Id = 1, FirstName = "Jane", LastName = "Doe", DateOfBirth = new DateTime(1982, 3, 20), Relationship = Relationship.Spouse, EmployeeId = 1 },
                 new Dependent { Id = 2, FirstName = "Michael", LastName = "Doe", DateOfBirth = new DateTime(2010, 10, 5), Relationship = Relationship.Child, EmployeeId = 1 },
                 new Dependent { Id = 3, FirstName = "Emily", LastName = "Doe", DateOfBirth = new DateTime(2015, 7, 18), Relationship = Relationship.Child, EmployeeId = 1 },
                 new Dependent { Id = 4, FirstName = "Alice", LastName = "Smith", DateOfBirth = new DateTime(1960, 2, 10), Relationship = Relationship.DomesticPartner, EmployeeId = 2 },
                 new Dependent { Id = 5, FirstName = "David", LastName = "Smith", DateOfBirth = new DateTime(2005, 12, 1), Relationship = Relationship.Child, EmployeeId = 2 }
            );

            context.SaveChanges();
        }


        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseCors(allowLocalhost);

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}