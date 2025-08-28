namespace Payroll.WebApp.Models.DTOs
{
    public class UserRoleBasedMenuDTO
    {
        public int Menu_Id { get; set; }
        public string MenuName { get; set; }
        public string ActionUrl { get; set; }
        public int? ParentMenuId { get; set; }
        public int Level { get; set; }
        public int DisplayOrder { get; set; }

        #region Added By Krunali 
        public bool HasPerDtl { get; set; }
        public bool GrantAdd { get; set; } 
        public bool GrantView { get; set; }
        public bool GrantEdit { get; set; }
        public bool GrantApprove { get; set; } 
        public bool GrantDelete { get; set; }
        public bool GrantRptDownload { get; set; } 
        public bool GrantRptPrint { get; set; }
        #endregion
    }
}
