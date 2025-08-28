using Payroll.Common.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollTransactionService.BAL.ReportModel
{
    public class PayCalculationUpdateRequest : BaseModel
    {
        public int CompanyId { get; set; }
        public List<PayCalculationUpdateItem> EntityStructureUpdateList { get; set; }       
    }
    public class PayCalculationUpdateItem
    {
        public int Entity_ID { get; set; }
        public int? SalaryStructureId { get; set; }
    }
}
