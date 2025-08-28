using Payroll.Common.ApplicationModel;

namespace Payroll.WebApp.Models.DTOs
{
    public class AreaDTO : BaseModel
    {
        public int Area_Id { get; set; } // Primary key, auto-incremented
        public int Location_Id { get; set; } 
        public int cityid { get; set; } 
        public int State_Id { get; set; } 
        public int CountryId { get; set; } 
        public string AreaName { get; set; }
        public string LocationName { get; set; }

        public string OffCanvasId { get; set; }
        public string FormTitle { get; set; }
        public bool IsEditMode { get; set; }
    }
}
