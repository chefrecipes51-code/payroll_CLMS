using Dapper;
using PayrollTransactionService.DAL.Interface;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PayrollTransactionService.BAL.ReportModel;
using Payroll.Common.EnumUtility;
using PayrollTransactionService.BAL.Models;
using System.Globalization;


namespace PayrollTransactionService.DAL.Service
{
    public class ContractorValidationServiceRepositor : IContractorValidationRepository
    {
        #region Filter Previous Month Year
        public async Task<CompanyPreviousMonthYearRequest?> GetPreviousMonthYearPeriodByCompanyIdAsync(string procedureName, int companyId)
        {
            var result = await _dbConnection.QueryFirstOrDefaultAsync<CompanyPreviousMonthYearRequest>(
                procedureName,
                new { Company_Id = companyId },
                commandType: CommandType.StoredProcedure);

            // Add month name conversion
            if (result != null)
            {
                result.MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(result.Month_Id);
            }

            return result;
        }
        #endregion

        #region Constructor 
        private readonly IDbConnection _dbConnection;
        public ContractorValidationServiceRepositor(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
        public async Task<IEnumerable<ContractorValidationRequest>> GetContractorValidationAsync(
                      string procedureName,
                      byte companyId,
                      List<int>? locationIds,
                      //List<int>? contractorIds,
                      List<int>? workOrderIds)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@CompanyId", companyId, DbType.Byte);

            // Convert List<int> to DataTable
            parameters.Add("@LocationList", locationIds.ToDataTable("Location_ID"), DbType.Object);
            //parameters.Add("@ContractorIds", contractorIds.ToDataTable("Contractor_ID"), DbType.Object);
            parameters.Add("@WorkOrderIds", workOrderIds.ToDataTable("WorkOrder_ID"), DbType.Object);

            var result = await _dbConnection.QueryAsync<ContractorValidationRequest>(
                procedureName,
                parameters,
                commandType: CommandType.StoredProcedure);

            return result;
        }

        public async Task<ValidateContractorRequest> UpdateValidateContractorsAsync(string procedureName, ValidateContractorRequest request)
        {
            var contractorIdTable = new DataTable();
            contractorIdTable.Columns.Add("Contractor_ID", typeof(int));
            foreach (var id in request.ContractorIds)
            {
                contractorIdTable.Rows.Add(id);
            }
            var parameters = new DynamicParameters();
            parameters.Add("@CompanyId", request.CompanyId);
            parameters.Add("@ContractorIds", contractorIdTable.AsTableValuedParameter("ContractorIdList"));
            parameters.Add("@UpdatedBy", request.UpdatedBy);
            try
            {
                var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
                if (result != null)
                {
                    request.StatusMessage = result.ApplicationMessage;
                    request.MessageType = result.ApplicationMessageType;
                }
                else
                {
                    request.StatusMessage = "No message returned from the procedure.";
                    request.MessageType = 0;
                }
            }
            catch (Exception ex)
            {
                request.StatusMessage = $"Exception occurred: {ex.Message}";
                request.MessageType = 0;
            }

            return request;
        }
        #endregion

        #region Entity
        public async Task<IEnumerable<EntityValidationRequest>> GetEntityValidationAsync(string procedureName, byte companyId, List<int> contractorIds)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@CompanyId", companyId, DbType.Byte);
            var contractorTable = new DataTable();
            contractorTable.Columns.Add("Contractor_ID", typeof(int));
            foreach (var id in contractorIds)
            {
                contractorTable.Rows.Add(id);
            }
            parameters.Add("@ContractorIds", contractorTable.AsTableValuedParameter("ContractorIdList"));

            var result = await _dbConnection.QueryAsync<EntityValidationRequest>(
                procedureName,
                parameters,
                commandType: CommandType.StoredProcedure);

            return result;
        }

