using Payroll.Common.ApplicationModel;

namespace Payroll.WebApp.Models.DTOs
{
    public class PayrollTransDataForProcessDTO : BaseModel
    {
        public int Company_ID { get; set; }
        public int Month_ID { get; set; }
        public int Year_ID { get; set; }
        public string LocationIDs { get; set; }
        public string ContractorIDs { get; set; }
        public string WorkOrderIDs { get; set; }
        public int Updated_By { get; set; }
        public int UpdatedRecords {  get; set; }
    }
}
