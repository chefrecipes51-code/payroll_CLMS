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
using Payroll.Common.EnumUtility;

namespace PayrollTransactionService.DAL.Service
{
    public class PayrollProcessRepositoryService : IPayrollProcessRepository
    {
        private readonly IDbConnection _dbConnection;
        private readonly IConfiguration _configuration;
        public PayrollProcessRepositoryService(IConfiguration configuration)
        {
            _configuration = configuration;
            _dbConnection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }

        public async Task<IEnumerable<CompanyPayrollValidation>> GetAllCompanyPayrollValidationAsync(string procedureName, object parameters)
        {
            var dynamicParameters = new DynamicParameters(parameters);

            return await _dbConnection.QueryAsync<CompanyPayrollValidation>(
                procedureName,
                dynamicParameters,
                commandType: CommandType.StoredProcedure
            );
        }
        public async Task<IEnumerable<ContractorPayrollValidation>> GetAllContractorPayrollValidationAsync(string procedureName, object parameters)
        {
            var dynamicParameters = new DynamicParameters(parameters);

            return await _dbConnection.QueryAsync<ContractorPayrollValidation>(
                procedureName,
                dynamicParameters,
                commandType: CommandType.StoredProcedure
            );
        }
        public async Task<IEnumerable<CompanyLocationPayrollValidation>> GetAllCompanyLocationPayrollValidationAsync(string procedureName, object parameters)
        {
            var dynamicParameters = new DynamicParameters(parameters);

            return await _dbConnection.QueryAsync<CompanyLocationPayrollValidation>(
                procedureName,
                dynamicParameters,
                commandType: CommandType.StoredProcedure
            );
        }
        public async Task<IEnumerable<WorkOrderPayrollValidation>> GetAllWorkOrderPayrollValidationAsync(string procedureName, object parameters)
        {
            var dynamicParameters = new DynamicParameters(parameters);

            return await _dbConnection.QueryAsync<WorkOrderPayrollValidation>(
                procedureName,
                dynamicParameters,
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<IEnumerable<PreviousMonthYearPeriod>> GetAllPreviousMonthYearPeriodAsync(string procedureName, object parameters)
        {
            var dynamicParameters = new DynamicParameters(parameters);

            return await _dbConnection.QueryAsync<PreviousMonthYearPeriod>(
                procedureName,
                dynamicParameters,
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<PayrollTransDataForProcess> AddPayrollTransDataForProcessAsync(string procedureName, PayrollTransDataForProcess model)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Company_ID", model.Company_ID);
                parameters.Add("@Month_ID", model.Month_ID);
                parameters.Add("@Year_ID", model.Year_ID);
                parameters.Add("@LocationIDs", model.LocationIDs);
                parameters.Add("@ContractorIDs", model.ContractorIDs);
                parameters.Add("@WorkOrderIDs", model.WorkOrderIDs);
                parameters.Add("@Updated_By", model.Updated_By);

                // Execute stored procedure and capture message output
                var result = await _dbConnection.QueryFirstOrDefaultAsync<dynamic>(
                    procedureName, parameters, commandType: CommandType.StoredProcedure);

                if (result != null)
                {
                    model.StatusMessage = result.ApplicationMessage;
                    model.MessageType = result.ApplicationMessageType;
                    model.UpdatedRecords = result.UpdatedRecords;
                }
                else
                {
                    model.StatusMessage = "No response from procedure.";
                    model.MessageType = 3; // Treat as error
                }
            }
            catch (Exception ex)
            {
                model.StatusMessage = ex.Message;
            }

            return model;
        }

        public async Task<StartPayrollProcess> AddStartPayrollProcessAsync(string procedureName, StartPayrollProcess model)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Cmp_Id", model.Cmp_Id);
                parameters.Add("@Month_Id", model.Month_Id);
                parameters.Add("@Year_Id", model.Year_Id);
                parameters.Add("@CreatedBy", model.CreatedBy);
                parameters.Add("@ProcessSequence_Id", model.Process_Sequence_Id);
                parameters.Add("@PayrollProcessId", model.Payroll_Process_Id, DbType.Int32);
                
                // Execute the stored procedure synchronously (waits until full payroll is processed)
                var result = await _dbConnection.QueryFirstOrDefaultAsync<StartPayrollProcess>(
                   procedureName,
                   parameters,
                   commandType: CommandType.StoredProcedure,
                   commandTimeout: 600 // 10 minutes increase timeout
               );
                // Check result for messages and status
                if (result != null)
                {
                    model.StatusMessage = "sucess";
                    model.MessageType = 1;
                }

                return result;
            }
            catch (Exception ex)
            {
                model.MessageType = -1;
                model.StatusMessage = "Error occurred: " + ex.Message;
                throw;
            }
        }

        public async Task<PayrollProcessResultModel> ProcessPayrollEmployeesAsync(string procedureName, PayrollProcessRequestModel model)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Payroll_Process_Id", model.PayrollProcessId);
            parameters.Add("@MonthId", model.MonthId);
            parameters.Add("@YearId", model.YearId);
            parameters.Add("@CreatedBy", model.CreatedBy);

            try
            {
                var result = await _dbConnection.QueryAsync<PayrollProcessOutputModel>(
                    procedureName,
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return new PayrollProcessResultModel
                {
                    PayrollProcessId = model.PayrollProcessId,
                    Headers = result.ToList(),
                    IsSuccess = true,
                    Message = "Payroll processed successfully."
                };
            }
            catch (Exception ex)
            {
                // Optional: Log exception
                return new PayrollProcessResultModel
                {
                    PayrollProcessId = model.PayrollProcessId,
                    IsSuccess = false,
                    Message = $"Error during payroll processing: {ex.Message}"
                };
            }
        }

        public async Task<IEnumerable<PayrollProcessusingSignalR>> GetAllPayrollProcessusingSignalRAsync(string procedureName, object parameters)
        {
            var dynamicParameters = new DynamicParameters(parameters);

            return await _dbConnection.QueryAsync<PayrollProcessusingSignalR>(
                procedureName,
                dynamicParameters,
                commandType: CommandType.StoredProcedure
            );
        }
        public async Task<IEnumerable<dynamic>> GetPhaseWiseComponentPayrollProcessAsync(string procedureName, PhaseWiseComponentPayrollProcess model)
        {
            try
            {
                // Assuming wageGradeDetail is passed as the object
                var parameters = new DynamicParameters();
                parameters.Add("@Company_Id", model.Company_Id);
                parameters.Add("@Payroll_Process_Id", model.Payroll_Process_Id);
                parameters.Add("@Payroll_Header_Id", model.Payroll_Header_Id);
                parameters.Add("@MonthId", model.MonthId);
                parameters.Add("@YearId", model.YearId);
                parameters.Add("@Process_Sequence_Id", model.Process_Sequence_Id);
                parameters.Add("@Payroll_Run_Type", model.Payroll_Run_Type);

                var result = await _dbConnection.QueryAsync<dynamic>(
                    procedureName,
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return result;
            }
            catch (Exception ex)
            {
                // Log the exception or throw as needed
                throw new Exception("Error executing GetPhaseWisePayrollProcessAsync", ex);
            }
        }


    }
}