        public async Task<EntityUpdateRequest> UpdateValidateEntitiesAsync(string procedureName, EntityUpdateRequest request)
        {
            // 1. Convert List<EntityUpdateItem> to DataTable
            var entityUpdateTable = new DataTable();
            entityUpdateTable.Columns.Add("Entity_ID", typeof(int));
            entityUpdateTable.Columns.Add("GradeConfigName", typeof(int));

            foreach (var item in request.EntityUpdateList)
            {
                var gradeConfigValue = item.GradeConfigName.HasValue ? (object)item.GradeConfigName.Value : DBNull.Value;
                entityUpdateTable.Rows.Add(item.Entity_ID, gradeConfigValue);
            }

            // 2. Create parameters
            var parameters = new DynamicParameters();
            parameters.Add("@CompanyId", request.CompanyId);
            parameters.Add("@EntityUpdateList", entityUpdateTable.AsTableValuedParameter("EntityUpdateList"));
            parameters.Add("@UpdatedBy", request.UpdatedBy);

            try
            {
                // 3. Call stored procedure
                var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);

                if (result != null)
                {
                    request.StatusMessage = result.ApplicationMessage;
                    request.MessageType = result.ApplicationMessageType;
                }
                else
                {
                    request.StatusMessage = "No response returned from stored procedure.";
                    request.MessageType = 0;
                }
            }
            catch (Exception ex)
            {
                request.StatusMessage = $"Exception occurred: {ex.Message}";
                request.MessageType = 0;
            }

