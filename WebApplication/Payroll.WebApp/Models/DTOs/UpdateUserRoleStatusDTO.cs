using Payroll.Common.ApplicationModel;

namespace Payroll.WebApp.Models.DTOs
{
    public class UpdateUserRoleStatusDTO : BaseModel
    {
        public int User_Id { get; set; }
        public int Role_User_Id { get; set; }
        public bool ActualActivestatus { get; set; }
    }
}
