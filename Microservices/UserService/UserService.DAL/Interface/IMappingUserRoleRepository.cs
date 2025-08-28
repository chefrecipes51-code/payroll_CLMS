using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.BAL.Models;
using UserService.BAL.Requests;

namespace UserService.DAL.Interface
{
    public interface IMappingUserRoleRepository
    {
        Task<UserRoleMapping> AddUserRoleMappingAsync(string procedureName, UserRoleMapping userRoleMapping);
        Task<UserRoleMappingRequest> GetUserRoleMappingByIdAsync(string procedureName, object parameters);
        Task<IEnumerable<UserRoleMappingRequest>> GetAllUserRoleMappingsAsync(string procedureName, int? roleUserId = null, bool? isActive = null);
        Task<UserRoleMapping> UpdateUserRoleMappingAsync(string procedureName, UserRoleMapping userRoleMapping);
        Task<UserRoleMapping> DeleteUserRoleMappingAsync(string procedureName, object roleUserDetail);
        //Added By Harshida 10-01-25
        Task<IEnumerable<UserRoleBasedMenuRequest>> GetAllUserRoleMenuAsync(string procedureName, int? companyid = null, int? roleid = null, int? userid = null, int? userMapLocation_Id = null);
        //Added By Priyanshi Jain 21-02-25
        Task<IEnumerable<UserRoleBasedMenuRequest>> GetAllUserRoleMenuEditAsync(string procedureName, int? companyid = null, int? roleid = null, int? userid = null, int? rolemenuheaderid = null, int? correspondanceId = null);
        Task<IEnumerable<BreadCrumbMaster>> GetBreadcrumbByMenuIdAsync(string procedureName, object parameters);
    }
}
