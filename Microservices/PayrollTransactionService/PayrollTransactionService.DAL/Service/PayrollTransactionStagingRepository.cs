using Dapper;
using Payroll.Common.EnumUtility;
using PayrollTransactionService.BAL.ReportModel;
using PayrollTransactionService.DAL.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollTransactionService.DAL.Service
{
    public class PayrollTransactionStagingRepository : IPayrollTransactionStagingRepository
    {
        private readonly IDbConnection _dbConnection;
        public PayrollTransactionStagingRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
        public async Task<PayrollStgData> SavePayrollStagingDataAsync(
            string procedureName,
            SavePayrollStagingRequestModel request)
        {
            var response = new PayrollStgData();

            var table = new DataTable();
            table.Columns.Add("Contractor_ID", typeof(int));
            table.Columns.Add("Entity_ID", typeof(int));

            foreach (var item in request.PayrollData)
            {
                table.Rows.Add(item.Contractor_ID ?? (object)DBNull.Value, item.Entity_ID);
            }

            var parameters = new DynamicParameters();
            parameters.Add("@PayrollData", table.AsTableValuedParameter("dbo.udt_tbl_trn_payroll_stg_data"));
            parameters.Add("@CreatedBy", request.CreatedBy, DbType.Int32);
            parameters.Add("@Month_Id", request.Month_Id, DbType.Int32);
            parameters.Add("@Year_Id", request.Year_Id, DbType.Int32);
            parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information);
            parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Insert);
            parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.PayrollTranSTGData);

            var result = await _dbConnection.QueryFirstOrDefaultAsync(
                procedureName,
                parameters,
                commandType: CommandType.StoredProcedure
            );

            if (result != null)
            {
                response.StatusMessage = result.ApplicationMessage;
                response.MessageType = result.ApplicationMessageType;
            }
            else
            {
                response.StatusMessage = "No response from procedure.";
                response.MessageType = 0;
            }

            return response;
        }
        public async Task<PayrollTranStgDataRequest> AddAsync(string procedureName, PayrollTranStgDataRequest model)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Payroll_Process_No", model.Payroll_Process_No);
            parameters.Add("@Entity_ID", model.Entity_ID);
            parameters.Add("@Payroll_Month", model.Payroll_Month);
            parameters.Add("@Attendance_Month", model.Attendance_Month);
            parameters.Add("@Days_Present", model.Days_Present);
            parameters.Add("@Weekly_Offs", model.Weekly_Offs);
            parameters.Add("@Holidays", model.Holidays);
            parameters.Add("@Paid_Leaves", model.Paid_Leaves);
            parameters.Add("@Unpaid_Leaves", model.Unpaid_Leaves);
            parameters.Add("@Total_Working_Days", model.Total_Working_Days);
            parameters.Add("@Total_Ot_Hrs", model.Total_Ot_Hrs);
            parameters.Add("@Created_By", model.CreatedBy);
            parameters.Add("@ProcessStage", model.ProcessStage);
            parameters.Add("@Process_Sequence_ID", model.Process_Sequence_ID);
            parameters.Add("@IsLocked", model.IsLocked);
            parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information); // Cast enum to int
            parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Insert); // Cast enum to int
            parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.PayrollTranSTGData); // Cast enum to int
            var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
            if (result != null)
            {
                model.StatusMessage = result.ApplicationMessage;
                model.MessageType = ((dynamic)result).ApplicationMessageType;
            }
            return model;
        }
        public Task<PayrollTranStgDataRequest> DeleteAsync(string procedureName, object parameters)
        {
            throw new NotImplementedException();
        }
        public Task<IEnumerable<PayrollTranStgDataRequest>> GetAllAsync(string procedureName)
        {
            throw new NotImplementedException();
        }
        public Task<PayrollTranStgDataRequest> GetByIdAsync(string procedureName, object parameters)
        {
            throw new NotImplementedException();
        }
        public Task<PayrollTranStgDataRequest> UpdateAsync(string procedureName, PayrollTranStgDataRequest model)
        {
            throw new NotImplementedException();
        }
    }
}
