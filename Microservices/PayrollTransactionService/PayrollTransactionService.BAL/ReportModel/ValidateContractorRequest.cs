using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Payroll.Common.ApplicationModel;

namespace PayrollTransactionService.BAL.ReportModel
{
    public class ValidateContractorRequest :BaseModel
    {
        public int CompanyId { get; set; }
        public List<int> ContractorIds { get; set; } = new();
    }
}
