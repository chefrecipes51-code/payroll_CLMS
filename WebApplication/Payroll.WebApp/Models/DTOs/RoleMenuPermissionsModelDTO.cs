namespace Payroll.WebApp.Models.DTOs
{
    public class RoleMenuPermissionsModelDTO
    {
        public int RoleId { get; set; }
        public int CompanyId { get; set; }
        public int CorrespondanceId { get; set; }
        public int UserId { get; set; } //Added By Harshida
        public DateTime EffectiveFromDt { get; set; } // Added By Harshida
        public List<PermissionDataModelDTO> PermissionsData { get; set; }
    }

    public class PermissionDataModelDTO
    {
        public int MenuId { get; set; }
        public int? ParentMenuId { get; set; } // Nullable since parentMenuId might be null
        public int RoleMenuHdrId { get; set; }
        public int RoleMenuDtlId { get; set; }
        public PermissionsDTO Permissions { get; set; }
    }

    public class PermissionsDTO
    {
        public bool Add { get; set; }
        public bool View { get; set; }
        public bool Edit { get; set; }
        public bool Delete { get; set; }
        public bool Approve { get; set; }
        public bool RptPrint { get; set; }
        public bool RptDownload { get; set; }
        public bool DocDownload { get; set; }
        public bool DocUpload { get; set; }
    }
}
