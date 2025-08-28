using Payroll.Common.ApplicationModel;

namespace Payroll.WebApp.Models
{
    public class RoleMenuMappingHeaderDTO : BaseModel
    {
        public int Role_Menu_Hdr_Id { get; set; }
        public int Company_Id { get; set; }
        public int Role_Id { get; set; }
        public DateTime EffectiveFrom { get; set; }

    }
    public class RoleMenuDetailDTO : BaseModel
    {
        public int Role_Menu_Dtl_Id { get; set; }
        public int Role_Menu_Hdr_Id { get; set; }          // Detail ID
        public int Menu_Id { get; set; }                 // Menu ID
        public int Company_Id { get; set; }              // Company ID
    }
    public class RoleMenuMappingRequestDTO
    {
        public RoleMenuMappingHeaderDTO Header { get; set; }                // Header data
        public List<RoleMenuDetailDTO> Details { get; set; } = new();       // List of detail data
        public int MessageType { get; set; }                                // Message type
        public int MessageMode { get; set; }                                // Message mode
        public int ModuleId { get; set; }                                   // Module ID
    }
}
