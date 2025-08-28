using Dapper;
using System.Data;
using Payroll.Common.Helpers;
using RoleService.BAL.Models;
using UserService.BAL.Requests;
using Payroll.Common.CommonDto;
using UserService.DAL.Interface;
using Payroll.Common.EnumUtility;
using Payroll.Common.ApplicationConstant;
using System.Text.Json;
using System.ComponentModel.Design;
using UserService.BAL.Models;
using static Dapper.SqlMapper;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;

namespace UserService.DAL.Service
{
    public class UserServiceRepository : IUserRepository
    {
        private readonly IDbConnection _dbConnection;
        private readonly IConfiguration _configuration;
        public UserServiceRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            _dbConnection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }

        #region User core functionality

        // Note :- TFilter model not use in GetAllAsync(TModel,TFilter) because ristricted by / purojit sir.
        public async Task<IEnumerable<UserRequest>> GetAllAsync(string procedureName)
        {
            try
            {
                var result = await _dbConnection.QueryAsync<UserRequest>(procedureName, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as necessary.
                throw new Exception($"An error occurred while requesting data from {procedureName}", ex);
            }
        }
        public async Task<IEnumerable<UserListModel>> GetUsersListAsync(string procedureName)
        {
            try
            {
                var result = await _dbConnection.QueryAsync<UserListModel>(procedureName, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as necessary.
                throw new Exception($"An error occurred while requesting data from {procedureName}", ex);
            }
        }

        public async Task<IEnumerable<UserListModel>> GetLocationwiseUsersListAsync(string procedureName, int? Company_Id, int? Location_Id)
        {
            try
            {
                var dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@Company_Id", Company_Id, DbType.Int32);
                dynamicParameters.Add("@Location_Id", Location_Id, DbType.Int32);

                var result = await _dbConnection.QueryAsync<UserListModel>(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as necessary.
                throw new Exception($"An error occurred while requesting data from {procedureName}", ex);
            }
        }

        public async Task<IEnumerable<UserRoleMapping>> GetUserMapRoleById(string procedureName, object User_Id)
        {
            try
            {
                var dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@User_Id", User_Id);
                var result = await _dbConnection.QueryAsync<UserRoleMapping>(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as necessary.
                throw new Exception($"An error occurred while requesting data from {procedureName}", ex);
                return null;
            }
        }
        public async Task<UserRequest> GetByIdAsync(string procedureName, object Id)
        {
            try
            {
                var dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@Id", Id);
                var result = await _dbConnection.QuerySingleOrDefaultAsync<UserRequest>(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as necessary.
                throw new Exception($"An error occurred while requesting data from {procedureName}", ex);
                return null;
            }
        }

        //chirag gurjar payroll-359 5jan2025 
        public async Task<UserRequest> GetByIdAuthAsync(string procedureName, object Id,string isClsm) //isClmsUser Added 03-04-25 
        {
            try
            {
                var dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@Id", Id);
                dynamicParameters.Add("@Is_Clms_User", isClsm);
                var result = await _dbConnection.QueryAsync<UserRequest>(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure);
                var singleResult = result.FirstOrDefault();

                return singleResult;
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as necessary.
                throw new Exception($"An error occurred while requesting data from {procedureName}", ex);
                return null;
            }
        }
        #region Added By Priyanshi 14 Feb 2025 For User Edit Information Get
        public async Task<UserMapDetailModel> GetEditUserMapDetailsByIdAsync(string procedureName, int userId)
        {
            var dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("@User_Id", userId, DbType.Int32);
            using (var multi = await _dbConnection.QueryMultipleAsync(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure))
            {
                // 1. Read user details (first result set)
                var userDetails = await multi.ReadSingleOrDefaultAsync<UserMapDetailModel>() ?? new UserMapDetailModel();

                // Ensure lists are always initialized
                userDetails.Company_Ids = new List<int>();
                userDetails.LocationDetails = new List<LocationDetailModel>();
                userDetails.UserRoles = new List<UserRole>();

                // 2. Read Company_Ids (second result set)
                var companyIds = await multi.ReadAsync<int>();
                userDetails.Company_Ids = companyIds.ToList();

                // 3. Read LocationDetails (third result set)
                var locations = await multi.ReadAsync<LocationDetailModel>();
                userDetails.LocationDetails = locations.ToList();

                // 4. Read UserRoles (fourth result set)
                var roles = await multi.ReadAsync<UserRole>();
                userDetails.UserRoles = roles.ToList();

                return userDetails;
            }
        }

        public async Task<(IEnumerable<LocationWiseRole>, IEnumerable<RoleMenuHeader>)> GetEditUserLocationWiseRole(string procedureName, int userId, int companyId, int? correspondanceId)
        {
            using (var connection = _dbConnection)
            {
                var dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@User_Id", userId, DbType.Int32);
                dynamicParameters.Add("@Company_id", companyId, DbType.Int32);
                dynamicParameters.Add("@Correspondance_Id", correspondanceId, DbType.Int32);

                using (var multi = await connection.QueryMultipleAsync(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure))
                {
                    var locationWiseRoles = await multi.ReadAsync<LocationWiseRole>();
                    var roleMenuHeaders = correspondanceId.HasValue ? await multi.ReadAsync<RoleMenuHeader>() : Enumerable.Empty<RoleMenuHeader>();

                    return (locationWiseRoles, roleMenuHeaders);
                }
            }
        }

        #endregion

        // updated for payroll-377 (krunali)

        #region Added By Priyanshi 

        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail :- This API handles the addition of user details using the mapping based on the provided organization data.
        ///  Created Date   :- 26-Dec-2024
        ///  Change Date    :- 26-Dec-2024
        ///  Change detail  :- Not yet modified.
        /// </summary>
        /// <param name="model"> user detail to be added.</param>
        /// <returns>Returns a model response with the result of the operation.</returns>
        public async Task<UserRequest> AddAsync(string procedureName, UserRequest model)
        {
            try
            {
                var dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@UserId", model.UserId);
                dynamicParameters.Add("@userType_Id", model.userType_Id);
                dynamicParameters.Add("@Salutation", model.Salutation);
                dynamicParameters.Add("@FirstName", model.FirstName);
                dynamicParameters.Add("@MiddleName", model.MiddleName);
                dynamicParameters.Add("@LastName", model.LastName);
                dynamicParameters.Add("@Email", model.Email);
                dynamicParameters.Add("@Username", model.Username);
                dynamicParameters.Add("@ContactNo", model.PhoneNumber);
                dynamicParameters.Add("@ActivationCode", model.ActivationCode);
                dynamicParameters.Add("@ActivationLink", model.ActivationLink);
                dynamicParameters.Add("@CountryId", model.CountryId);
                dynamicParameters.Add("@CompanyId", model.CompanyId);
                dynamicParameters.Add("@CreatedBy", model.CreatedBy);
                dynamicParameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information);
                dynamicParameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Insert);
                dynamicParameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.UserMaster);
                dynamicParameters.Add("@Effective_From_Dt", model.EffectiveFromDt);

                // Populate UserMapLocation from Correspondance_ID
                foreach (var departmentId in model.Correspondance_ID)
                {
                    model.UserMapLocation.Add(new UserMapLocation
                    {
                        User_ID = model.UserId,
                        Company_Id = (byte)model.CompanyId,
                        Correspondance_ID = departmentId,
                        IsUserMapToLowLevel = false, // Adjust as necessary
                        IsActive = true,
                        IsDeleted = false
                    });
                }

                // Populate MapUserRole from Role_Menu_Header_Id
                foreach (var roleId in model.Role_Menu_Header_Id)
                {
                    model.MapUserRole.Add(new UserRoleMapping
                    {
                        User_Id = model.UserId,
                        Role_Menu_Header_Id = roleId,
                        //Role_Ids = roleId
                        Company_Id = (byte)model.CompanyId,
                        Default_Role = 0, // Adjust as necessary
                        Effective_From = model.EffectiveFromDt,//DateTime.UtcNow, // Example
                        IsActive = true
                    });
                }

                // Prepare UserMapLocation DataTable
                var userMapLocationDT = CreateUserMapLocationTable(model.UserMapLocation);
                dynamicParameters.Add("@UserMapLocation_Id", userMapLocationDT.AsTableValuedParameter("dbo.udt_map_userlocation"));

                // Prepare UserRole DataTable
                var userRoleTable = CreateUserRoleTable(model.MapUserRole);
                dynamicParameters.Add("@Mapuserrole", userRoleTable.AsTableValuedParameter("[dbo].[udt_tbl_map_userrole]"));

                // Prepare UserCompany parameters and execute
                await ExecuteUserCompanyProcedure(model);

                // Execute stored procedure
                var result = await _dbConnection.QueryFirstOrDefaultAsync(
                    procedureName,
                    dynamicParameters,
                    commandType: CommandType.StoredProcedure
                );
                // Check result for messages and status
                if (result != null)
                {
                    model.StatusMessage = result.ApplicationMessage;
                    model.MessageType = result.ApplicationMessageType;
                }
                return model;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while executing {procedureName}: {ex.Message}", ex);
            }
        }
        private DataTable CreateUserMapLocationTable(IEnumerable<UserMapLocation> userMapLocationList)
        {
            var table = new DataTable();
            table.Columns.Add("UserMapLocation_Id", typeof(int));
            table.Columns.Add("User_ID", typeof(int));
            table.Columns.Add("Company_Id", typeof(byte));
            table.Columns.Add("Correspondance_ID", typeof(int));
            table.Columns.Add("IsUserMapToLowLevel", typeof(bool));
            table.Columns.Add("IsActive", typeof(bool));
            table.Columns.Add("IsDeleted", typeof(bool));

            foreach (var location in userMapLocationList)
            {
                table.Rows.Add(
                    0,  // Allow for DBNull if UserMapLocation_Id is 0 (new entry)
                    location.User_ID,
                    location.Company_Id,
                    location.Correspondance_ID,
                    location.IsUserMapToLowLevel,
                    location.IsActive,
                    location.IsDeleted
                );
            }

            return table;
        }
        private DataTable CreateUserRoleTable(IEnumerable<UserRoleMapping> userRoleList)
        {
            var table = new DataTable();
            table.Columns.Add("Role_User_Id", typeof(int));
            table.Columns.Add("Role_Id", typeof(int));
            //table.Columns.Add("Company_Id", typeof(byte));
            table.Columns.Add("User_Id", typeof(int));
            table.Columns.Add("Role_Menu_Header_Id", typeof(int));
            table.Columns.Add("Default_Role", typeof(int));
            table.Columns.Add("Effective_From", typeof(DateTime));
            table.Columns.Add("IsActive", typeof(bool));
            foreach (var role in userRoleList)
            {
                table.Rows.Add(
                    role.Role_User_Id,
                    0,
                    role.User_Id,
                    role.Role_Menu_Header_Id,
                    role.Default_Role,
                    role.Effective_From,
                    role.IsActive
                );
            }
            return table;
        }
        private async Task ExecuteUserCompanyProcedure(UserRequest model)
        {

            var userCompanyParams = new DynamicParameters();
            userCompanyParams.Add("@User_Id", model.UserId);
            userCompanyParams.Add("@Company_Id", model.CompanyId);
            userCompanyParams.Add("@Effective_From_Dt", model.EffectiveFromDt);
            userCompanyParams.Add("@CreatedBy", model.CreatedBy);
            userCompanyParams.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information);
            userCompanyParams.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Insert);
            userCompanyParams.Add("@ModuleId", (int)EnumUtility.ModuleEnum.CompanyMaster);


            await _dbConnection.ExecuteAsync(
                "[dbo].[SP_AddUpdateMapUserCompany]",
                userCompanyParams,
                commandType: CommandType.StoredProcedure
            );
        }

