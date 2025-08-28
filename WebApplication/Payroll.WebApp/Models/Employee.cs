namespace Payroll.WebApp.Models
{
    #region Employee Properties
    public class Employee
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Position { get; set; }
        public decimal Salary { get; set; }
    }
    #endregion
    #region Employee ViewModel Properties
    public class EmployeeViewModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Position { get; set; }
        public decimal Salary { get; set; }
        public string? EncryptedUrl { get; set; }
    }
    #endregion
}
