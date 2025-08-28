using Dapper;
using Microsoft.AspNetCore.Http;
using Payroll.Common.ApplicationConstant;
using Payroll.Common.ApplicationModel;
using Payroll.Common.CommonDto;
using Payroll.Common.Repository.Interface;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading.Tasks;
using static Payroll.Common.EnumUtility.EnumUtility;

namespace Payroll.Common.Repository.Service
{
    public class RolePermisionRepository : IRolePermmison
    {
        private readonly DapperContext.DapperContext _dbConnection;

        public RolePermisionRepository()
        {
            _dbConnection = DapperContext.DapperContext.GetInstance();
        }
        public async Task<RoleApiPermissionModel> RolePermissionMappingAsync(string RoleName, string ApiEndpoint)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@RoleName", RoleName, DbType.String);
            parameters.Add("@ApiEndpoint", ApiEndpoint, DbType.String);
            using var con = _dbConnection.CreateConnection();
            var rolePermissionModel = await con.QueryFirstOrDefaultAsync<RoleApiPermissionModel>(
                DbConstants.SelectRoleApiPermission,
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return rolePermissionModel;
        }

    }
}
