using Payroll.Common.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollTransactionService.BAL.ReportModel
{
    public class PayComponentActivationRequest : BaseModel
    {
        public int EarningDeduction_Id { get; set; }
        public int Company_Id { get; set; }
    }
}
