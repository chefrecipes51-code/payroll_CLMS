using Payroll.Common.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollTransactionService.BAL.ReportModel
{
    public class PayrollGlobalParamRequest : BaseModel
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
    public class PayrollSettingsWrapper
    {
        public PayrollGlobalParamRequest GlobalParams { get; set; }
        public PayrollSettingRequest Settings { get; set; }
        public PayrollComplianceRequest Compliances { get; set; }
        public ThirdPartyParameterRequest ThirdPartyParams { get; set; }

    }
}
