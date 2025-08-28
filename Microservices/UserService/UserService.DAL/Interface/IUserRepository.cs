using System.Globalization;
using Payroll.Common.ApplicationModel;
using Payroll.Common.CommonDto;
using RoleService.BAL.Models;
using UserService.BAL.Models;
using UserService.BAL.Requests;

namespace UserService.DAL.Interface
{
    public interface IUserRepository : IGenericRepository<UserRequest>
    {
        //updated by krunali gohil payroll-377
        //Task<UserRequest> GetUserMapDetailsByIdAsync(string procedureName, object User_Id);

        Task<IEnumerable<UserListModel>> GetUsersListAsync(string procedureName);
        Task<IEnumerable<UserListModel>> GetLocationwiseUsersListAsync(string procedureName, int? Company_Id, int? Location_Id);
        Task <UserRequest> GetUserMapDetailsByIdAsync(string procedureName, object User_Id);

        //Task<UserRequest> GetByIdAuthAsync(string procedureName, object User_Id);
        Task<UserRequest> GetByIdAuthAsync(string procedureName, object User_Id, string isClsm); // New parameter add for To check IsCLMS USER
        Task<UserMapDetailModel> GetEditUserMapDetailsByIdAsync(string procedureName, int userId);
        Task<(IEnumerable<LocationWiseRole>, IEnumerable<RoleMenuHeader>)> GetEditUserLocationWiseRole(string procedureName, int userId, int companyId, int? correspondanceId);
        Task<UpdateUserRoleStatusModel> UpdateUserRoleStatusAsync(string storedProcedure, UpdateUserRoleStatusModel updateUserRoleStatus);
        Task<UpdateUserLocationStatusModel> UpdateUserLocationStatusAsync(string storedProcedure, UpdateUserLocationStatusModel updateUserLocationStatus);
        Task<DeactivateUser> UpdateDeactiveUserStatusAsync(string storedProcedure, DeactivateUser deactivateUser);
        #region User Role Mapping

        Task<List<RoleMasterModel>> GetUserRoleMappingDetailAsync(string procedureName, object parameters);
        Task<string> GetRoleNameByIdAsync(string procedureName, int roleId);
        //Task<IEnumerable<UserRoleMapping>> GetUserMapRoleById (String procedureName, object User_Id);

        //Task<List<UserRoleMapping>> GetUserMapRoleById(String procedureName, object User_Id);


        Task<ResponseModel> UpdateLoginActivityAsync(string procedureName, UserRequest model);
        Task<ResponseModel> SendEmailAsync(string procedureName, SendEmailModel model);
        Task<AuthConfigModel> VerifyOTPAsync(string procedureName, SendEmailModel model);
        Task<ResponseModel> UpdateUserPasswordAsync(string procedureName, SendEmailModel model);
        Task<UserRequest> UpdateUserAccountStatus(string procedureName, UserRequest model);
        Task<UserRequest> ChangeUserPasswordAsync(string procedureName, UserRequest model);
        Task<UserRequest> UpdateUserActiveDeactiveStatus(string procedureName, int id);

        #endregion

        #region Added By Harshida
        Task<UserRequest> DeleteUserByIDAsync(string procedureName, object parameters); //Added By Harshida 01-01-'25
        Task<(IEnumerable<UserCompanyDetails>, IEnumerable<UserLocationDetails>, IEnumerable<UserRoleDetails>)> GetUserAdditionalDetailsAsync(string procedureName, int userId); // Added By Harshida  23-12-'24
        Task<string> AddOrUpdateUserRoleMenuAsync(string procedureName, AddUpdateUserRoleMenuRequest request);//Added By Harshida 17-01-'25
        Task<LoginHistory> AddLoginHistoryAsync(string procedureName, LoginHistory loginHistory);
        Task<LoginHistory> UpdateLoginHistoryAsync(string procedureName, LoginHistory loginHistory);
        Task<int> GetUserLoginStatusAsync(string procedureName, LoginHistoryRequestModel model);
        #endregion
    }

    // Note :- below code is commited because purojit sir not want to apply advance filter on all getallasync type methods or it is option to apply  based on modules
    //public interface IUserRepository : IGenericRepository<UserRequest, CommonFilterModel>
    //{
    //    #region User Role Mapping

    //    Task<List<RoleMasterModel>> GetUserRoleMappingDetailAsync(string procedureName, object parameters);
    //    Task<string> GetRoleNameByIdAsync(string procedureName, int roleId);
    //    Task<ResponseModel> UpdateLoginActivityAsync(string procedureName, UserRequest model);
    //    Task<ResponseModel> SendEmailAsync(string procedureName, SendEmailModel model);
    //    Task<AuthConfigModel> VerifyOTPAsync(string procedureName, SendEmailModel model);
    //    Task<ResponseModel> UpdateUserPasswordAsync(string procedureName, SendEmailModel model);
    //    Task<UserRequest> UpdateUserAccountStatus(string procedureName, UserRequest model);
    //    Task<UserRequest> ApproveOrRejectUpdateUserAccountStatus(string procedureName, int id);

    //    #endregion
    //}

}
