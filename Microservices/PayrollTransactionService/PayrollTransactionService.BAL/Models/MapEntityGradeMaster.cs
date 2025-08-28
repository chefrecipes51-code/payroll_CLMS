using Payroll.Common.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollTransactionService.BAL.Models
{
    public class MapEntityGradeMaster : BaseModel
    {
        public List<MapEntityGrade> MapEntityGrade { get; set; } = new();
        // public long Map_TaxRegime_ID { get; set; }
    }
    public class MapEntityGrade
    {
        public int Entity_ID { get; set; }
        public int Pay_Grade_ID { get; set; }
    }
}
