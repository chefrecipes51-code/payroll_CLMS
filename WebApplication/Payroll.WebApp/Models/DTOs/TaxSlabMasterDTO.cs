using Payroll.Common.ApplicationModel;

namespace Payroll.WebApp.Models.DTOs
{
    public class TaxSlabMasterDTO : BaseModel
    {
        public int YearlyItTableDetail_Id { get; set; }
        public int YearlyItTable_Id { get; set; }
        public int Company_Id { get; set; }
        public string SlabName { get; set; }
        public decimal Income_From { get; set; }
        public decimal Income_To { get; set; }
        public int TaxPaybleInPercentage { get; set; }
        public decimal TaxPaybleInAmount { get; set; }
    }
    public class TaxRegimeMasterDTO : BaseModel
    {
        public int YearlyItTable_Id { get; set; }
        public int Company_Id { get; set; }
        public DateTime EffectiveDate { get; set; }
        public string Remarks { get; set; }
        public string Regimename { get; set; }
        public int Currency_Id { get; set; }
    }
}
