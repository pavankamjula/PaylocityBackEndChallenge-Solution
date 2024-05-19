using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTests
{

    // Helper DTOs created for unit test for cost calculations
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
    }

    public class EmployeePaycheckDto
    {
        public int EmployeeId { get; set; }
        public string FullName { get; set; }
        public decimal YearlySalary { get; set; }
        public decimal PaycheckAmount { get; set; }
        public decimal TotalDeductions { get; set; }
        public List<DependentDeductionDto> DependentDeductions { get; set; } = new List<DependentDeductionDto>();
    }

    public class DependentDeductionDto
    {
        public string DependentName { get; set; }
        public decimal Deduction { get; set; }
    }
    Te
}
