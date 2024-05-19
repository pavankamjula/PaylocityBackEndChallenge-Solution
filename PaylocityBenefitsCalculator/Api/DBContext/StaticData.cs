using Api.Models;

namespace Api.DBContext
{
    // This is Static data to support ImMemory data store for DB Context
    public static class StaticData
    {
        public static List<Employee> Employees = new List<Employee>
        {
            new Employee
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Salary = 90000,
                DateOfBirth = new DateTime(1980, 5, 15),
                Dependents = new List<Dependent>
                {
                    new Dependent
                    {
                        Id = 1,
                        FirstName = "Jane",
                        LastName = "Doe",
                        DateOfBirth = new DateTime(1982, 3, 20),
                        Relationship = Relationship.Spouse
                    },
                    new Dependent
                    {
                        Id = 2,
                        FirstName = "Michael",
                        LastName = "Doe",
                        DateOfBirth = new DateTime(2010, 10, 5),
                        Relationship = Relationship.Child
                    },
                    new Dependent
                    {
                        Id = 3,
                        FirstName = "Emily",
                        LastName = "Doe",
                        DateOfBirth = new DateTime(2015, 7, 18),
                        Relationship = Relationship.Child
                    }
                }
            },
            new Employee
            {
                Id = 2,
                FirstName = "Bob",
                LastName = "Smith",
                Salary = 60000,
                DateOfBirth = new DateTime(1985, 9, 25),
                Dependents = new List<Dependent>
                {
                    new Dependent
                    {
                        Id = 4,
                        FirstName = "Alice",
                        LastName = "Smith",
                        DateOfBirth = new DateTime(1960, 2, 10),
                        Relationship = Relationship.DomesticPartner
                    },
                    new Dependent
                    {
                        Id = 5,
                        FirstName = "David",
                        LastName = "Smith",
                        DateOfBirth = new DateTime(2005, 12, 1),
                        Relationship = Relationship.Child
                    }
                }
            }
        };
    }
}
