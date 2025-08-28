using Dapper;
using Microsoft.AspNetCore.Mvc;
using Payroll.Common.EnumUtility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.BAL.Models;
using UserService.BAL.Requests;
using UserService.DAL.Interface;

namespace UserService.DAL.Service
{
    /// <summary>
    /// Developer Name :- Harshida Parmar
    /// Created Date   :- 15-Oct-2024
    /// Message detail :- Mapping User Role Service Repository perform CRUD
    /// </summary>
    public class MappingUserRoleRepository : IMappingUserRoleRepository
    {
        #region CTOR and Private Variable
        private readonly IDbConnection _dbConnection;

        public MappingUserRoleRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
        #endregion
        #region MappingUserRole CRUD
        #region MappingUserRoleAdd
        public async Task<UserRoleMapping> AddUserRoleMappingAsync(string procedureName, UserRoleMapping userRoleMapping)
        {
            // Convert UserRoleMapping to DataTable
            var userRoleMapTable = CreateUserRoleMapDataTable(userRoleMapping);

            // Create dynamic parameters
            var parameters = new DynamicParameters();
            parameters.Add("@MapRoleUser_Id", userRoleMapping.Role_User_Id);
            parameters.Add("@Company_Id", userRoleMapping.Company_Id);
            parameters.Add("@Mapuserrole", userRoleMapTable.AsTableValuedParameter("udt_tbl_map_userrole"));
            parameters.Add("@CreatedBy", userRoleMapping.CreatedBy);
            //parameters.Add("@UpdatedBy", userRoleMapping.UpdatedBy);
            parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information);
            parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Insert);
            parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.MappingUserRole);

            try
            {
                var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
                // Map the output values
                if (result != null)
                {
                    userRoleMapping.StatusMessage = result.ApplicationMessage;
                    userRoleMapping.MessageType = result.ApplicationMessageType;
                }
                return userRoleMapping;

            }
            catch (Exception ex)
            {
                throw new Exception($"Error in AddUserRoleMappingAsync: {ex.Message}", ex);
            }
        }
        #endregion
        #region Common_CreateDataTable
        public static DataTable CreateUserRoleMapDataTable(UserRoleMapping userRoleMapping)
        {
            var table = new DataTable();

            table.Columns.Add("Role_User_Id", typeof(int));
            table.Columns.Add("Role_Id", typeof(int));
            table.Columns.Add("User_Id", typeof(int));
            table.Columns.Add("Role_Menu_Header_Id", typeof(int));
            table.Columns.Add("Effective_From", typeof(DateTime));
            table.Columns.Add("IsActive", typeof(bool));

            foreach (var roleId in userRoleMapping.Role_Ids)
            {
                table.Rows.Add(
                    userRoleMapping.Role_User_Id,
                    roleId,
                    userRoleMapping.User_Id,
                    userRoleMapping.Role_Menu_Header_Id,
                    userRoleMapping.Effective_From,
                    true
                );
            }
            return table;
        }
        #endregion
        #region MappingUserRoleDelete
        public Task<int> DeleteUserRoleMappingAsync(int roleUserId)
        {
            throw new NotImplementedException();
        }
        public async Task<UserRoleMapping> DeleteUserRoleMappingAsync(string procedureName, object roleUserDetail)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Role_User_Id", ((dynamic)roleUserDetail).Role_User_Id, DbType.Int32);
            parameters.Add("@UpdatedBy", ((dynamic)roleUserDetail).UpdatedBy, DbType.Int32);
            // Additional parameters for messages and status using enum values
            parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information);
            parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Delete);
            parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.MappingUserRole);
            var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);

            if (result != null)
            {
                ((dynamic)roleUserDetail).StatusMessage = result.ApplicationMessage;
                ((dynamic)roleUserDetail).MessageType = result.ApplicationMessageType;
            }
            return (UserRoleMapping)roleUserDetail;
        }
        #endregion
        #region MappingUserRoleUpdate
        public async Task<UserRoleMapping> UpdateUserRoleMappingAsync(string procedureName, UserRoleMapping userRoleMapping)
        {
            // Convert UserRoleMapping to DataTable
            var userRoleMapTable = CreateUserRoleMapDataTable(userRoleMapping);

            // Create dynamic parameters
            var parameters = new DynamicParameters();
            parameters.Add("@MapRoleUser_Id", userRoleMapping.Role_User_Id); // Non-zero value for update
            parameters.Add("@Company_Id", userRoleMapping.Company_Id);
            parameters.Add("@Mapuserrole", userRoleMapTable.AsTableValuedParameter("udt_tbl_map_userrole"));
            parameters.Add("@CreatedBy", userRoleMapping.CreatedBy); // Required for inserts
            parameters.Add("@UpdatedBy", userRoleMapping.UpdatedBy); // Required for updates
            parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information);
            parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Update); // MessageMode for update
            parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.MappingUserRole);

            try
            {
                // Execute the stored procedure
                var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);

                // var result1 = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
                return userRoleMapping;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in UpdateUserRoleMappingAsync: {ex.Message}", ex);
            }
        }

        #endregion
        #region MappingUserRoleSelectALLAndSelectByID
        public async Task<IEnumerable<UserRoleMappingRequest>> GetAllUserRoleMappingsAsync(string procedureName, int? roleUserId = null, bool? isActive = null)
        {
            var parameters = new DynamicParameters();

            if (roleUserId.HasValue)
                parameters.Add("@Role_User_Id", roleUserId.Value);

            if (isActive.HasValue)
                parameters.Add("@IsActive", isActive.Value);

            var result = await _dbConnection.QueryAsync<UserRoleMappingRequest>(
                procedureName,
                parameters,
                commandType: CommandType.StoredProcedure);

            return result;
        }
        public async Task<UserRoleMappingRequest> GetUserRoleMappingByIdAsync(string procedureName, object parameters)
        {
            var dynamicParameters = new DynamicParameters(parameters); // Pass the entire object directly to DynamicParameters
            return await _dbConnection.QuerySingleOrDefaultAsync<UserRoleMappingRequest>(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure);
        }
        #endregion
        #endregion
        #region User Role Based Menu Endpoint Handlers (CRUD)
        #region MappingUserRoleSelectALLAndSelectByID
        public async Task<IEnumerable<UserRoleBasedMenuRequest>> GetAllUserRoleMenuAsync(string procedureName, int? companyid = null, int? roleid = null, int? userid = null, int? userMapLocation_Id = null)
        {
            var parameters = new DynamicParameters();

            if (companyid.HasValue)
                parameters.Add("@Company_Id", companyid.Value);

            if (roleid.HasValue)
                parameters.Add("@Role_Id", roleid.Value);

            if (userid.HasValue)
                parameters.Add("@User_Id", userid.Value);

            if (userMapLocation_Id.HasValue)
                parameters.Add("@UserMapLocation_Id", userMapLocation_Id.Value);

            var result = await _dbConnection.QueryAsync<UserRoleBasedMenuRequest>(
                procedureName,
                parameters,
                commandType: CommandType.StoredProcedure);

            return result;
        }
        #endregion

        #region MappingUserRoleSelectALLAndSelectByID for edit menu
        public async Task<IEnumerable<UserRoleBasedMenuRequest>> GetAllUserRoleMenuEditAsync(string procedureName, int? companyid = null, int? roleid = null, int? userid = null, int? rolemenuheaderid = null,int? correspondanceId = null)
        {
            var parameters = new DynamicParameters();

            if (companyid.HasValue)
                parameters.Add("@Company_Id", companyid.Value);

            if (roleid.HasValue)
                parameters.Add("@Role_Id", roleid.Value);

            if (userid.HasValue)
                parameters.Add("@User_Id", userid.Value);

            if (rolemenuheaderid.HasValue)
                parameters.Add("@Role_Menu_Hdr_Id", rolemenuheaderid.Value); 
            
            if (correspondanceId.HasValue)
                parameters.Add("@Correspondance_ID", correspondanceId.Value);

            var result = await _dbConnection.QueryAsync<UserRoleBasedMenuRequest>(
                procedureName,
                parameters,
                commandType: CommandType.StoredProcedure);

            return result;
        }

        public async Task<IEnumerable<BreadCrumbMaster>> GetBreadcrumbByMenuIdAsync(string procedureName, object parameters)
        {
            var dynamicParameters = new DynamicParameters(parameters);
            return await _dbConnection.QueryAsync<BreadCrumbMaster>(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure);

        }
        #endregion
        #endregion
    }
}

