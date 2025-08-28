using Payroll.Common.ApplicationModel;

namespace Payroll.WebApp.Models.DTOs
{
    public class MapEntityTaxRegimeDTO : BaseModel
    {
        public List<EntityTaxRegimeDTO> EntityTaxRegime { get; set; } = new();
        // public long Map_TaxRegime_ID { get; set; }
    }
    public class EntityTaxRegimeDTO
    {
        public int Contractor_Id { get; set; }
        public string Contractor_Code { get; set; }
        public int Entity_ID { get; set; }
        public string Entity_Code { get; set; } = string.Empty;
        public int Regime_Id { get; set; }
        public int FinYear_ID { get; set; }
        public bool IsActive { get; set; }
    }
}
