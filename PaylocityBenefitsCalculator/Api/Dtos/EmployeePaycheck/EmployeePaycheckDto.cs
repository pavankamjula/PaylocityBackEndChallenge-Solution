namespace Api.Dtos.EmployeePaycheck
{
    public class EmployeePaycheckDto
    {
        public int EmployeeId { get; set; }
        public string FullName { get; set; }
        public decimal YearlySalary { get; set; }
        public decimal PaycheckAmount { get; set; }
        public decimal TotalDeductions { get; set; }
        public List<DependentDeductionDto> DependentDeductions { get; set; }
    }

    public class DependentDeductionDto
    {
        public string DependentName { get; set; }
        public decimal Deduction { get; set; }
    }
}