#region BACKUP CODE 
//public async Task<UserRoleMapping> AddUserRoleMappingAsync(string storedProcedure, UserRoleMapping userRoleMapping)
//{
//    using (var connection = _dbConnection)  // _dbConnection is your database connection
//    {
//        // Create and populate a DataTable for the UserRoleMappings
//        var userRoleMappingTable = new DataTable();
//        userRoleMappingTable.Columns.Add("Role_Id", typeof(int));               // Role_Id (from your list)
//        userRoleMappingTable.Columns.Add("User_Id", typeof(int));               // User_Id
//        userRoleMappingTable.Columns.Add("Role_Menu_Header_Id", typeof(int));   // Role_Menu_Header_Id
//        userRoleMappingTable.Columns.Add("Effective_From", typeof(DateTime));   // Effective_From
//        userRoleMappingTable.Columns.Add("IsActive", typeof(bool));             // IsActive

//        // Populate the DataTable with UserRoleMapping records
//        foreach (var role in userRoleMapping.)
//        {
//            // Add a row for each role in the list
//            userRoleMappingTable.Rows.Add(
//                role.RoleId,                   // Role_Id
//                userRoleMapping.UserId,        // User_Id
//                role.RoleMenuHeaderId,         // Role_Menu_Header_Id
//                role.EffectiveFrom,            // Effective_From
//                role.IsActive                  // IsActive
//            );
//        }

