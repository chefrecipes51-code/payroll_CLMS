using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollTransactionService.BAL.Models
{
    public class ContractorMaster
    {
        public int Contractor_ID { get; set; }
        public string Contractor_Code { get; set; }
        public string Contractor_Name { get; set; }
        public string totalcl { get; set; }
        public bool IsActive { get; set; }
    }
}
