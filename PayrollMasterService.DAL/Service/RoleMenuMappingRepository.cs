using Dapper;
using Payroll.Common.EnumUtility;
using PayrollMasterService.BAL.Models;
using PayrollMasterService.DAL.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollMasterService.DAL.Service
{
    /// <summary>
    /// Created By:- Harshida Parmar
    /// </summary>
    public class RoleMenuMappingRepository : IRoleMenuMappingRepository
    {
        #region Constructor 
        private readonly IDbConnection _dbConnection;
        public RoleMenuMappingRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
        #endregion
        #region RoleMenuMapping Endpoint Handlers (CRUD)
        #region RoleMenuMapping Add
        /// <summary>
        /// Adds or updates the RoleMenuMappingRequest asynchronously in the database using the specified stored procedure.
        /// </summary>       
        public async Task<RoleMenuMappingRequest> AddAsync(string storedProcedure, RoleMenuMappingRequest roleMenuMapping)
        {
            //using (var connection = _dbConnection)  // _dbConnection is your database connection
            //{
            // Create and populate a DataTable for the RoleMenuDetails
            var roleMenuDetailTable = new DataTable();
            roleMenuDetailTable.Columns.Add("Role_Menu_Dtl_Id", typeof(int));  // Initially 0 for Role_Menu_Hdr_Id
            roleMenuDetailTable.Columns.Add("Menu_Id", typeof(int));
            roleMenuDetailTable.Columns.Add("Company_Id", typeof(int));

            // Populate the DataTable with RoleMenuDetail records
            foreach (var detail in roleMenuMapping.Details)
            {
                roleMenuDetailTable.Rows.Add(
                    0,  // Initially 0, will be updated after the header insert
                    detail.Menu_Id,
                    detail.Company_Id
                );
            }

            // Prepare the parameters for the stored procedure
            var parameters = new DynamicParameters();
            parameters.Add("@RoleMenuDetails", roleMenuDetailTable.AsTableValuedParameter("dbo.RoleMenuDetailType"));
            parameters.Add("@Role_Menu_Hdr_Id", roleMenuMapping.Header.Role_Menu_Hdr_Id); // Pass 0 for the header
            parameters.Add("@Role_Id", roleMenuMapping.Header.Role_Id);
            parameters.Add("@EffectiveFrom", roleMenuMapping.Header.EffectiveFrom);
            // parameters.Add("@Company_Id", roleMenuMapping.Header.Company_Id);            
            parameters.Add("@CreatedBy", roleMenuMapping.Header.CreatedBy);

            parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information); // Cast enum to int
            parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Insert); // Cast enum to int
            parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.RoleMenuMapping); // Cast enum to int

            //try
            //{
            //    var result = await _dbConnection.ExecuteAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure);

            //}
            //catch (Exception ex)
            //{

            //}
            var result = await _dbConnection.QueryFirstOrDefaultAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
            // Check result for messages and status
            if (result != null)
            {
                roleMenuMapping.StatusMessage = result.ApplicationMessage;
                roleMenuMapping.MessageType = result.ApplicationMessageType;
            }
            roleMenuMapping.Header.Role_Menu_Hdr_Id = parameters.Get<int>("@Role_Menu_Hdr_Id");

            return roleMenuMapping;
            //roleMenuMapping.Header.Role_Menu_Hdr_Id = parameters.Get<int>("@Role_Menu_Hdr_Id");

            // return roleMenuMapping;
            // }
        }

        #endregion
        #region RoleMenuMapping Get Data ALL and By ID
        public async Task<IEnumerable<RoleMenuMappingRequest>> GetAllAsync(string procedureName)
        {
            return await _dbConnection.QueryAsync<RoleMenuMappingRequest>(procedureName, commandType: CommandType.StoredProcedure);
        }
        public async Task<RoleMenuMappingRequest> GetByIdAsync(string procedureName, object parameters)
        {
            var dynamicParameters = new DynamicParameters(parameters); // Pass the entire object directly to DynamicParameters
            return await _dbConnection.QuerySingleOrDefaultAsync<RoleMenuMappingRequest>(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure);
        }
        #endregion
        #region RoleMenuMapping Update
        /// <summary>
        /// Updates the Role Menu Detail record asynchronously in the database using the specified stored procedure.
        /// </summary>       
        public async Task<RoleMenuMappingRequest> UpdateAsync(string procedureName, RoleMenuMappingRequest model)
        {
            throw new NotImplementedException();
        }
        #endregion
        #region RoleMenuMapping Delete
        /// <summary>
        /// Deletes the Role Menu Detail record asynchronously in the database using the specified stored procedure.
        /// </summary>       
        public async Task<RoleMenuMappingRequest> DeleteAsync(string procedureName, object roleMenuDetail)
        {
            var parameters = new DynamicParameters();

            // Required parameters for deletion
            parameters.Add("@Role_Menu_Hdr_Id", ((dynamic)roleMenuDetail).Header.Role_Menu_Hdr_Id);
            parameters.Add("@Role_Menu_Dtl_Id", ((dynamic)roleMenuDetail).Details[0].Role_Menu_Dtl_Id);
            parameters.Add("@UpdatedBy", ((dynamic)roleMenuDetail).Header.UpdatedBy);

            // Additional parameters for messages and status using enum values
            parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information); // Cast enum to int
            parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Delete); // Cast enum to int
            parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.RoleMenuMapping); // Cast enum to int

            var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);

            if (result != null)
            {
                ((dynamic)roleMenuDetail).StatusMessage = result.ApplicationMessage;
                ((dynamic)roleMenuDetail).MessageType = result.ApplicationMessageType;
            }
            return (RoleMenuMappingRequest)roleMenuDetail;

        }


        public async Task<IEnumerable<UserRoleMenu>> GetRoleMenuByIdAsync(string procedureName, object parameters)
        {
            var dynamicParameters = new DynamicParameters(parameters); // Pass the entire object directly to DynamicParameters
            return await _dbConnection.QueryAsync<UserRoleMenu>(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure);
        }
        #endregion
        #endregion
    }
}
