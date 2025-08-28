using Payroll.Common.ApplicationModel;

namespace Payroll.WebApp.Models.DTOs
{
    public class MapDepartmentLocationDTO : BaseModel
    {
        public int Department_Location_Id { get; set; }
        public byte Company_Id { get; set; }
        public int Correspondance_ID { get; set; }
        public byte Department_Id { get; set; }
        public string Department_Code { get; set; }
        public int Area_Id { get; set; }
        public int Location_Id { get; set; }
        public int City_ID { get; set; }
        public int State_Id { get; set; }
        public int Country_ID { get; set; }
        public byte Floor_Id { get; set; }
        public string LocationName { get; set; }
        public string AreaName { get; set; }
        public string DepartmentName { get; set; }
    }
}
