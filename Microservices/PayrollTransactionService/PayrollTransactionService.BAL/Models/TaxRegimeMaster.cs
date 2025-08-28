using Payroll.Common.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollTransactionService.BAL.Models
{
    public class TaxRegimeMaster : BaseModel
    {
        public int YearlyItTable_Id { get; set; }
        public int Company_Id { get; set; }
        public DateTime EffectiveDate { get; set; }
        public string Remarks { get; set; }
        public string Regimename { get; set; }
        public int Currency_Id { get; set; }
    }
}
