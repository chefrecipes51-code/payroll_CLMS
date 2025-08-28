using Payroll.Common.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollTransactionService.BAL.ReportModel
{
    public class CopySettingsRequest : BaseModel
    {
        public string? SelectParam { get; set; } // Comma-separated string like "1,2,3"
        public int CopyFromCompanyID { get; set; }
        public int CopyToCompanyID { get; set; }
    }
}
