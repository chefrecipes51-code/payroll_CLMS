using Payroll.Common.ApplicationModel;

namespace Payroll.WebApp.Models.DTOs
{
    public class PayrollGlobalParamDTO : BaseModel
    {
        public int Global_Param_ID { get; set; }
        public int Company_ID { get; set; }
        public int Salary_Frequency { get; set; }
        public int MonthlySalary_Based_On { get; set; }
        public decimal Round_Off_Components { get; set; }
        public int EffectivePayroll_start_Mnth { get; set; }
        public bool Allow_Adhoc_Components { get; set; }
        public bool LOckSalary_Post_Payroll { get; set; }
        public int? CopyFromCompanyId { get; set; }
    }
}
