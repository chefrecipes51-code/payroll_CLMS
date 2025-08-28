namespace Payroll.Common.ApplicationModel.FilterModel
{
    public class CommonFilterModel
    {
        public string? Search { get; set; } = null;
        public int? UserId { get; set; }
        public bool? IsActive { get; set; }
    }
}
