using Payroll.Common.ApplicationModel;

namespace Payroll.WebApp.Models.DTOs
{
    public class BreadCrumbDTO : BaseModel
    {
        public int Breadcrumb_Id { get; set; }
        public int MenuGroup_Id { get; set; }
        public string Title { get; set; }
        public string Action_URL { get; set; }
        public int DisplayOrder { get; set; }

    }
}
