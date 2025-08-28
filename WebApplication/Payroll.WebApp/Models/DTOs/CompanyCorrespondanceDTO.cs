using Payroll.Common.ApplicationModel;

namespace Payroll.WebApp.Models.DTOs
{
    public class CompanyCorrespondanceDTO : BaseModel
    {
        public byte Correspondance_ID { get; set; }
        public byte Company_Id { get; set; }
        public string CompanyAddress { get; set; }
        public string Building_No { get; set; }
        public string Building_Name { get; set; }
        public string Street { get; set; }
        public int Country_ID { get; set; }
        public int State_Id { get; set; }
        public int City_ID { get; set; }
        public int Location_ID { get; set; }
        public int CorrespondanceType { get; set; }
        public int PinCode { get; set; }
        public string Primary_Phone_no { get; set; }
        public string Secondary_Phone_No { get; set; }
        public string Primary_Email_Id { get; set; }
        public string Secondary_Email_ID { get; set; }
        public string WebsiteUrl { get; set; }
        public string Company_LogoImage_Path { get; set; }
    }
}
