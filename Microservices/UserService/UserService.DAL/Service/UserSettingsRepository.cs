using Dapper;
using Microsoft.Extensions.Configuration;
using Payroll.Common.ApplicationConstant;
using Payroll.Common.EnumUtility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.BAL.Models;
using UserService.BAL.Requests;
using UserService.DAL.Interface;

namespace UserService.DAL.Service
{
    public class UserSettingsRepository : IUserSettingsRepository
    {
        #region CTOR and Private Variable
        private readonly IDbConnection _dbConnection;
        private readonly IConfiguration _configuration;
        public UserSettingsRepository(IConfiguration configuration,IDbConnection dbConnection)
        {
            //_dbConnection = dbConnection;
            _configuration = configuration;
            _dbConnection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }
        #endregion
        #region UpdateUserRoleORLocation 
        public async Task<RoleOrLocationRequest> UpdateUserRoleLocationAsync(string procedureName, RoleOrLocationRequest userRoleLocation)
        {           
            // Create dynamic parameters
            var parameters = new DynamicParameters();
            parameters.Add("@UpdateType", userRoleLocation.UpdateType);
            parameters.Add("@UserId", userRoleLocation.UserId); 
            parameters.Add("@Role_User_Id", userRoleLocation.Role_User_Id);
            parameters.Add("@UserMapLocation_Id", userRoleLocation.UserMapLocation_Id);
            
            var result = (await _dbConnection.QueryFirstOrDefaultAsync<RoleOrLocationRequest>(procedureName, parameters, commandType: CommandType.StoredProcedure));
            if (result != null)
            {
                userRoleLocation.StatusMessage = result.ApplicationMessage;
                userRoleLocation.MessageType = result.ApplicationMessageType;
            }
            return userRoleLocation;
          
        }
        #endregion
        #region Added By Harshida 23-12'24

        /// <summary>
        /// IMP NOTE ADDING THESE METHOD AS CONNECTION STRING BECOME NULL
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

        #endregion
        #region User Email already Exist 
        public async Task<(string ApplicationMessage, int ApplicationMessageType)> CheckUserExistAsync(string procedureName, string email)
        {
            using var dbConnection = _dbConnection;
            var parameters = new DynamicParameters();
            parameters.Add("@Email", email, DbType.String, ParameterDirection.Input, 300);

            var result = await dbConnection.QueryFirstOrDefaultAsync<dynamic>(
                procedureName,
                parameters,
                commandType: CommandType.StoredProcedure
            );

            if (result == null)
                throw new Exception("Stored procedure returned null result.");

            return (result.ApplicationMessage, result.ApplicationMessageType);
        }
        #endregion

        public async Task<UserCompanyRoleLocation> GetUserRoleLocationAsync(string procedureName, int userId, int? companyId = null, int? userMapLocationId = null)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@User_Id", userId, DbType.Int32);
            parameters.Add("@Company_id", companyId, DbType.Int32);
            parameters.Add("@Correspondance_Id", DBNull.Value, DbType.Int32);
            parameters.Add("@UserMapLocation_Id", userMapLocationId, DbType.Int32);

            var result = new UserCompanyRoleLocation { UserId = userId };

            var multi = await _dbConnection.QueryMultipleAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);

            // Skip the first result set (locations)
            await multi.ReadAsync<UserLocationDetails>();

            // Read only roles and assign to result
            result.RoleDetails = await multi.ReadAsync<UserRoleDetails>();

            return result;
        }
    }
}
