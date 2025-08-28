using UserService.BAL.Requests;

namespace Payroll.WebApp.Models.DTOs
{
    public class UserRoleMenuDTO
    {
        public int Menu_Id { get; set; }
        public string MenuName { get; set; }
        public string ActionURL { get; set; }
        public int ParentMenuId { get; set; }
        public int Level { get; set; }
        public int DisplayOrder { get; set; }
        public bool HasPerDtl { get; set; }
        public int Role_Menu_Hdr_Id { get; set; } //Added By Harshida 16-01-'25
        public int Role_Menu_Dtl_Id { get; set; }//Added By Harshida 16-01-'25
        public bool GrantAdd { get; set; }
        public bool GrantView { get; set; }
        public bool GrantEdit { get; set; }
        public bool GrantDelete { get; set; }
        public bool GrantRptPrint { get; set; }
        public bool GrantRptDownload { get; set; }
        public bool DocDownload { get; set; }
        public bool DocUpload { get; set; }
    }

    #region Added By Harshida(17-01-'25) [NOTE:- DO NOT Write Any code without asking Abhishek OR Harshida Or Priyanshi]
    public static class RoleMenuPermissionsMapper
    {
        public static AddUpdateUserRoleMenuRequest MapToApiRequest(RoleMenuPermissionsModelDTO model, int createdBy, DateTime effectiveFrom)
        {
            return new AddUpdateUserRoleMenuRequest
            {
                Role_User_Id = 0, // Default for insert
                Role_Menu_Hdr_Id = model.PermissionsData.FirstOrDefault()?.RoleMenuHdrId ?? 0,
                User_Id = model.UserId,
                Company_Id = model.CompanyId,
                Correspondance_Id = model.CorrespondanceId,
                EffectiveFrom = model.EffectiveFromDt,
                CreatedBy = createdBy,
                UserRoleMenuDetails = model.PermissionsData.Select(p => new UserRoleMenuDetail
                {
                    Role_User_Dtl_Id = 0, // Default for insert
                    Role_Menu_Dtl_Id = p.RoleMenuDtlId,
                    Menu_Id = p.MenuId,
                    HasPerDtl = true, 
                    GrantAdd = p.Permissions.Add,
                    GrantEdit = p.Permissions.Edit,
                    GrantView = p.Permissions.View,
                    GrantDelete = p.Permissions.Delete,
                    GrantApprove = p.Permissions.Approve,
                    GrantRptPrint = p.Permissions.RptPrint,
                    GrantRptDownload = p.Permissions.RptDownload,
                    DocDownload = p.Permissions.DocDownload,
                    DocUpload = p.Permissions.DocUpload
                }).ToList()
            };
        }
    }

    #endregion
}