            return request;
        }
        #endregion

        #region Pay Calculation
        public async Task<IEnumerable<EntityPayValidationRequest>> GetEntityPayValidationAsync(string procedureName, byte companyId, List<int> entityIds)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@CompanyId", companyId, DbType.Byte);

            // Create DataTable for TVP
            var entityTable = new DataTable();
            entityTable.Columns.Add("Entity_ID", typeof(int));

            foreach (var id in entityIds)
            {
                entityTable.Rows.Add(id);
            }

            parameters.Add("@EntityIds", entityTable.AsTableValuedParameter("EntityIdList"));

            // Execute stored procedure
            var result = await _dbConnection.QueryAsync<EntityPayValidationRequest>(
                procedureName,
                parameters,
                commandType: CommandType.StoredProcedure);

            return result;
        }
        public async Task<PayCalculationUpdateRequest> UpdateValidatePayCalcAsync(string procedureName, PayCalculationUpdateRequest request)
        {
            var entityStructureTable = new DataTable();
            entityStructureTable.Columns.Add("Entity_ID", typeof(int));
            entityStructureTable.Columns.Add("SalaryStructureId", typeof(int));

            foreach (var item in request.EntityStructureUpdateList)
            {
                var salaryStructureValue = item.SalaryStructureId.HasValue ? (object)item.SalaryStructureId.Value : DBNull.Value;
                entityStructureTable.Rows.Add(item.Entity_ID, salaryStructureValue);
            }

            // 2. Create parameters
            var parameters = new DynamicParameters();
            parameters.Add("@CompanyId", request.CompanyId);
            parameters.Add("@EntityStructureUpdateList", entityStructureTable.AsTableValuedParameter("dbo.EntityStructureUpdateList"));
            parameters.Add("@UpdatedBy", request.UpdatedBy);

            try
            {
                // 3. Call stored procedure
                var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);

                if (result != null)
                {
                    request.StatusMessage = result.ApplicationMessage;
                    request.MessageType = result.ApplicationMessageType;
                }
                else
                {
                    request.StatusMessage = "No response returned from stored procedure.";
                    request.MessageType = 0;
                }
            }
            catch (Exception ex)
            {
                request.StatusMessage = $"Exception occurred: {ex.Message}";
                request.MessageType = 0;
            }

            return request;
        }

        #endregion

        #region Compliance
        public async Task<IEnumerable<EntityComplianceValidationRequest>> GetEntityComplianceValidationAsync(string procedureName, byte companyId, List<int> entityIds)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@CompanyId", companyId, DbType.Byte);

            // Prepare the TVP
            var entityTable = new DataTable();
            entityTable.Columns.Add("Entity_ID", typeof(int));

            foreach (var id in entityIds)
            {
                entityTable.Rows.Add(id);
            }

            parameters.Add("@EntityIds", entityTable.AsTableValuedParameter("EntityIdList"));

            var result = await _dbConnection.QueryAsync<EntityComplianceValidationRequest>(
                procedureName,
                parameters,
                commandType: CommandType.StoredProcedure);

            return result;
        }
        public async Task<EntityComplianceUpdateRequest> UpdateValidateComplianceAsync(string procedureName, EntityComplianceUpdateRequest request)
        {
            var table = new DataTable();
            table.Columns.Add("Entity_ID", typeof(int));
            table.Columns.Add("PFNo", typeof(string));
            table.Columns.Add("PFAmount", typeof(decimal));
            table.Columns.Add("PFPercent", typeof(decimal));
            table.Columns.Add("VPFValue", typeof(decimal));
            table.Columns.Add("VPFPercent", typeof(decimal));
            table.Columns.Add("UANNo", typeof(string));
            table.Columns.Add("ESICNo", typeof(string));

            foreach (var item in request.EntityComplianceUpdateList)
            {
                table.Rows.Add(
                    item.Entity_ID,
                    string.IsNullOrWhiteSpace(item.PFNo) ? DBNull.Value : item.PFNo,
                    item.PFAmount ?? (object)DBNull.Value,
                    item.PFPercent ?? (object)DBNull.Value,
                    item.VPFValue ?? (object)DBNull.Value,
                    item.VPFPercent ?? (object)DBNull.Value,
                    string.IsNullOrWhiteSpace(item.UANNo) ? DBNull.Value : item.UANNo,
                    string.IsNullOrWhiteSpace(item.ESICNo) ? DBNull.Value : item.ESICNo
                );
            }

            var parameters = new DynamicParameters();
            parameters.Add("@CompanyId", request.CompanyId);
            parameters.Add("@EntityComplianceUpdateList", table.AsTableValuedParameter("EntityComplianceUpdateList"));
            parameters.Add("@UpdatedBy", request.UpdatedBy);

            try
            {
                var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);

                request.StatusMessage = result?.ApplicationMessage ?? "No response";
                request.MessageType = result?.ApplicationMessageType ?? 0;
            }
            catch (Exception ex)
            {
                request.StatusMessage = $"Exception: {ex.Message}";
                request.MessageType = 0;
            }
            return request;
        }
        #endregion

        #region Attendance
        public async Task<IEnumerable<EntityAttendanceRequest>> GetEntityAttendanceValidationAsync(string procedureName, byte companyId, List<int> entityIds,
            int payrollMonth,    
            int payrollYear)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@CompanyId", companyId, DbType.Byte);
            parameters.Add("@Payroll_Month", payrollMonth, DbType.Int32);
            parameters.Add("@Payroll_Year", payrollYear, DbType.Int32);  
            var entityTable = new DataTable();
            entityTable.Columns.Add("Entity_ID", typeof(int));
            foreach (var id in entityIds)
            {
                entityTable.Rows.Add(id);
            }

            parameters.Add("@EntityIds", entityTable.AsTableValuedParameter("EntityIdList"));

            var result = await _dbConnection.QueryAsync<EntityAttendanceRequest>(
                procedureName,
                parameters,
                commandType: CommandType.StoredProcedure);

            return result;
        }
        public async Task<EntityAttendanceUpdateRequest> UpdateValidateAttendanceAsync(string procedureName, EntityAttendanceUpdateRequest request)
        {
            var table = new DataTable();
            table.Columns.Add("Entity_ID", typeof(int));
            foreach (var id in request.EntityAttendanceIds)
            {
                table.Rows.Add(id);
            }
            var parameters = new DynamicParameters();
            parameters.Add("@CompanyId", request.CompanyId, DbType.Int32);
            parameters.Add("@EntityAttendanceIds", table.AsTableValuedParameter("EntityAttendanceIds"));
            parameters.Add("@UpdatedBy", request.UpdatedBy, DbType.Int32);
            try
            {
                var result = await _dbConnection.QueryFirstOrDefaultAsync(
                    procedureName,
                    parameters,
                    commandType: CommandType.StoredProcedure
                );
                request.StatusMessage = result?.ApplicationMessage ?? "No response from procedure.";
                request.MessageType = result?.ApplicationMessageType ?? 0;
            }
            catch (Exception ex)
            {
                request.StatusMessage = $"Exception: {ex.Message}";
                request.MessageType = 0;
            }
            return request;
        }

        #endregion
    }
    public static class ListExtensions
    {
        public static DataTable ToDataTable(this List<int>? list, string columnName)
        {
            var dt = new DataTable();
            dt.Columns.Add(columnName, typeof(int));

            if (list != null)
            {
                foreach (var item in list)
                    dt.Rows.Add(item);
            }
            return dt;
        }
    }

}