        #endregion
        public async Task<UserRequest> UpdateAsync(string procedureName, UserRequest model)
        {
            try
            {
                var dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@UserId", model.UserId);
                dynamicParameters.Add("@FirstName", model.FirstName);
                dynamicParameters.Add("@MiddleName", model.MiddleName);
                dynamicParameters.Add("@LastName", model.LastName);
                dynamicParameters.Add("@Email", model.Email);
                dynamicParameters.Add("@Password", model.Password);
                dynamicParameters.Add("@CountryId", model.CountryId);
                dynamicParameters.Add("@CreatedBy", null);
                dynamicParameters.Add("@UpdatedBy", model.UpdatedBy); // Rohit Tiwari Note :-  comes from session userId.
                dynamicParameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information);
                dynamicParameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Update);
                dynamicParameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.UserMaster);

                var result = await _dbConnection.QuerySingleOrDefaultAsync<UserRequest>(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while requesting data from {procedureName}", ex);
                return null;
            }
        }

        public async Task<DeactivateUser> UpdateDeactiveUserStatusAsync(string storedProcedure, DeactivateUser model)
        {
            try
            {
                var dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@Country_Id", model.Country_Id);
                dynamicParameters.Add("@Company_Id", model.Company_Id);
                dynamicParameters.Add("@UserId", model.UserId);
                dynamicParameters.Add("@Deactivation_Type", model.Deactivation_Type);
                dynamicParameters.Add("@Deactivation_Reason", model.Deactivation_Reason);
                dynamicParameters.Add("@IsActive", model.IsActive);
                dynamicParameters.Add("@UpdatedBy", model.UpdatedBy);
                dynamicParameters.Add("@Approve_Reject_Level", model.Approve_Reject_Level);
                dynamicParameters.Add("@Effective_On", model.Effective_On);
                dynamicParameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information);
                dynamicParameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Update);
                dynamicParameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.UserMaster);

                var result = await _dbConnection.QuerySingleOrDefaultAsync<DeactivateUser>(
                    storedProcedure,
                    dynamicParameters,
                    commandType: CommandType.StoredProcedure
                );

                return result;
            }
            catch (Exception ex)
            {
                // Optional: Log the exception using a logger if available
                throw new Exception($"An error occurred while executing '{storedProcedure}': {ex.Message}", ex);
            }
        }




        #region CODE NOT USED 
        public async Task<UserRequest> DeleteAsync(string procedureName, UserRequest model)
        {
            try
            {
                var dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@UserId", model.UserId);
                dynamicParameters.Add("@UpdatedBy", model.UpdatedBy);
                dynamicParameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information);
                dynamicParameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Delete);
                dynamicParameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.UserMaster);

                var result = await _dbConnection.QuerySingleOrDefaultAsync<UserRequest>(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure);
                return result ?? new UserRequest();
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while requesting data from {procedureName}", ex);
                return null;
            }
        }
        #endregion

        public async Task<UserRequest> DeleteUserByIDAsync(string procedureName, object userInfo)
        {
            var parameters = new DynamicParameters();
            // Add the parameters required for the stored procedure
            parameters.Add("@UserId", ((dynamic)userInfo).UserId); // 0 for insert, greater than 0 for update
            parameters.Add("@UpdatedBy", ((dynamic)userInfo).UpdatedBy);
            // Additional parameters for messages and status using enum values
            parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information); // Cast enum to int
            parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Delete); // Cast enum to int
            parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.UserMaster); // Cast enum to int
            //Execute stored procedure
            var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
            // Check result for messages and status
            if (result != null)
            {
                ((dynamic)userInfo).StatusMessage = result.ApplicationMessage;
                ((dynamic)userInfo).MessageType = result.ApplicationMessageType;
            }
            return (UserRequest)userInfo;
        }
        #endregion

        #region User Role Mapping
        public async Task<string> GetRoleNameByIdAsync(string procedureName, int roleId)
        {
            try
            {
                var dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@RoleId", roleId);

                // Fetch role details. Assuming only one role is returned.
                var role = await _dbConnection.QueryFirstOrDefaultAsync<RoleMasterModel>(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure);

                // Check if role is found and return the role name
                return role?.RoleName;
            }
            catch (Exception ex)
            {
                // Log the exception (you might want to use a logging framework)
                // For example: _logger.LogError(ex, "Error retrieving role name by RoleId.");
            }
            return null;
        }
        public async Task<List<RoleMasterModel>> GetUserRoleMappingDetailAsync(string procedureName, object parameter)
        {
            try
            {
                var dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@UserId", parameter);
                var roles = await _dbConnection.QueryAsync<RoleMasterModel>(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure);

                return roles.ToList();
            }
            catch (Exception ex)
            {
                var message = ex.Message;
            }
            return null;
        }

        #endregion


        // Move to base service reso..  .

        public async Task<ResponseModel> UpdateUserPasswordAsync(string procedureName, SendEmailModel model)
        {
            try
            {
                var hashPassword = GenerateHashKeyHelper.HashKey(model.NewPassword);
                var dynamicParameters = new DynamicParameters();

                dynamicParameters.Add("@Email", model.Email);
                dynamicParameters.Add("@NewPassword", hashPassword);
                dynamicParameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information);
                dynamicParameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Update);
                dynamicParameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.UserMaster);

                // Use QuerySingleOrDefaultAsync to handle cases where no result is returned
                var response = await _dbConnection.QuerySingleOrDefaultAsync<ResponseModel>(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure);

                return response ?? new ResponseModel();
            }
            catch (Exception ex)
            {
                // Log or handle the exception
                throw new Exception($"An error occurred while requesting data from {procedureName}", ex);
                return new ResponseModel();
            }
        }

        public async Task<ResponseModel> SendEmailAsync(string procedureName, SendEmailModel model)
        {
            var response = new ResponseModel();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Email", model.Email);
                parameters.Add("@TemplateType", model.TemplateType);
                parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information);
                parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Send);
                parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.EmailMaster);

                response = await _dbConnection.QuerySingleOrDefaultAsync<ResponseModel>(procedureName, parameters, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                // log message in db (ex.Message);
                response.ApplicationMessage = "Failed to send email.";
            }
            return response;
        }

        public async Task<AuthConfigModel> VerifyOTPAsync(string procedureName, SendEmailModel model)
        {
            try
            {
                var dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@Email", model.Email);
                dynamicParameters.Add("@OTP", model.OTP);
                dynamicParameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information);
                dynamicParameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Fetch);
                dynamicParameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.EmailMaster);

                // Use QuerySingleOrDefaultAsync to get an AuthConfigModel from the stored procedure
                var authConfig = await _dbConnection.QuerySingleOrDefaultAsync<AuthConfigModel>(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure);

                // Return the response or a new instance if null
                return authConfig ?? new AuthConfigModel();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred while verifying OTP: {ex.Message}");
                // Return a new instance of AuthConfigModel with default values in case of an error
                return new AuthConfigModel();
            }
        }

        public async Task<ResponseModel> UpdateLoginActivityAsync(string procedureName, UserRequest userMaster)
        {
            var response = new ResponseModel();
            try
            {
                //userMaster.Ipaddress = "Empty IpAddress"; // Note :- pending so ignore this.
                var dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@UserId", userMaster.UserId);
                dynamicParameters.Add("@MaxAttempts", userMaster.MaxAttempts);
                dynamicParameters.Add("@LockAccount", userMaster.LockAccount);
                dynamicParameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information);
                dynamicParameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Update);
                dynamicParameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.UserMaster);

                response = await _dbConnection.QuerySingleOrDefaultAsync<ResponseModel>(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure);
                return response;
            }
            catch (Exception ex)
            {
                // log message in db (ex.Message);
                response.ApplicationMessage = MessageConstants.TechnicalIssue;
                return response;
            }
        }

        public async Task<UserRequest> UpdateUserAccountStatus(string procedureName, UserRequest model)
        {
            try
            {
                var dynamicParameters = new DynamicParameters();

                dynamicParameters.Add("@Country_Id", model.CountryId);
                dynamicParameters.Add("@Company_Id", model.CompanyId);
                dynamicParameters.Add("@UserId", model.UserId);
                dynamicParameters.Add("@Deactivation_Type", model.Deactivation_Type);
                dynamicParameters.Add("@Deactivation_Reason", model.Deactivation_Reason);
                dynamicParameters.Add("@IsActive", model.IsActive);
                dynamicParameters.Add("@UpdatedBy", model.UpdatedBy);
                dynamicParameters.Add("@Effective_On", model.EffectiveOn);
                dynamicParameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information);
                dynamicParameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Deactivated);
                dynamicParameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.UserMaster);

                // Use QuerySingleOrDefaultAsync to handle cases where no result is returned
                var response = await _dbConnection.QuerySingleOrDefaultAsync<UserRequest>(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure);

                return response ?? new UserRequest();
            }
            catch (Exception ex)
            {
                // Log or handle the exception
                throw new Exception($"An error occurred while requesting data from {procedureName}", ex);
                return new UserRequest();
            }
        }

        public async Task<UserRequest> UpdateUserActiveDeactiveStatus(string procedureName, int Id)
        {
            try
            {
                var dynamicParameters = new DynamicParameters();

                dynamicParameters.Add("@Srv_Appr_Rej_Id", Id);
                dynamicParameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information);
                dynamicParameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Update);
                dynamicParameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.UserMaster);

                // Use QuerySingleOrDefaultAsync to handle cases where no result is returned
                var response = await _dbConnection.QuerySingleOrDefaultAsync<UserRequest>(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure);

                return response ?? new UserRequest();
            }
            catch (Exception ex)
            {
                // Log or handle the exception
                throw new Exception($"An error occurred while requesting data from {procedureName}", ex);
                return new UserRequest();
            }
        }

        #region Added By Harshida 23-12'24

        /// <summary>
        /// GetUserAdditionalDetailsAsync Method helps you to get the Additional of User Before login
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<(IEnumerable<UserCompanyDetails>, IEnumerable<UserLocationDetails>, IEnumerable<UserRoleDetails>)> GetUserAdditionalDetailsAsync(string procedureName, int userId)
        {
            using var dbConnection = _dbConnection; // Assuming `_dbConnection` is available
            var dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("@User_Id", userId);

            using var multi = await dbConnection.QueryMultipleAsync(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure);

            var companyDetails = multi.Read<UserCompanyDetails>().ToList();
            var locationDetails = multi.Read<UserLocationDetails>().ToList();
            var roleDetails = multi.Read<UserRoleDetails>().ToList();

            return (companyDetails, locationDetails, roleDetails);
        }
        public async Task<string> AddOrUpdateUserRoleMenuAsync(string procedureName, AddUpdateUserRoleMenuRequest request)
        {
            var parameters = new DynamicParameters();
            var dataTable = new DataTable();
            dataTable.Columns.Add("Role_User_Dtl_Id", typeof(int));
            dataTable.Columns.Add("Role_Menu_Dtl_Id", typeof(int));
            dataTable.Columns.Add("Menu_Id", typeof(int));
            dataTable.Columns.Add("HasPerDtl", typeof(bool));
            dataTable.Columns.Add("GrantAdd", typeof(bool));
            dataTable.Columns.Add("GrantEdit", typeof(bool));
            dataTable.Columns.Add("GrantView", typeof(bool));
            dataTable.Columns.Add("GrantDelete", typeof(bool));
            dataTable.Columns.Add("GrantApprove", typeof(bool));
            dataTable.Columns.Add("GrantRptPrint", typeof(bool));
            dataTable.Columns.Add("GrantRptDownload", typeof(bool));
            dataTable.Columns.Add("DocDownload", typeof(bool));
            dataTable.Columns.Add("DocUpload", typeof(bool));

            foreach (var detail in request.UserRoleMenuDetails)
            {
                dataTable.Rows.Add(
                    detail.Role_User_Dtl_Id,
                    detail.Role_Menu_Dtl_Id,
                    detail.Menu_Id,
                    detail.HasPerDtl,
                    detail.GrantAdd,
                    detail.GrantEdit,
                    detail.GrantView,
                    detail.GrantDelete,
                    detail.GrantApprove,
                    detail.GrantRptPrint,
                    detail.GrantRptDownload,
                    detail.DocDownload,
                    detail.DocUpload
                );
            }
            parameters.Add("@Role_User_Id", request.Role_User_Id);
            parameters.Add("@Role_Menu_Hdr_Id", request.Role_Menu_Hdr_Id);
            parameters.Add("@User_ID", request.User_Id);
            parameters.Add("@Company_ID", request.Company_Id);
            parameters.Add("@Correspondance_Id", request.Correspondance_Id);
            parameters.Add("@EffectiveFrom", request.EffectiveFrom);
            parameters.Add("@CreatedBy", request.CreatedBy);
            parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information);
            parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Insert);
            parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.UserMaster);
            parameters.Add("@UserRoleMenuDetails", dataTable.AsTableValuedParameter(DbConstants.UDTUserRoleMenuDetails));
            var result = await _dbConnection.QueryFirstOrDefaultAsync<string>(
              // var result = await _dbConnection.ExecuteScalarAsync<string>(
              procedureName,
              parameters,
              commandType: CommandType.StoredProcedure
          );
            // Check result for messages and status
            //if (result != null)
            //{
            //    request.StatusMessage = result.ApplicationMessage;
            //    request.MessageType = result.ApplicationMessageType;
            //}
            return result; // Return the JSON string directly
        }
        /// <summary>
        /// Developer Name:- Harshida Parmar (Date-27-03-25)
        /// AddLoginHistoryAsync Method helps you to insert User details in tbl_trn_login_history Before login     
        /// Modified By and Date:-    
        /// <returns></returns>
        /// </summary>
        /// <param name="procedureName"></param>
        /// <param name="loginHistory"></param>
        /// <returns></returns>
        public async Task<LoginHistory> AddLoginHistoryAsync(string procedureName, LoginHistory loginHistory)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@User_Id", loginHistory.UserId);
            parameters.Add("@Log_In_Time", loginHistory.LogInTime);
            parameters.Add("@Log_Out_Time", loginHistory.LogOutTime);
            parameters.Add("@Ip_Address", loginHistory.IpAddress);
            parameters.Add("@Operation_Type", "I"); // 'I' = Insert

            parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information, DbType.Int32);
            parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Insert, DbType.Int32);
            parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.UserTransationHistory, DbType.Int32);

            //await _dbConnection.ExecuteAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);

            //loginHistory.MessageType = parameters.Get<int>("@MessageType");
            //loginHistory.StatusMessage = parameters.Get<string>("@ApplicationMessage");
            var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
            if (result != null)
            {
                loginHistory.StatusMessage = result.ApplicationMessage;
                loginHistory.MessageType = result.ApplicationMessageType;
            }

            return loginHistory;
        }
        public async Task<LoginHistory> UpdateLoginHistoryAsync(string procedureName, LoginHistory loginHistory)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@User_Id", loginHistory.UserId);
            parameters.Add("@Log_Out_Time", loginHistory.LogOutTime);
            parameters.Add("@Ip_Address", loginHistory.IpAddress);
            parameters.Add("@Operation_Type", "U"); // 'U' = Update

            parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information, DbType.Int32);
            parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Update, DbType.Int32);
            parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.UserTransationHistory, DbType.Int32);

            //await _dbConnection.ExecuteAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);

            //loginHistory.MessageType = parameters.Get<int>("@MessageType");
            //loginHistory.StatusMessage = parameters.Get<string>("@ApplicationMessage");
            var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
            if (result != null)
            {
                loginHistory.StatusMessage = result.ApplicationMessage;
                loginHistory.MessageType = result.ApplicationMessageType;
            }
            return loginHistory;            
        }
        public async Task<int> GetUserLoginStatusAsync(string procedureName, LoginHistoryRequestModel model)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@User_Id", model.UserId);
            parameters.Add("@UseerName", model.UserName);
            parameters.Add("@DbStatus", model.DbStatus);
            parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information, DbType.Int32);
            parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Exist, DbType.Int32);
            parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.UserTransationHistory, DbType.Int32);
            parameters.Add("@userExistOut", dbType: DbType.Int32, direction: ParameterDirection.Output); 

            // Execute stored procedure
            await _dbConnection.ExecuteAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);

            // Retrieve the output parameter value
            int userExistCount = parameters.Get<int>("@userExistOut"); 

            return userExistCount;
        }

        // updated for payroll-377 (krunali)
        public async Task<UserRequest> GetUserMapDetailsByIdAsync(string procedureName, object User_Id)
        {

            var dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("@User_Id", User_Id);
            using (var multi = await _dbConnection.QueryMultipleAsync(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure))
            {
                var result = new UserRequest
                {
                    userDetails = (await multi.ReadAsync<UserRequest>()).ToList(),
                    maproleDetails = (await multi.ReadAsync<UserRoleMapping>()).ToList(),

                };
                return result;
            }
            //using var multiResult = await _dbConnection.QueryMultipleAsync(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure);
            //var userDetails = multiResult.Read<UserRequest>();
            //var roleDetails = multiResult.Read<UserRoleMapping>();
            //return (userDetails,roleDetails);
        }

        #region Added By Priyanshi 17 Feb 2025 for Update User Role And Location.
        public async Task<UpdateUserRoleStatusModel> UpdateUserRoleStatusAsync(string procedureName, UpdateUserRoleStatusModel updateUserRoleStatus)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@User_Id", updateUserRoleStatus.User_Id, DbType.Int32);
            parameters.Add("@Role_User_Id", updateUserRoleStatus.Role_User_Id, DbType.Int32);
            parameters.Add("@ActualActivestatus", updateUserRoleStatus.ActualActivestatus, DbType.Boolean);
            parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information, DbType.Int32);
            parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Update, DbType.Int32);
            parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.UserMaster, DbType.Int32);


            // Execute stored procedure
            var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
            // Check result for messages and status
            if (result != null)
            {
                updateUserRoleStatus.StatusMessage = result.ApplicationMessage;
                updateUserRoleStatus.MessageType = result.MessageType;
            }
            return updateUserRoleStatus;

        }

        public async Task<UpdateUserLocationStatusModel> UpdateUserLocationStatusAsync(string procedureName, UpdateUserLocationStatusModel updateUserLocationStatus)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@User_Id", updateUserLocationStatus.User_Id, DbType.Int32);
            parameters.Add("@Correspondance_ID", updateUserLocationStatus.Correspondance_ID, DbType.Int32);
            parameters.Add("@ActualActivestatus", updateUserLocationStatus.ActualActivestatus, DbType.Boolean);
            parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information, DbType.Int32);
            parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Update, DbType.Int32);
            parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.UserMaster, DbType.Int32);


            // Execute stored procedure
            var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
            // Check result for messages and status
            if (result != null)
            {
                updateUserLocationStatus.StatusMessage = result.ApplicationMessage;
                updateUserLocationStatus.MessageType = (int)result.MessageType;
            }
            return updateUserLocationStatus;

        }
        #endregion

        #endregion

        #region change user password Added By Abhishek 17-02-2025
        public async Task<UserRequest> ChangeUserPasswordAsync(string procedureName, UserRequest model)
        {
            try
            {
                var hashNewPassword = GenerateHashKeyHelper.HashKey(model.NewPassword);
                var hashOldPassword = GenerateHashKeyHelper.HashKey(model.OldPassword);
                var dynamicParameters = new DynamicParameters();

                dynamicParameters.Add("@OldPassword", hashOldPassword);
                dynamicParameters.Add("@userId", model.UserId);

                dynamicParameters.Add("@NewPassword", hashNewPassword);
                dynamicParameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information);
                dynamicParameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Update);
                dynamicParameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.UserMaster);

                // Use QuerySingleOrDefaultAsync to handle cases where no result is returned
                var result = await _dbConnection.QuerySingleOrDefaultAsync<UserRequest>(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure);
                return result;

            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while requesting data from {procedureName}", ex);
                return null;
            }
        }
        #endregion 

    }
}
