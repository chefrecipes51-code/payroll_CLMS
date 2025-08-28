using Payroll.Common.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace PayrollTransactionService.BAL.Models
{
    public class MapEntityTaxRegime : BaseModel
    {
        public List<EntityTaxRegime> EntityTaxRegime { get; set; } = new();
        // public long Map_TaxRegime_ID { get; set; }
    }
    public class EntityTaxRegime
    {
        public int Contractor_Id { get; set; }
        public char Contractor_Code { get; set; }
        public int Entity_ID { get; set; }
        public string Entity_Code { get; set; } = string.Empty;
        public int Regime_Id { get; set; }
        public int FinYear_ID { get; set; }
        public bool IsActive { get; set; }
    }
}
