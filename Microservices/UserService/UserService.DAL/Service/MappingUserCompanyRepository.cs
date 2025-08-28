using Dapper;
using Payroll.Common.EnumUtility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.BAL.Models;
using UserService.DAL.Interface;
using static Dapper.SqlMapper;

namespace UserService.DAL.Service
{
    /// <summary>
    /// Developer Name :- Harshida Parmar
    /// Created Date   :- 11-Oct-2024
    /// Message detail :- Mapping User Company Service Repository perform CRUD
    /// </summary>
    public class MappingUserCompanyRepository : IMappingUserCompanyRepository
    {
        #region Constructor 
        private readonly IDbConnection _dbConnection;

        public MappingUserCompanyRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
        #endregion
        #region Mapping User Company Endpoint Handlers (CRUD)
        #region Mapping User Company Fetch All And By ID  
        public async Task<IEnumerable<UserCompanyMapModel>> GetAllAsync(string procedureName)
        {
            return await _dbConnection.QueryAsync<UserCompanyMapModel>(procedureName, commandType: CommandType.StoredProcedure);
        }       
        public async Task<UserCompanyMapModel> GetByIdAsync(string procedureName, object parameters)
        {
            var dynamicParameters = new DynamicParameters(parameters); // Pass the entire object directly to DynamicParameters
            return await _dbConnection.QuerySingleOrDefaultAsync<UserCompanyMapModel>(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure);
        }
        #endregion
        #region Mapping User Company Add
        public async Task<UserCompanyMapModel> AddAsync(string procedureName, UserCompanyMapModel mappingusercompany)
        {
            try
            {
                var parameters = new DynamicParameters();
                // Add the parameters required for the stored procedure
                parameters.Add("@UserMapCompany_Id", (mappingusercompany.UserMapCompany_Id)); // 0 for insert, greater than 0 for update
                parameters.Add("@User_Id", (mappingusercompany.User_Id));
                parameters.Add("@Company_Id", (mappingusercompany.Company_Id));
                parameters.Add("@Effective_From_Dt", (mappingusercompany.Effective_From_Dt));
                parameters.Add("@IsActive", (mappingusercompany.IsActive));
                parameters.Add("@CreatedBy", (mappingusercompany.CreatedBy));
                //parameters.Add("@IsDeleted", (mappingusercompany.IsDeleted);
                // Additional parameters for messages and status using enum values
                parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information); // Cast enum to int
                parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Insert); // Cast enum to int
                parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.MappingUserCompany); // Cast enum to int
                                                                                             // Execute stored procedure
                var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
                // Check result for messages and status
                if (result != null)
                {
                    mappingusercompany.StatusMessage = result.ApplicationMessage;
                    mappingusercompany.MessageType = result.ApplicationMessageType;
                }
                return mappingusercompany; // Ensure we return the updated object
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        #endregion
        #region Mapping User Company Delete        
        public async Task<UserCompanyMapModel> DeleteAsync(string procedureName, object mappingusercompany)
        {
            try
            {
                var parameters = new DynamicParameters();
                // Assuming wageGradeDetail is passed as the object
                parameters.Add("@UserMapCompany_Id", ((dynamic)mappingusercompany).UserMapCompany_Id ); 
                parameters.Add("@UpdatedBy", ((dynamic)mappingusercompany).UpdatedBy);
                // Additional parameters for messages and status using enum values
                parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information); // Cast enum to int
                parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Delete); // Cast enum to int
                parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.MappingUserCompany); // Cast enum to int
                                                                                             // Execute stored procedure
                var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
                // Check result for messages and status
                if (result != null)
                {
                    ((dynamic)mappingusercompany).StatusMessage = result.ApplicationMessage;
                    ((dynamic)mappingusercompany).MessageType = result.ApplicationMessageType;
                }
                return (UserCompanyMapModel)mappingusercompany; 
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        #endregion
        #region Mapping User Company Update
        public async Task<UserCompanyMapModel> UpdateAsync(string procedureName, UserCompanyMapModel mappingusercompany)
        {
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("@UserMapCompany_Id", mappingusercompany.UserMapCompany_Id); 
                parameters.Add("@User_Id", mappingusercompany.User_Id);
                parameters.Add("@Company_Id", mappingusercompany.Company_Id);
                parameters.Add("@Effective_From_Dt", mappingusercompany.Effective_From_Dt);
                parameters.Add("@IsActive", mappingusercompany.IsActive);
                parameters.Add("@CreatedBy", mappingusercompany.CreatedBy);
                parameters.Add("@UpdatedBy", ((dynamic)mappingusercompany).UpdatedBy ?? DBNull.Value);

                // Additional parameters for messages and status using enum values
                parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information); // Cast enum to int
                parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Update); // Cast enum to int
                parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.MappingUserCompany); // Cast enum to int

                // Execute stored procedure
                var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);

                // Check result for messages and status
                if (result != null)
                {
                    mappingusercompany.StatusMessage = result.ApplicationMessage;
                    mappingusercompany.MessageType = result.ApplicationMessageType;
                }

                return mappingusercompany; // Ensure we return the updated object
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null; // In case of an exception, return null
            }
        }

        #endregion
        #endregion
    }
}
