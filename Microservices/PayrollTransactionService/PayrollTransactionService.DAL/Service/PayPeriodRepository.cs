using Dapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
    public class PayPeriodRepository : IPayPeriodRepository
    {
        #region Constructor 
        private readonly IDbConnection _dbConnection;
        public PayPeriodRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
        #endregion

        public async Task<IEnumerable<PeriodRequest>> GetAllAsync(string procedureName)
        {
            return await _dbConnection.QueryAsync<PeriodRequest>(procedureName, commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<PeriodRequest>> GetPayPeriodsByCompanyIdAsync(string procedureName, object parameters)
        {
            return await _dbConnection.QueryAsync<PeriodRequest>(procedureName, parameters, commandType: CommandType.StoredProcedure);
        }
        //public async Task<IEnumerable<PeriodRequest>> GetPayPeriodsByCompanyIdAndSDateAsync(string procedureName, object parameters)
        //{
        //    var result = await _dbConnection.QueryFirstOrDefaultAsync<string>(
        //        sql: procedureName,
        //        param: parameters,
        //        commandType: CommandType.StoredProcedure);

        //    //if (!string.IsNullOrEmpty(result))
        //    //{
        //    //    var deserialized = JsonConvert.DeserializeObject<PeriodResponseWrapper>(result);
        //    //    return deserialized?.Periods ?? new List<PeriodRequest>();
        //    //}

        //    //return new List<PeriodRequest>();
        //    if (string.IsNullOrWhiteSpace(result))
        //        return new List<PeriodRequest>();

        //    if (IsValidJson(result))
        //    {
        //        var deserialized = JsonConvert.DeserializeObject<PeriodResponseWrapper>(result);
        //        return deserialized?.Periods ?? new List<PeriodRequest>();
        //    }
        //    else
        //    {
        //        return new List<PeriodRequest>();
        //    }
        //}
        public async Task<(IEnumerable<PeriodRequest> Periods, string ErrorMessage)> GetPayPeriodsByCompanyIdAndSDateAsync(string procedureName, object parameters)
        {
            var result = await _dbConnection.QueryFirstOrDefaultAsync<string>(
                sql: procedureName,
                param: parameters,
                commandType: CommandType.StoredProcedure);

            if (string.IsNullOrWhiteSpace(result))
                return (new List<PeriodRequest>(), null);

            if (IsValidJson(result))
            {
                var deserialized = JsonConvert.DeserializeObject<PeriodResponseWrapper>(result);
                return (deserialized?.Periods ?? new List<PeriodRequest>(), null);
            }
            else
            {
                return (new List<PeriodRequest>(), result);
            }
        }

        private bool IsValidJson(string input)
        {
            input = input.Trim();
            if ((input.StartsWith("{") && input.EndsWith("}")) || // object
                (input.StartsWith("[") && input.EndsWith("]")))   // array
            {
                try
                {
                    var _ = JToken.Parse(input);
                    return true;
                }
                catch (JsonReaderException)
                {
                    return false;
                }
            }
            return false;
        }


        public Task<PeriodRequest> UpdateAsync(string procedureName, PeriodRequest model)
        {
            throw new NotImplementedException();
        }
        public async Task<PeriodRequest> AddAsync(string procedureName, PeriodRequest model)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@PeriodType", model.PeriodType, DbType.String);
            parameters.Add("@CompanyId", model.Company_Id, DbType.Int32);
            parameters.Add("@CreatedBy", model.CreatedBy, DbType.Int32);
           // parameters.Add("@Is_Custom", model.IsCustom, DbType.Boolean);
            //parameters.Add("@Is_Default", model.IsDefault, DbType.Boolean);

            // Optional: only add FromDate/ToDate if IsCustom = true
           // if (model.IsCustom)
            //{
                parameters.Add("@FromDate", model.PeriodFrom_Date, DbType.Date);
                parameters.Add("@ToDate", model.PeriodTo_Date, DbType.Date);
           // }
            parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information); // Cast enum to int
            parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Insert); // Cast enum to int
            parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.PayPeriod); // Cast enum to int

            try
            {
               // var result = await _dbConnection.ExecuteAsyncExecuteAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
                var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
                if (result != null)
                {
                    model.StatusMessage = result.ApplicationMessage;
                    model.MessageType = result.ApplicationMessageType;
                }                
            }
            catch (Exception ex)
            {               
                throw;
            }

            return model;
        }


        public Task<PeriodRequest> DeleteAsync(string procedureName, object parameters)
        {
            throw new NotImplementedException();
        }
    }
}
