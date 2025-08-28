using Dapper;
using Microsoft.Extensions.Configuration;
using Payroll.Common.EnumUtility;
using Payroll.Common.ViewModels;
using PayrollMasterService.BAL.Models;
using PayrollMasterService.BAL.Requests;
using PayrollMasterService.DAL.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace PayrollMasterService.DAL.Service
{
    public class ApprovalSetUpRepository : IApprovalSetUpRepository
    {
        private readonly IDbConnection _dbConnection;
        private readonly IConfiguration _configuration;
        public ApprovalSetUpRepository(IConfiguration configuration )
        {
            _configuration = configuration;
            _dbConnection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }

        #region ApprovalSetUp Endpoint Handlers (CRUD)
        #region ApprovalSetUp Add


        public async Task<ApprovalConfigCommon> AddApprovalConfigAsync(string storedProcedure, ApprovalConfigCommon approvalConfigCommon)
        {
            using (var connection = _dbConnection)
            {
                var levelTable = new DataTable();
                levelTable.Columns.Add("LevelID", typeof(int));
                levelTable.Columns.Add("ConfigID", typeof(int));
                levelTable.Columns.Add("LevelNumber", typeof(int));
                levelTable.Columns.Add("ApprovalType", typeof(string));
                levelTable.Columns.Add("isApproveByAll", typeof(bool));
                levelTable.Columns.Add("ApproveByAnyCount", typeof(int));

                var detailTable = new DataTable();
                detailTable.Columns.Add("ApprovalID", typeof(int));         
                detailTable.Columns.Add("ConfigID", typeof(int));
                detailTable.Columns.Add("LevelID", typeof(int));
                detailTable.Columns.Add("UserID", typeof(int));
                detailTable.Columns.Add("SequenceOrder", typeof(int));
                detailTable.Columns.Add("IsAlternate", typeof(bool));

                // Fill level and detail tables
                foreach (var level in approvalConfigCommon.Levels)
                {
                    levelTable.Rows.Add(
                        level.LevelID,
                        approvalConfigCommon.Config.ConfigID,
                        level.LevelNumber,
                        level.ApprovalType,
                        level.IsApproveByAll,
                        level.ApproveByAnyCount ?? (object)DBNull.Value
                    );
                }

                foreach (var detail in approvalConfigCommon.Details)
                {
                    detailTable.Rows.Add(
                        detail.ApprovalID,
                        approvalConfigCommon.Config.ConfigID,
                        detail.LevelID, // LevelID will be set in SP after insert
                        detail.UserID,
                        detail.SequenceOrder,
                        detail.IsAlternate
                    );
                }

                var parameters = new DynamicParameters();

                // Main table params
                // parameters.Add("@ConfigID", approvalConfigCommon.ConfigConfigID, DbType.Int32, ParameterDirection.InputOutput);
                parameters.Add("@ConfigID", approvalConfigCommon.Config.ConfigID);
                parameters.Add("@CompanyId", approvalConfigCommon.Config.CompanyId);
                parameters.Add("@ServiceID", approvalConfigCommon.Config.ServiceID);
                parameters.Add("@LocationId", approvalConfigCommon.Config.LocationId);
                parameters.Add("@ApprovalProcessName", approvalConfigCommon.Config.ApprovalProcessName);
                parameters.Add("@TotalLevels", approvalConfigCommon.Config.TotalLevels);
                parameters.Add("@Priority", approvalConfigCommon.Config.Priority);
                parameters.Add("@ApproveType", approvalConfigCommon.Config.ApproveType);
                parameters.Add("@NoOfDays", approvalConfigCommon.Config.NoOfDays);
                parameters.Add("@IsEmailAlert", approvalConfigCommon.Config.IsEmailAlert);
                parameters.Add("@IsNotificationEnabled", approvalConfigCommon.Config.IsNotificationEnabled);
                parameters.Add("@IsActive", approvalConfigCommon.Config.IsActive);
                parameters.Add("@IsDeleted", approvalConfigCommon.Config.IsDeleted);
                parameters.Add("@CreatedBy", approvalConfigCommon.Config.CreatedBy);
                parameters.Add("@UpdatedBy", approvalConfigCommon.Config.UpdatedBy);
                parameters.Add("@EffectiveDate", approvalConfigCommon.Config.EffectiveDate);
                // TVPs
                parameters.Add("@LevelData", levelTable.AsTableValuedParameter("dbo.udt_approval_conf_levels"));
                parameters.Add("@DetailData", detailTable.AsTableValuedParameter("dbo.udt_approval_conf_details"));

                // System messages
                parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information);
                parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Insert);
                parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.ApprovalSetUp);

                try
                {
                    await connection.ExecuteAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure);

                    // Retrieve new ConfigID if inserted
                    //approvalConfig.ConfigID = parameters.Get<int>("@ConfigID");
                }
                catch (Exception ex)
                {
                    // Log or handle the exception
                    throw new Exception("Failed to save approval config", ex);
                }

                return approvalConfigCommon;
            }
        }


        public async Task<IEnumerable<ApprovalConfigGrid>> GetApprovalConfigGridAsync(string procedureName, int? companyId, int? locationId, int? serviceId)
        {
            using var dbConnection = _dbConnection; // Assuming `_dbConnection` is available
            var dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("@CompanyId", companyId);
            dynamicParameters.Add("@LocationId", locationId);
            dynamicParameters.Add("@ServiceId", serviceId);

            try
            {
                var approvalConfigGrid = await dbConnection.QueryAsync<ApprovalConfigGrid>(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure);

                return (IEnumerable<ApprovalConfigGrid>)approvalConfigGrid;
            }
            catch (Exception ex)
            {

                throw;
            }
           
        }

        public async Task<(ApprovalConfig, IEnumerable<ApprovalLevel>, IEnumerable<ApprovalDetail>)> GetApprovalConfigDetailsAsync(string procedureName, int configId)
        {
            using var dbConnection = _dbConnection; // Assuming `_dbConnection` is available
            var dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("@ConfigId", configId);


            using var multi = await dbConnection.QueryMultipleAsync(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure);


            var approvalConfig = new ApprovalConfig();
            // 1️ Main Config
            approvalConfig = await multi.ReadFirstOrDefaultAsync<ApprovalConfig>();
            //if (approvalConfig == null)
            //    return 1;

            // 2️ Levels
            var approvalLevel = (await multi.ReadAsync<ApprovalLevel>()).ToList();

            // 3️ Details
            var approvalDetail = (await multi.ReadAsync<ApprovalDetail>()).ToList();

            //  Map Details into Levels
            //foreach (var level in approvalLevel)
            //{
            //    level.Details = approvalDetail.Where(d => d.LevelID == level.LevelID).ToList();
                
            //}
            //approvalConfig.Levels = approvalLevel;



            return (approvalConfig, approvalLevel,approvalDetail);
        }

        public async Task<ApprovalSetUp> AddNewAsync(string storedProcedure, ApprovalSetUp approvalSetUp)
        {
            using (var connection = _dbConnection)  // _dbConnection is your database connection
            {
                // Create and populate a DataTable for the Approval SetUp
                var approvalSetUpDetailsTable = new DataTable();
                approvalSetUpDetailsTable.Columns.Add("ServiceID", typeof(int));
                approvalSetUpDetailsTable.Columns.Add("LevelNumber", typeof(int));
                approvalSetUpDetailsTable.Columns.Add("UserID", typeof(int));
                approvalSetUpDetailsTable.Columns.Add("SequenceOrder", typeof(int));
                approvalSetUpDetailsTable.Columns.Add("IsAlternate", typeof(bool));

                // Populate the DataTable with Approval Setup records
                foreach (var detail in approvalSetUp.approvalSetUpDetails)
                {
                    approvalSetUpDetailsTable.Rows.Add(
                        detail.ServiceID,
                        detail.LevelNumber,
                        detail.UserID,
                        detail.SequenceOrder,
                        detail.IsAlternate
                    );
                }


                // Stored Procedure parameters.
                var parameters = new DynamicParameters();
                parameters.Add("@ApprovalSetUp", approvalSetUpDetailsTable.AsTableValuedParameter("dbo.udt_conf_approvalsetup"));
                parameters.Add("@CreatedBy", approvalSetUp.CreatedBy);
                parameters.Add("@UpdatedBy", approvalSetUp.UpdatedBy);
                // parameters.Add("@CountryId", approvalSetUp.CountryId);

                parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information); // Cast enum to int
                parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Insert); // Cast enum to int
                parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.ApprovalSetUp); // Cast enum to int

                try
                {
                    var result = await connection.ExecuteAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
                }
                catch (Exception ex)
                {
                }
                return approvalSetUp;
            }
        }
        public async Task<ApprovalConfigGrid> GetByIdAsync(string procedureName, object parameters)
        {
            var dynamicParameters = new DynamicParameters(parameters); // Pass the entire object directly to DynamicParameters
            return await _dbConnection.QuerySingleOrDefaultAsync<ApprovalConfigGrid>(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure);
        }
        public async Task<IEnumerable<ApprovalSetUpFilter>> GetByFilterAttributesAsync(string procedureName, object parameters)
        {
            try
            {
                var dynamicParameters = new DynamicParameters(parameters); // Pass the entire object directly to DynamicParameters
                return await _dbConnection.QueryAsync<ApprovalSetUpFilter>(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure);

            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public Task<ApprovalConfigGrid> AddAsync(string procedureName, ApprovalConfigGrid model)
        {
            throw new NotImplementedException();
        }
        #endregion
      
        public async Task<ApprovalConfigGrid> DeleteAsync(string procedureName, object approvalConfig)
        {
            var parameters = new DynamicParameters();
            // Assuming wageGradeDetail is passed as the object
            parameters.Add("@ConfigID", ((dynamic)approvalConfig).@ConfigID); // 0 for insert, greater than 0 for update
            parameters.Add("@UpdatedBy", ((dynamic)approvalConfig).UpdatedBy);
            // Additional parameters for messages and status using enum values
            parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information); // Cast enum to int
            parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Delete); // Cast enum to int
            parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.ApprovalSetUp); // Cast enum to int
            // Execute stored procedure
            var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
            // Check result for messages and status
            if (result != null)
            {
                ((dynamic)approvalConfig).StatusMessage = result.ApplicationMessage;
                ((dynamic)approvalConfig).MessageType = result.ApplicationMessageType;
            }
            return (ApprovalConfigGrid)approvalConfig;
        }
        public Task<ApprovalConfigGrid> UpdateAsync(string procedureName, ApprovalConfigGrid model)
        {
            throw new NotImplementedException();
        }
        Task<IEnumerable<ApprovalConfigGrid>> IGenericRepository<ApprovalConfigGrid>.GetAllAsync(string procedureName)
        {
            throw new NotImplementedException();
        }
        #endregion

        //public async Task<IEnumerable<ApprovalDetailRequest>> GetListAsync(string procedureName, object parameters)
        //{
        //    var dynamicParameters = new DynamicParameters(parameters);
        //    return await _dbConnection.QueryAsync<ApprovalDetailRequest>(
        //        procedureName,
        //        dynamicParameters,
        //        commandType: CommandType.StoredProcedure
        //    );
        //}
        public async Task<ApprovalListViewModel> GetApprovalWithSummaryAsync(string procedureName, object parameters)
        {
            var dynamicParameters = new DynamicParameters(parameters);

            using var multi = await _dbConnection.QueryMultipleAsync(
                procedureName,
                dynamicParameters,
                commandType: CommandType.StoredProcedure
            );

            var approvalList = (await multi.ReadAsync<ApprovalDetailRequest>()).ToList();
            var summary = await multi.ReadFirstOrDefaultAsync<ApprovalSummaryCounts>();

            return new ApprovalListViewModel
            {
                ApprovalList = approvalList,
                SummaryCounts = summary ?? new ApprovalSummaryCounts()
            };
        }

        public async Task<ApprovalDetailRequest> UpdateApprovalAsync(string procedureName, ApprovalDetailRequest model)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Approval_ID", model.Approval_ID);
            parameters.Add("@Approver_ID", model.Approver_ID);
            parameters.Add("@Approval_Hdr_Id", model.Approval_Hdr_Id);
            parameters.Add("@Approval_Status", model.Approval_Status);          
            parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information);
            parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Update); 
            parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.ApprovalStatus);
            var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
            if (result != null)
            {
                model.StatusMessage = result.ApplicationMessage;
                model.MessageType = result.ApplicationMessageType;
            }
            return model;
        }

    }
}
