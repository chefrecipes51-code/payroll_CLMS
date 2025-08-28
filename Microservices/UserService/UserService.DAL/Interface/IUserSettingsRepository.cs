using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.BAL.Models;
using UserService.BAL.Requests;

namespace UserService.DAL.Interface
{
    public interface IUserSettingsRepository
    {
        Task<RoleOrLocationRequest> UpdateUserRoleLocationAsync(string procedureName, RoleOrLocationRequest userRoleMapping);
        // /// IMP NOTE ADDING THESE BELOW METHOD AS CONNECTION STRING BECOME NULL
        Task<(IEnumerable<UserCompanyDetails>, IEnumerable<UserLocationDetails>, IEnumerable<UserRoleDetails>)> GetUserAdditionalDetailsAsync(string procedureName, int userId);
        Task<(string ApplicationMessage, int ApplicationMessageType)> CheckUserExistAsync(string procedureName, string email);
        Task<UserCompanyRoleLocation> GetUserRoleLocationAsync(string procedureName, int userId, int? companyId = null, int? userMapLocationId = null);

    }
}
