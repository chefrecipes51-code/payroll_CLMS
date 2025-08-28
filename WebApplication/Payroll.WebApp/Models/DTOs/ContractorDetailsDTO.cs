using Payroll.Common.ApplicationModel;

namespace Payroll.WebApp.Models.DTOs
{
    public class ContractorDetailsDTO : BaseModel
    {
        public int Contractor_ID { get; set; }
        public int Company_Id { get; set; }
        public int Contractor_Code { get; set; }
        public int Correspondance_ID { get; set; }
        public string Contractor_Name { get; set; }
        public int totalcl { get; set; }
    }


}
