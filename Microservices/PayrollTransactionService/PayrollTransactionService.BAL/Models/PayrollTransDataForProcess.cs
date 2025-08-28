using Payroll.Common.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollTransactionService.BAL.Models
{
    public class PayrollTransDataForProcess : BaseModel
    {
        public int Company_ID { get; set; }
        public int Month_ID { get; set; }
        public int Year_ID { get; set; }
        public string LocationIDs { get; set; }
        public string ContractorIDs { get; set; }
        public string WorkOrderIDs { get; set; }
        public int Updated_By { get; set; }
        public int UpdatedRecords { get; set; }
    }
}
