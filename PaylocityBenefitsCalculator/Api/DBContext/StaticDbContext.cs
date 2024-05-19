using Api.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Api.DBContext
{
    // This is DB context which currently runs on inMemory data store
    // This is extensible and can be replaced with any DB store later
    // To demonstrate the EF real case we have used inMemory data store for now
    public class StaticDbContext : DbContext
    {
        public StaticDbContext(DbContextOptions<StaticDbContext> options) : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Dependent> Dependents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Employee>()
                .HasMany(e => e.Dependents)
                .WithOne(d => d.Employee)
                .HasForeignKey(d => d.EmployeeId);
        }
    }

}
