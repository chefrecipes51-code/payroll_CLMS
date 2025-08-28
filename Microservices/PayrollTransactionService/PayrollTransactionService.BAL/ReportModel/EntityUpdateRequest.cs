using Payroll.Common.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollTransactionService.BAL.ReportModel
{
    public class EntityUpdateRequest : BaseModel
    {
        public int CompanyId { get; set; }
        public List<EntityUpdateItem> EntityUpdateList { get; set; }
    }
    public class EntityUpdateItem
    {
        public int Entity_ID { get; set; }      
        public int? GradeConfigName { get; set; } // Now nullable
    }

}
