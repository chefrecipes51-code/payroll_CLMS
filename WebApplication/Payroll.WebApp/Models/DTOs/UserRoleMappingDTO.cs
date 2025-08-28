using Payroll.Common.ApplicationModel;

namespace Payroll.WebApp.Models.DTOs
{
    public class UserRoleMappingDTO : BaseModel 
    {
        public int Role_User_Id { get; set; }
        public List<int> Role_Ids { get; set; }
        public int Role_Menu_Header_Id { get; set; }
        public int Company_Id { get; set; }
        public int User_Id { get; set; }
        public DateTime Effective_From { get; set; }
    }

}
