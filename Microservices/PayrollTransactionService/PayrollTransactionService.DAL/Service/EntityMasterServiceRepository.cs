using Microsoft.Extensions.Configuration;
using PayrollTransactionService.BAL.Models;
using PayrollTransactionService.DAL.Interface;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Payroll.Common.ApplicationConstant;
using Payroll.Common.EnumUtility;
using static Dapper.SqlMapper;
using System.Data.Common;

namespace PayrollTransactionService.DAL.Service
{
    public class EntityMasterServiceRepository : IEntityMasterRepository
    {
        private readonly IDbConnection _dbConnection;
        private readonly IConfiguration _configuration;
        public EntityMasterServiceRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            _dbConnection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }
        public async Task<IEnumerable<ContractorMaster>> GetAllContratcorAsync(string procedureName, object parameters)
        {
            var dynamicParameters = new DynamicParameters(parameters);
            return await _dbConnection.QueryAsync<ContractorMaster>(
                procedureName,
                dynamicParameters,
                commandType: CommandType.StoredProcedure
            );
        }
        public async Task<IEnumerable<EntityDataValidation>> GetAllEntityDataValidationAsync(string procedureName, object parameters)
        {
            var dynamicParameters = new DynamicParameters(parameters);
            return await _dbConnection.QueryAsync<EntityDataValidation>(
                procedureName,
                dynamicParameters,
                commandType: CommandType.StoredProcedure
            );
        }
        public async Task<MapEntityGradeMaster> AddAsync(string procedureName, MapEntityGradeMaster mapGradeEntity)
        {
            var parameters = new DynamicParameters();

            // Prepare UDT DataTable
            var mapGradeEntityDataTable = new DataTable();
            mapGradeEntityDataTable.Columns.Add("Entity_ID", typeof(int));
            mapGradeEntityDataTable.Columns.Add("Pay_Grade_ID", typeof(int)); // string for char(1)
            //mapGradeEntityDataTable.Columns.Add("IsActive", typeof(bool));

            // Populate UDT
            foreach (var entitytaxregime in mapGradeEntity.MapEntityGrade)
            {
                mapGradeEntityDataTable.Rows.Add(
                    entitytaxregime.Entity_ID,
                    entitytaxregime.Pay_Grade_ID
                //entitytaxregime.IsActive
                );
            }

            // Add parameters
            parameters.Add("@EntityPaygradeUDT", mapGradeEntityDataTable.AsTableValuedParameter(DbConstants.UDTMapEntityGrade));
            parameters.Add("@UpdatedBy", mapGradeEntity.UpdatedBy); // Make sure UpdatedBy is included
            parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information);
            parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Update);
            parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.MapGradeEntity);

            try
            {
                var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
                if (result != null)
                {
                    mapGradeEntity.StatusMessage = result.ApplicationMessage;
                    mapGradeEntity.MessageType = result.ApplicationMessageType;
                }
            }
            catch (Exception ex)
            {
                // Optionally log the error
                mapGradeEntity.StatusMessage = "An error occurred while saving data.";
                mapGradeEntity.MessageType = (int)EnumUtility.ApplicationMessageTypeEnum.Error;
                // You could log ex here if needed
            }

            return mapGradeEntity;
        }

        public async Task<IEnumerable<EntityMaster>> GetAllEntityAsync(string procedureName, object parameters)
        {
            var dynamicParameters = new DynamicParameters(parameters);
            return await _dbConnection.QueryAsync<EntityMaster>(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure);

        }
        public async Task<EntityCompliance> EntityComplianceAsync(string procedureName, EntityCompliance entityCompliance)
        {
            var parameters = new DynamicParameters();
            // Add the parameters required for the stored procedure
            parameters.Add("@Entity_ID", entityCompliance.Entity_ID); // 0 for insert, greater than 0 for update
            parameters.Add("@Pf_No", entityCompliance.Pf_No); // 0 for insert, greater than 0 for update
            parameters.Add("@Uan_No", entityCompliance.Uan_No); // 0 for insert, greater than 0 for update
            parameters.Add("@EsicNo", entityCompliance.EsicNo); // 0 for insert, greater than 0 for update
            parameters.Add("@Pf_Applicable", entityCompliance.Pf_Applicable);
            parameters.Add("@Vpf_Applicable", entityCompliance.Vpf_Applicable);
            parameters.Add("@VPF_Percent", entityCompliance.Vpf_percent);
            parameters.Add("@Vpf_Value", entityCompliance.Vpf_Value);
            parameters.Add("@PT_Applicable", entityCompliance.Pt_Applicable);
            parameters.Add("@PT_State_Id", entityCompliance.Pt_State_ID);
            parameters.Add("@Lwf_Applicable", entityCompliance.Lwf_Applicable);
            parameters.Add("@Esi_Exit_Date", entityCompliance.Esi_Exit_Date);
            parameters.Add("@Pay_Grade_Id", entityCompliance.Pay_Grade_Id);
            parameters.Add("@Policy_No", entityCompliance.Policy_No);
            parameters.Add("@PolicyAmt", entityCompliance.PolicyAmt);
            parameters.Add("@GratuityApplicable", entityCompliance.GratuityApplicable);
            parameters.Add("@Pf_Amount", entityCompliance.Pf_Amount);
            parameters.Add("@Pf_Percent", entityCompliance.Pf_Percent);
            parameters.Add("@UpdatedBy", entityCompliance.UpdatedBy);
            // Additional parameters for messages and status using enum values
            parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information); // Cast enum to int
            parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Update); // Cast enum to int
            parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.EntityCompliance); // Cast enum to int
            // Execute stored procedure
            var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
            // Check result for messages and status
            if (result != null)
            {
                entityCompliance.StatusMessage = result.ApplicationMessage;
                entityCompliance.MessageType = result.ApplicationMessageType;
            }

            return entityCompliance;
        }
        public async Task<IEnumerable<WorkOrderMaster>> GetAllWorkorderAsync(string procedureName, object parameters)
        {
            var dynamicParameters = new DynamicParameters(parameters);
            return await _dbConnection.QueryAsync<WorkOrderMaster>(
                procedureName,
                dynamicParameters,
                commandType: CommandType.StoredProcedure
            );
        }
        public async Task<IEnumerable<ContractorWorkOrderRequest>> GetContractorWorkorderAsync(string procedureName, object parameters)
        {
            var dynamicParameters = new DynamicParameters(parameters);
            return await _dbConnection.QueryAsync<ContractorWorkOrderRequest>(
                procedureName,
                dynamicParameters,
                commandType: CommandType.StoredProcedure
            );
        }
    }
}