//        // Prepare the parameters for the stored procedure
//        var parameters = new DynamicParameters();
//        parameters.Add("@Mapuserrole", userRoleMappingTable.AsTableValuedParameter("dbo.udt_tbl_map_userrole")); // Table-Valued Parameter
//        parameters.Add("@MapRoleUser_Id", userRoleMapping.MapRoleUserId);  // Pass 0 for insert, >0 for update
//        parameters.Add("@Company_Id", userRoleMapping.Company_Id);          // Company ID
//        parameters.Add("@CreatedBy", userRoleMapping.CreatedBy);            // Created By
//        parameters.Add("@UpdatedBy", userRoleMapping.UpdatedBy);            // Updated By (optional)
//        parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information); // Example Enum casting
//        parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Insert);     // Example Enum casting
//        parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.RoleMenuMapping);  // Example Enum casting

//        try
//        {
//            // Execute the stored procedure
//            var result = await connection.ExecuteAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure);

//            // If necessary, process the result (if the stored procedure returns something)
//        }
//        catch (Exception ex)
//        {
//            // Handle any errors that might occur
//            throw new Exception("Error occurred while adding/updating the UserRoleMapping.", ex);
//        }

//        // After the operation, retrieve the updated or newly created Role User ID or other details (if returned)
//        userRoleMapping.MapRoleUserId = parameters.Get<int>("@MapRoleUser_Id");

//        return userRoleMapping;
//    }
//{
//    "AssignRole_Id": 0,
//  "Company_Id":1,
//  "user_Id": 3,
//  "Role_Id":2,
//  "effective_From_Dt": "2024-10-16T13:01:39.030Z", 
//  "CreatedBy": 1,
//  "Messagetype": 1,              
//  "MessageMode": 1,              
//  "ModuleId": 16
//}
//}
#endregion