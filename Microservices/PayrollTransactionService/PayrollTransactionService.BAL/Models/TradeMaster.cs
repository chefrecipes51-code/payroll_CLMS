using Payroll.Common.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollTransactionService.BAL.Models
{
    public class TradeMaster : BaseModel
    {
        public int Trade_mst_Id { get; set; }
        public char ExternaluniqueID { get; set; }
        public string Trade_Name { get; set; }
        public int Company_ID { get; set; }
        public int Company_Location_ID { get; set; }
    }
}
