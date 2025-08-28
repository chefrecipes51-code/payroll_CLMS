using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollTransactionService.BAL.Models
{
    public class WorkOrderMaster
    {
        public string WorkOrder_Code { get; set; }
        public string WorkOrder_No { get; set; }
        public string Contractor_Code { get; set; }
    }
    public class ContractorWorkOrderRequest
    {
        public int WorkOrder_Code { get; set; }
        public string ContractorWiseWorkOrder { get; set; }
    }

}
