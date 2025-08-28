using Payroll.Common.ApplicationModel;

namespace Payroll.WebApp.Models.DTOs
{
    public class MapEntityGradeMasterDTO : BaseModel
    {
        public List<MapEntityGradeDTO> MapEntityGrade { get; set; } = new();
    }
    public class MapEntityGradeDTO
    {
        public int Entity_ID { get; set; }
        public int Pay_Grade_ID { get; set; }
    }
}
