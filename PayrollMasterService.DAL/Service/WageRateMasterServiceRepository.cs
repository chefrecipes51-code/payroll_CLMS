using Dapper;
using Payroll.Common.ApplicationConstant;
using Payroll.Common.EnumUtility;
using PayrollMasterService.BAL.Models;
using PayrollMasterService.DAL.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

namespace PayrollMasterService.DAL.Service
{
    /// <summary>
    /// Developer Name :- Harshida Parmar
    /// Created Date   :- 18-Sep-2024
    /// Message detail :- Wage Rate Master Service Repository perform CRUD
    /// </summary>
    public class WageRateMasterServiceRepository : IWageRateMasterRepository
    {
        #region Constructor 
        private readonly IDbConnection _dbConnection;
        public WageRateMasterServiceRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
        #endregion

        #region Wage Rate Master Endpoint Handlers (CRUD)

        #region WageRateMaster Add
        /// <summary>
        /// Adds or updates the Wage Rate Master record asynchronously in the database using the specified stored procedure.
        /// </summary>       
        public async Task<WageRateMaster> AddAsync(string procedureName, WageRateMaster wageRateDetail)
        {
            var parameters = new DynamicParameters();

            // Required parameters
            parameters.Add("@WageRateCode", wageRateDetail.WageRateCode);
            parameters.Add("@WageRateName", wageRateDetail.WageRateName);
            parameters.Add("@WageRateType", wageRateDetail.WageRateType);
            parameters.Add("@WageRate", wageRateDetail.WageRate);
            parameters.Add("@CalculationHour", wageRateDetail.CalculationHour);
            parameters.Add("@CreatedBy", wageRateDetail.CreatedBy);

            //    // Optional parameters (Nullable in database)            
            parameters.Add("@WageRateDescription", wageRateDetail.WageRateDescription);
            parameters.Add("@EffectiveDate", wageRateDetail.EffectiveDate);

            // Additional parameters for messages and status using enum values
            parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information); // Cast enum to int
            parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Insert); // Cast enum to int
            parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.UserActivateDeactivateStatus); // Cast enum to int

            var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);

            if (result != null)
            {
                wageRateDetail.StatusMessage = result.ApplicationMessage;
                wageRateDetail.MessageType = result.ApplicationMessageType;
            }
            return wageRateDetail;
        }


        #endregion
        #region WageRateMaster Update
        /// <summary>
        /// Updates an existing Wage Rate Master record in the database.
        /// The method executes a stored procedure that updates the wage rate details, including
        /// parameters like WageRateCode, WageRateName, WageRateDescription, WageRateType, and more.
        /// Handles nullable fields by passing DBNull for null values and retrieves output messages
        /// after the stored procedure is executed.
        /// </summary>
        public async Task<WageRateMaster> UpdateAsync(string procedureName, WageRateMaster wageRateDetail)
        {
            var parameters = new DynamicParameters();

            // Assuming wageGradeDetail is passed as the object        

            parameters.Add("@WageRate_Id", wageRateDetail.WageRate_Id);
            parameters.Add("@WageRateCode", wageRateDetail.WageRateCode);
            parameters.Add("@WageRateName", wageRateDetail.WageRateName);
            parameters.Add("@WageRateDescription", wageRateDetail.WageRateDescription);
            parameters.Add("@WageRateType", wageRateDetail.WageRateType);
            parameters.Add("@WageRate", wageRateDetail.WageRate);
            parameters.Add("@CalculationHour", wageRateDetail.CalculationHour);
            parameters.Add("@EffectiveDate", wageRateDetail.EffectiveDate);
            parameters.Add("@IsActive", wageRateDetail.IsActive);
            parameters.Add("@CreatedBy", wageRateDetail.CreatedBy);
            parameters.Add("@UpdatedBy", wageRateDetail.UpdatedBy);

            // Additional parameters for messages and status using enum values
            parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information); // Cast enum to int
            parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Update); // Cast enum to int
            parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.WageRateMaster); // Cast enum to int

            var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);

            if (result != null)
            {
                wageRateDetail.StatusMessage = result.ApplicationMessage;
                wageRateDetail.MessageType = result.ApplicationMessageType;
            }
            return wageRateDetail;
        }
        #endregion
        #region WageRateMaster Delete
        /// <summary>
        /// Asynchronously deletes a wage rate entry from the database using the specified stored procedure.
        /// </summary>
        /// <param name="procedureName">The name of the stored procedure to execute for the delete operation.</param>
        /// <param name="wageRateDetail">An object containing the details of the wage rate to be deleted, such as its identifier.</param>
        public async Task<WageRateMaster> DeleteAsync(string procedureName, object wageRateDetail)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@WageRate_Id", ((dynamic)wageRateDetail).WageRate_Id, DbType.Int32);
            parameters.Add("@UpdatedBy", ((dynamic)wageRateDetail).UpdatedBy, DbType.Int32);
            // Additional parameters for messages and status using enum values
            parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information); // Cast enum to int
            parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Delete); // Cast enum to int
            parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.WageRateMaster); // Cast enum to int
                                                                                     // Execute the stored procedure
            var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);

            if (result != null)
            {
                ((dynamic)wageRateDetail).StatusMessage = result.ApplicationMessage;
                ((dynamic)wageRateDetail).MessageType = result.ApplicationMessageType;
            }
            return (WageRateMaster)wageRateDetail;
        }
        #endregion
        #region WageRateMaster Fetch All And By ID  
        /// <summary>
        /// Retrieves all Wage Rate Master records from the database by executing the specified stored procedure.
        /// </summary>
        /// <param name="procedureName">The name of the stored procedure to be executed.</param>
        public async Task<IEnumerable<WageRateMaster>> GetAllAsync(string procedureName)
        {
            return await _dbConnection.QueryAsync<WageRateMaster>(procedureName, commandType: CommandType.StoredProcedure);
        }
        /// <summary>
        /// Retrieves a single Wage Rate Master record from the database by its ID, 
        /// using the specified stored procedure and parameters.
        /// </summary>
        public async Task<WageRateMaster> GetByIdAsync(string procedureName, object parameters)
        {
            var dynamicParameters = new DynamicParameters(parameters); // Pass the entire object directly to DynamicParameters
            return await _dbConnection.QuerySingleOrDefaultAsync<WageRateMaster>(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure);
        }
        #endregion
        #endregion
    }
}
