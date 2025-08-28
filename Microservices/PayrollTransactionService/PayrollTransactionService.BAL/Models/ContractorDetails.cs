using Payroll.Common.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollTransactionService.BAL.Models
{
    public class ContractorDetails : BaseModel
    {
        public int Contractor_ID { get; set; }
        public int Company_Id { get; set; }
        public int Contractor_Code { get; set; }
        public int Correspondance_ID { get; set; }
        public string Contractor_Name { get; set; }
        public int totalcl { get; set; }

    }
}
