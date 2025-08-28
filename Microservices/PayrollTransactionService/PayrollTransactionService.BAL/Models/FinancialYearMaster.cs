using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollTransactionService.BAL.Models
{
    public class FinancialYearMaster
    {
        public int FinancialYearID { get; set; }
        public int Company_Id { get; set; }
        public string FinYear { get; set; }

    }
}
