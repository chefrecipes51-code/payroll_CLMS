using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Payroll.Common.EnumUtility;
using PayrollMasterService.BAL.Models;
using PayrollMasterService.DAL.Interface;

namespace PayrollMasterService.DAL.Service
{
    public class EntityTaxStatutoryServiceRepository : IEntityTaxStatutoryRepository
    {
        private readonly IDbConnection _dbConnection;

        // Created by Krunali Gohil payroll-421
        public EntityTaxStatutoryServiceRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        #region EntityTaxStatutory EndPoint Handlers(CRUD)

        #region Add
        public async Task<EntityTaxStatutory> AddAsync(string procedureName, EntityTaxStatutory entityTaxStatutory)
        {
            var parameters =new DynamicParameters();
            parameters.Add("@Entity_statutory_Id", entityTaxStatutory.Entity_statutory_Id);
            parameters.Add("@Employee_id", entityTaxStatutory.Employee_id);
            parameters.Add("@PayrollNo", entityTaxStatutory.PayrollNo);
            parameters.Add("@Company_Id", entityTaxStatutory.Company_Id);
            parameters.Add("@PF_No", entityTaxStatutory.PF_No);
            parameters.Add("@PF_Employer_Contribution", entityTaxStatutory.PF_Employer_Contribution);
            parameters.Add("@PF_Employee_Contribution", entityTaxStatutory.PF_Employee_Contribution);
            parameters.Add("ESIC_No", entityTaxStatutory.ESIC_No) ;
            parameters.Add("@ESIC_Employer_Contribution", entityTaxStatutory.ESIC_Employer_Contribution);
            parameters.Add("@ESIC_Employee_Contribution", entityTaxStatutory.ESIC_Employee_Contribution);
            parameters.Add("Professional_Tax", entityTaxStatutory.Professional_Tax);
            parameters.Add("Is_Gratuity_Eligibility", entityTaxStatutory.Is_Gratuity_Eligibility) ;
            parameters.Add("Gratuity_Account_No", entityTaxStatutory.Gratuity_Account_No);
            parameters.Add("TIN_No", entityTaxStatutory.TIN_No);

            parameters.Add("@IsActive", entityTaxStatutory.IsActive);
            parameters.Add("@CreatedBy", entityTaxStatutory.CreatedBy);
            // Additional parameters for messages and status using enum values
            parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information); // Cast enum to int
            parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Insert); // Cast enum to int
            parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.EntityTaxStatutory); // Cast enum to int

            var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters,commandType : CommandType.StoredProcedure);

            if(result != null)
            {
                entityTaxStatutory.StatusMessage= result.ApplicationMessage;
                entityTaxStatutory.MessageType = result.ApplicationMessageType;
            }

            return entityTaxStatutory;
        }
        #endregion

        #region Delete
        public async Task<EntityTaxStatutory> DeleteAsync(string procedureName, object entityTaxStatutory)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Entity_statutory_Id", ((dynamic)entityTaxStatutory).Entity_statutory_Id);
            parameters.Add("@UpdatedBy", ((dynamic)entityTaxStatutory).UpdatedBy);
            parameters.Add("@MessageType",(int)EnumUtility.ApplicationMessageTypeEnum.Information);
            parameters.Add("@MessageMode",(int)EnumUtility.ApplicationMessageModeEnum.Delete);
            parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.EntityTaxStatutory);

            var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
            if (result != null)
            {
                ((dynamic)entityTaxStatutory).StatusMessage = result.ApplicationMessage;
                ((dynamic)entityTaxStatutory).MessageType= result.ApplicationMessageType;
            }
            return (EntityTaxStatutory)entityTaxStatutory;
        }
        #endregion

        #region GetAll
        public async Task<IEnumerable<EntityTaxStatutory>> GetAllAsync(string procedureName)
        {
            return await _dbConnection.QueryAsync<EntityTaxStatutory>(procedureName, commandType: CommandType.StoredProcedure);
        }
        #endregion

        #region GetId
        public async Task<EntityTaxStatutory> GetByIdAsync(string procedureName, object parameters)
        {
            var dynamicParameters = new DynamicParameters(parameters);
            return await _dbConnection.QuerySingleOrDefaultAsync<EntityTaxStatutory>(procedureName,dynamicParameters,commandType: CommandType.StoredProcedure);
        }
        #endregion

        #region update
        public async Task<EntityTaxStatutory> UpdateAsync(string procedureName, EntityTaxStatutory entityTaxStatutory)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Entity_statutory_Id", entityTaxStatutory.Entity_statutory_Id);
            parameters.Add("@Employee_id", entityTaxStatutory.Employee_id);
            parameters.Add("@PayrollNo", entityTaxStatutory.PayrollNo);
            parameters.Add("@Company_Id", entityTaxStatutory.Company_Id);
            parameters.Add("@PF_No", entityTaxStatutory.PF_No);
            parameters.Add("@PF_Employer_Contribution", entityTaxStatutory.PF_Employer_Contribution);
            parameters.Add("@PF_Employee_Contribution", entityTaxStatutory.PF_Employee_Contribution);
            parameters.Add("ESIC_No", entityTaxStatutory.ESIC_No);
            parameters.Add("@ESIC_Employer_Contribution", entityTaxStatutory.ESIC_Employer_Contribution);
            parameters.Add("@ESIC_Employee_Contribution", entityTaxStatutory.ESIC_Employee_Contribution);
            parameters.Add("Professional_Tax", entityTaxStatutory.Professional_Tax);
            parameters.Add("Is_Gratuity_Eligibility", entityTaxStatutory.Is_Gratuity_Eligibility);
            parameters.Add("Gratuity_Account_No", entityTaxStatutory.Gratuity_Account_No);
            parameters.Add("TIN_No", entityTaxStatutory.TIN_No);

            parameters.Add("@IsActive", entityTaxStatutory.IsActive);
            parameters.Add("@CreatedBy", entityTaxStatutory.CreatedBy);
            parameters.Add("@UpdatedBy", entityTaxStatutory.UpdatedBy);
            // Additional parameters for messages and status using enum values
            parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information); // Cast enum to int
            parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Update); // Cast enum to int
            parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.EntityTaxStatutory); // Cast enum to int

            var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);

            if (result != null)
            {
                entityTaxStatutory.StatusMessage = result.ApplicationMessage;
                entityTaxStatutory.MessageType = result.ApplicationMessageType;
            }

            return entityTaxStatutory;

        }
        #endregion

        #endregion
    }
}
