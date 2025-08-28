/***************************************************************************************************************
 *                                                                                                             *
 *  Description:                                                                                               *
 *  This repository class handles CRUD operations for the CompanyMaster  entity.                               *
 *  It uses the Dapper library for efficient database interaction and stored procedure execution.              *
 *                                                                                                             *
 *  Methods:                                                                                                   *
 *  - GetAllAsync           : Retrieves all CompanyMaster  records using a stored procedure.                   *
 *  - GetByIdAsync          : Retrieves a specific CompanyMaster  record by ID using a stored procedure.       *
 *  - AddCompanyAsync       : Inserts a new CompanyMaster  record into the database using a stored procedure.  *
 *  - UpdateCompanyAsync    : Updates an existing CompanyMaster  record using a stored procedure.              *
 *  - DeleteCompanyAsync    : Soft-deletes an CompanyMaster  record using a stored procedure.                  *
 *                                                                                                             *
 *  Key Features:                                                                                              *
 *  - Implements the ICompanyMasterRepository interface.                                               *
 *  - Handles input/output parameters for stored procedures using Dapper's DynamicParameters.                  *
 *  - Includes application-level enums for message type, mode, and module ID.                                  *
 *  - Ensures validation of returned messages and status from stored procedure execution.                      *
 *                                                                                                             *
 *  Author: Priyanshu Jain                                                                                     *
 *  Date  : 03-Oct-2024                                                                                        *
 *                                                                                                             *
 ***************************************************************************************************************/
using Dapper;
using Microsoft.Extensions.Configuration;
using Payroll.Common.EnumUtility;
using PayrollMasterService.BAL.Models;
using PayrollMasterService.BAL.Requests;
using PayrollMasterService.DAL.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollMasterService.DAL.Service
{
    public class CompanyMasterServiceRepository : ICompanyMasterRepository
    {
        private readonly IDbConnection _dbConnection;
        private readonly IConfiguration _configuration;
        public CompanyMasterServiceRepository(IConfiguration configuration)
        {
            //_dbConnection = dbConnection;
            _dbConnection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }

        #region Company Master Crud
        public async Task<IEnumerable<CompanyMaster>> GetAllAsync(string procedureName)
        {
            return await _dbConnection.QueryAsync<CompanyMaster>(procedureName, commandType: CommandType.StoredProcedure);
        }
        public async Task<CompanyMaster> GetByIdAsync(string procedureName, object parameters)
        {
            var dynamicParameters = new DynamicParameters(parameters); // Pass the entire object directly to DynamicParameters
            return await _dbConnection.QuerySingleOrDefaultAsync<CompanyMaster>(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure);
        }
        public async Task AddCompanyAsync(string procedureName, object companyMaster, string entityType)
        {
            var parameters = new DynamicParameters();
            switch (entityType)
            {
                case "CompanyMaster":
                    // Parameters for tbl_mst_company
                    parameters.Add("@Company_Id", ((dynamic)companyMaster).Company_Id);
                    parameters.Add("@CompanyType_ID", ((dynamic)companyMaster).CompanyType_ID);
                    parameters.Add("@Company_Code", ((dynamic)companyMaster).Company_Code);
                    parameters.Add("@Group_Id", ((dynamic)companyMaster).Group_Id);
                    parameters.Add("@CompanyName", ((dynamic)companyMaster).CompanyName);
                    parameters.Add("@CompanyPrintName", ((dynamic)companyMaster).CompanyPrintName);
                    parameters.Add("@IsParent", ((dynamic)companyMaster).IsParent);
                    parameters.Add("@CompanyShortName", ((dynamic)companyMaster).CompanyShortName);
                    parameters.Add("@CompanyLevel", ((dynamic)companyMaster).CompanyLevel);
                    parameters.Add("@Location_ID", ((dynamic)companyMaster).Location_ID);
                    parameters.Add("@Has_Subsidary", ((dynamic)companyMaster).Has_Subsidary);
                    parameters.Add("@IsActive", ((dynamic)companyMaster).IsActive);
                    parameters.Add("@CreatedBy", ((dynamic)companyMaster).CreatedBy);
                    // Additional parameters for messages and status using enum values
                    parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information); // Cast enum to int
                    parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Insert); // Cast enum to int
                    parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.CompanyMaster); // Cast enum to int
                                                                                            // Add output parameters for messages
                    parameters.Add("@OutCompanyId", dbType: DbType.Int64, direction: ParameterDirection.Output, size: 4000);
                    break;

                case "CompanyCorrespondance":
                    // Parameters for tbl_mst_companycorrespondance
                    parameters.Add("@Correspondance_ID", ((dynamic)companyMaster).Correspondance_ID);
                    parameters.Add("@Company_Id", ((dynamic)companyMaster).Company_Id);
                    parameters.Add("@CompanyAddress", ((dynamic)companyMaster).CompanyAddress);
                    parameters.Add("@Building_No", ((dynamic)companyMaster).Building_No);
                    parameters.Add("@Building_Name", ((dynamic)companyMaster).Building_Name);
                    parameters.Add("@Street", ((dynamic)companyMaster).Street);
                    parameters.Add("@Country_ID", ((dynamic)companyMaster).Country_ID);
                    parameters.Add("@State_Id", ((dynamic)companyMaster).State_Id);
                    parameters.Add("@City_ID", ((dynamic)companyMaster).City_ID);
                    parameters.Add("@Location_ID", ((dynamic)companyMaster).Location_ID);
                    parameters.Add("@Primary_Phone_no", ((dynamic)companyMaster).Primary_Phone_no);
                    parameters.Add("@Secondary_Phone_No", ((dynamic)companyMaster).Secondary_Phone_No);
                    parameters.Add("@Primary_Email_Id", ((dynamic)companyMaster).Primary_Email_Id);
                    parameters.Add("@Secondary_Email_ID", ((dynamic)companyMaster).Secondary_Email_ID);
                    parameters.Add("@WebsiteUrl", ((dynamic)companyMaster).WebsiteUrl);
                    parameters.Add("@Company_LogoImage_Path", ((dynamic)companyMaster).Company_LogoImage_Path);
                    parameters.Add("@IsActive", ((dynamic)companyMaster).IsActive);
                    parameters.Add("@CreatedBy", ((dynamic)companyMaster).CreatedBy);
                    // Additional parameters for messages and status using enum values
                    parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information); // Cast enum to int
                    parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Insert); // Cast enum to int
                    parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.CompanyCorrespondanceMaster); // Cast enum to int
                    break;

                case "CompanyStatutory":
                    // Parameters for tbl_mst_companystatutory
                    parameters.Add("@Statutory_Type_Id", ((dynamic)companyMaster).Statutory_Type_Id);
                    parameters.Add("@Company_Id", ((dynamic)companyMaster).Company_Id);
                    parameters.Add("@StatutoryType_Name", ((dynamic)companyMaster).StatutoryType_Name);
                    parameters.Add("@IsActive", ((dynamic)companyMaster).IsActive);
                    parameters.Add("@CreatedBy", ((dynamic)companyMaster).CreatedBy);
                    // Additional parameters for messages and status using enum values
                    parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information); // Cast enum to int
                    parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Insert); // Cast enum to int
                    parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.CompanyStatutoryMaster); // Cast enum to int
                    break;

                default:
                    throw new ArgumentException("Invalid entity type provided.");
            }

            // Execute stored procedure
            var result = await _dbConnection.QueryFirstOrDefaultAsync<dynamic>(procedureName, parameters, commandType: CommandType.StoredProcedure);
            // Retrieve the returned Company_Id and set it back to ((dynamic)companyMaster)
            if (entityType == "CompanyMaster")
            {
                var outCompanyId = parameters.Get<long?>("@OutCompanyId");

                // Update companyMaster only if outCompanyId is not null
                if (outCompanyId.HasValue)
                {
                    ((dynamic)companyMaster).Company_Id = (byte)outCompanyId.Value;
                }
            }
            // Check result for messages and status
            if (result != null)
            {
                ((dynamic)companyMaster).StatusMessage = result.ApplicationMessage;
                ((dynamic)companyMaster).MessageType = result.ApplicationMessageType;
            }
        }
        public async Task UpdateCompanyAsync(string procedureName, object companyMaster, string entityType)
        {
            var parameters = new DynamicParameters();
            switch (entityType)
            {
                case "CompanyMaster":
                    // Parameters for tbl_mst_company
                    parameters.Add("@Company_Id", ((dynamic)companyMaster).Company_Id);
                    parameters.Add("@CompanyType_ID", ((dynamic)companyMaster).CompanyType_ID);
                    parameters.Add("@Company_Code", ((dynamic)companyMaster).Company_Code);
                    parameters.Add("@Group_Id", ((dynamic)companyMaster).Group_Id);
                    parameters.Add("@CompanyName", ((dynamic)companyMaster).CompanyName);
                    parameters.Add("@CompanyPrintName", ((dynamic)companyMaster).CompanyPrintName);
                    parameters.Add("@IsParent", ((dynamic)companyMaster).IsParent);
                    parameters.Add("@ParentCompany_Id", ((dynamic)companyMaster).@ParentCompany_Id);//Added By Harshida 20-02-25
                    parameters.Add("@CompanyShortName", ((dynamic)companyMaster).CompanyShortName);
                    parameters.Add("@CompanyLevel", ((dynamic)companyMaster).CompanyLevel);
                    parameters.Add("@Location_ID", ((dynamic)companyMaster).Location_ID);
                    parameters.Add("@Has_Subsidary", ((dynamic)companyMaster).Has_Subsidary);
                    parameters.Add("@IsActive", ((dynamic)companyMaster).IsActive);
                    parameters.Add("@CreatedBy", ((dynamic)companyMaster).CreatedBy);
                    parameters.Add("@UpdatedBy", ((dynamic)companyMaster).UpdatedBy);
                    // Additional parameters for messages and status using enum values
                    parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information); // Cast enum to int
                    parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Update); // Cast enum to int
                    parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.CompanyMaster); // Cast enum to int
                    parameters.Add("@OutCompanyId", dbType: DbType.Int64, direction: ParameterDirection.Output, size: 4000);
                    break;

                case "CompanyCorrespondance":
                    // Parameters for tbl_mst_companycorrespondance
                    parameters.Add("@Correspondance_ID", ((dynamic)companyMaster).CompanyCorrespondance.Correspondance_ID);
                    parameters.Add("@Company_Id", ((dynamic)companyMaster).CompanyCorrespondance.Company_Id);
                    parameters.Add("@CompanyAddress", ((dynamic)companyMaster).CompanyCorrespondance.CompanyAddress);
                    parameters.Add("@Building_No", ((dynamic)companyMaster).CompanyCorrespondance.Building_No);
                    parameters.Add("@Building_Name", ((dynamic)companyMaster).CompanyCorrespondance.Building_Name);
                    parameters.Add("@Street", ((dynamic)companyMaster).CompanyCorrespondance.Street);
                    parameters.Add("@Country_ID", ((dynamic)companyMaster).CompanyCorrespondance.Country_ID);
                    parameters.Add("@State_Id", ((dynamic)companyMaster).CompanyCorrespondance.State_Id);
                    parameters.Add("@City_ID", ((dynamic)companyMaster).CompanyCorrespondance.City_ID);
                    parameters.Add("@Location_ID", ((dynamic)companyMaster).CompanyCorrespondance.Location_ID);
                    parameters.Add("@Primary_Phone_no", ((dynamic)companyMaster).CompanyCorrespondance.Primary_Phone_no);
                    parameters.Add("@Secondary_Phone_No", ((dynamic)companyMaster).CompanyCorrespondance.Secondary_Phone_No);
                    parameters.Add("@Primary_Email_Id", ((dynamic)companyMaster).CompanyCorrespondance.Primary_Email_Id);
                    parameters.Add("@Secondary_Email_ID", ((dynamic)companyMaster).CompanyCorrespondance.Secondary_Email_ID);
                    parameters.Add("@WebsiteUrl", ((dynamic)companyMaster).CompanyCorrespondance.WebsiteUrl);
                    parameters.Add("@Company_LogoImage_Path", ((dynamic)companyMaster).CompanyCorrespondance.Company_LogoImage_Path);
                    parameters.Add("@IsActive", ((dynamic)companyMaster).IsActive);
                    parameters.Add("@CreatedBy", ((dynamic)companyMaster).CreatedBy);
                    parameters.Add("@UpdatedBy", ((dynamic)companyMaster).UpdatedBy);
                    // Additional parameters for messages and status using enum values
                    parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information); // Cast enum to int
                    parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Update); // Cast enum to int
                    parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.CompanyCorrespondanceMaster); // Cast enum to int
                    break;

                case "CompanyStatutory":
                    // Parameters for tbl_mst_companystatutory
                    parameters.Add("@Statutory_Type_Id", ((dynamic)companyMaster).CompanyStatutory.Statutory_Type_Id);
                    parameters.Add("@Company_Id", ((dynamic)companyMaster).CompanyStatutory.Company_Id);
                    parameters.Add("@StatutoryType_Name", ((dynamic)companyMaster).CompanyStatutory.StatutoryType_Name);
                    parameters.Add("@IsActive", ((dynamic)companyMaster).CompanyStatutory.IsActive);
                    parameters.Add("@CreatedBy", ((dynamic)companyMaster).CompanyStatutory.CreatedBy);
                    parameters.Add("@UpdatedBy", ((dynamic)companyMaster).CompanyStatutory.UpdatedBy);
                    // Additional parameters for messages and status using enum values
                    parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information); // Cast enum to int
                    parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Update); // Cast enum to int
                    parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.CompanyStatutoryMaster); // Cast enum to int
                    break;

                default:
                    throw new ArgumentException("Invalid entity type provided.");
            }
            // Execute stored procedure
            var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
            // Check result for messages and status
            if (result != null)
            {
                ((dynamic)companyMaster).StatusMessage = result.ApplicationMessage;
                ((dynamic)companyMaster).MessageType = result.ApplicationMessageType;
            }
        }
        public async Task DeleteCompanyAsync(string procedureName, object companyMaster, string entityType)
        {
            var parameters = new DynamicParameters();
            switch (entityType)
            {
                case "CompanyMaster":
                    // Parameters for tbl_mst_company
                    parameters.Add("@Company_Id", ((dynamic)companyMaster).Company_Id);
                    parameters.Add("@UpdatedBy", ((dynamic)companyMaster).UpdatedBy);
                    // Additional parameters for messages and status using enum values
                    parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information); // Cast enum to int
                    parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Delete); // Cast enum to int
                    parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.CompanyMaster); // Cast enum to int
                    break;

                case "CompanyCorrespondance":
                    // Parameters for tbl_mst_companycorrespondance
                    parameters.Add("@Company_Id", ((dynamic)companyMaster).Company_Id);
                    parameters.Add("@UpdatedBy", ((dynamic)companyMaster).UpdatedBy);
                    // Additional parameters for messages and status using enum values
                    parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information); // Cast enum to int
                    parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Delete); // Cast enum to int
                    parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.CompanyCorrespondanceMaster); // Cast enum to int
                    break;

                case "CompanyStatutory":
                    // Parameters for tbl_mst_companystatutory
                    parameters.Add("@Company_Id", ((dynamic)companyMaster).Company_Id);
                    parameters.Add("@UpdatedBy", ((dynamic)companyMaster).UpdatedBy);
                    // Additional parameters for messages and status using enum values
                    parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information); // Cast enum to int
                    parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Delete); // Cast enum to int
                    parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.CompanyStatutoryMaster); // Cast enum to int
                    break;

                default:
                    throw new ArgumentException("Invalid entity type provided.");
            }
            // Execute stored procedure
            var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
            // Check result for messages and status
            if (result != null)
            {
                ((dynamic)companyMaster).StatusMessage = result.ApplicationMessage;
                ((dynamic)companyMaster).MessageType = result.ApplicationMessageType;
            }
        }
        public async Task<IEnumerable<CompanyCorrespondance>> GetAllCompanyCorrespondanceAsync(string procedureName)
        {
            return await _dbConnection.QueryAsync<CompanyCorrespondance>(procedureName, commandType: CommandType.StoredProcedure);
        }
        public async Task<CompanyCorrespondance> GetCompanyCorrespondanceByIdAsync(string procedureName, object parameters)
        {
            var dynamicParameters = new DynamicParameters(parameters); // Pass the entire object directly to DynamicParameters
            return await _dbConnection.QuerySingleOrDefaultAsync<CompanyCorrespondance>(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure);
        }
        public async Task<IEnumerable<CompanyStatutory>> GetAllCompanyStatutoryAsync(string procedureName)
        {
            return await _dbConnection.QueryAsync<CompanyStatutory>(procedureName, commandType: CommandType.StoredProcedure);
        }
        public async Task<CompanyStatutory> GetCompanyStatutoryByIdAsync(string procedureName, object parameters)
        {
            var dynamicParameters = new DynamicParameters(parameters); // Pass the entire object directly to DynamicParameters
            return await _dbConnection.QuerySingleOrDefaultAsync<CompanyStatutory>(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure);
        }
        public Task<CompanyMaster> AddAsync(string procedureName, CompanyMaster parameters)
        {
            throw new NotImplementedException();
        }
        public Task<CompanyMaster> UpdateAsync(string procedureName, CompanyMaster parameters)
        {
            throw new NotImplementedException();
        }
        public Task<CompanyMaster> DeleteAsync(string procedureName, object companyMaster)
        {
            throw new NotImplementedException();
        }
        #region Get List for Company Satutory and corpspondence
        public async Task<IEnumerable<CompanyCorrespondance>> GetAllCompanyCorrespondancesByCompanyIdAsync(string procedureName, object parameters)
        {
            var dynamicParameters = new DynamicParameters(parameters); // Pass the parameters to DynamicParameters
            return await _dbConnection.QueryAsync<CompanyCorrespondance>(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure);
        }
        public async Task<IEnumerable<CompanyStatutory>> GetAllCompanyStatutoriesByCompanyIdAsync(string procedureName, object parameters)
        {
            var dynamicParameters = new DynamicParameters(parameters); // Pass the parameters to DynamicParameters
            return await _dbConnection.QueryAsync<CompanyStatutory>(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure);
        }
        #endregion
        #endregion
        #region [CompanyCorrespondance CRUD][Added By Harshida]
        public async Task AddCompanyCorrespondanceAsync(string procedureName, object companyCorrespondanceMaster, string entityType)
        {
            var parameters = new DynamicParameters();
            switch (entityType)
            {
                case "CompanyCorrespondance":
                    // Parameters for tbl_mst_companycorrespondance
                    parameters.Add("@Correspondance_ID", 0);
                    parameters.Add("@Company_Id", ((dynamic)companyCorrespondanceMaster).Company_Id);
                    parameters.Add("@CompanyAddress", ((dynamic)companyCorrespondanceMaster).CompanyAddress);
                    parameters.Add("@Building_No", ((dynamic)companyCorrespondanceMaster).Building_No);
                    parameters.Add("@Building_Name", ((dynamic)companyCorrespondanceMaster).Building_Name);
                    parameters.Add("@Street", ((dynamic)companyCorrespondanceMaster).Street);
                    parameters.Add("@Country_ID", ((dynamic)companyCorrespondanceMaster).Country_ID);
                    parameters.Add("@State_Id", ((dynamic)companyCorrespondanceMaster).State_Id);
                    parameters.Add("@City_ID", ((dynamic)companyCorrespondanceMaster).City_ID);
                    parameters.Add("@Location_ID", ((dynamic)companyCorrespondanceMaster).Location_ID);
                    parameters.Add("@CorrespondanceType", 1); //In Update NOT REQUIRED SO PASS ANY VALUE
                    parameters.Add("@Primary_Phone_no", ((dynamic)companyCorrespondanceMaster).Primary_Phone_no);
                    parameters.Add("@Secondary_Phone_No", ((dynamic)companyCorrespondanceMaster).Secondary_Phone_No);
                    parameters.Add("@Primary_Email_Id", ((dynamic)companyCorrespondanceMaster).Primary_Email_Id);
                    parameters.Add("@Secondary_Email_ID", ((dynamic)companyCorrespondanceMaster).Secondary_Email_ID);
                    parameters.Add("@WebsiteUrl", ((dynamic)companyCorrespondanceMaster).WebsiteUrl);
                    parameters.Add("@Company_LogoImage_Path", ((dynamic)companyCorrespondanceMaster).Company_LogoImage_Path);
                    parameters.Add("@IsActive", ((dynamic)companyCorrespondanceMaster).IsActive);
                    parameters.Add("@CreatedBy", ((dynamic)companyCorrespondanceMaster).CreatedBy);
                    // Additional parameters for messages and status using enum values
                    parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information); // Cast enum to int
                    parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Insert); // Cast enum to int
                    parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.CompanyCorrespondanceMaster); // Cast enum to int
                    break;
                default:
                    throw new ArgumentException("Invalid entity type provided.");
            }

            // Execute stored procedure
            var result = await _dbConnection.QueryFirstOrDefaultAsync<dynamic>(procedureName, parameters, commandType: CommandType.StoredProcedure);
            if (result != null)
            {
                ((dynamic)companyCorrespondanceMaster).StatusMessage = result.ApplicationMessage;
                ((dynamic)companyCorrespondanceMaster).MessageType = result.ApplicationMessageType;
            }
        }
        public async Task UpdateCompanyCorrespondanceAsync(string procedureName, object companyCorrespondanceMaster, string entityType)
        {
            var parameters = new DynamicParameters();
            switch (entityType)
            {
                case "CompanyCorrespondance":
                    // Parameters for tbl_mst_companycorrespondance
                    parameters.Add("@Correspondance_ID", ((dynamic)companyCorrespondanceMaster).Correspondance_ID);
                    parameters.Add("@Company_Id", ((dynamic)companyCorrespondanceMaster).Company_Id);
                    parameters.Add("@CompanyAddress", ((dynamic)companyCorrespondanceMaster).CompanyAddress);
                    parameters.Add("@Building_No", ((dynamic)companyCorrespondanceMaster).Building_No);
                    parameters.Add("@Building_Name", ((dynamic)companyCorrespondanceMaster).Building_Name);
                    parameters.Add("@Street", ((dynamic)companyCorrespondanceMaster).Street);
                    parameters.Add("@Country_ID", ((dynamic)companyCorrespondanceMaster).Country_ID);
                    parameters.Add("@State_Id", ((dynamic)companyCorrespondanceMaster).State_Id);
                    parameters.Add("@City_ID", ((dynamic)companyCorrespondanceMaster).City_ID);
                    parameters.Add("@Location_ID", ((dynamic)companyCorrespondanceMaster).Location_ID);
                    parameters.Add("@CorrespondanceType", 1); //In Update NOT REQUIRED SO PASS ANY VALUE
                    parameters.Add("@Primary_Phone_no", ((dynamic)companyCorrespondanceMaster).Primary_Phone_no);
                    parameters.Add("@Secondary_Phone_No", ((dynamic)companyCorrespondanceMaster).Secondary_Phone_No);
                    parameters.Add("@Primary_Email_Id", ((dynamic)companyCorrespondanceMaster).Primary_Email_Id);
                    parameters.Add("@Secondary_Email_ID", ((dynamic)companyCorrespondanceMaster).Secondary_Email_ID);
                    parameters.Add("@WebsiteUrl", ((dynamic)companyCorrespondanceMaster).WebsiteUrl);
                    parameters.Add("@Company_LogoImage_Path", ((dynamic)companyCorrespondanceMaster).Company_LogoImage_Path);
                    parameters.Add("@IsActive", ((dynamic)companyCorrespondanceMaster).IsActive);
                    parameters.Add("@CreatedBy", ((dynamic)companyCorrespondanceMaster).CreatedBy);
                    parameters.Add("@UpdatedBy", ((dynamic)companyCorrespondanceMaster).UpdatedBy);
                    // Additional parameters for messages and status using enum values
                    parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information); // Cast enum to int
                    parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Update); // Cast enum to int
                    parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.CompanyCorrespondanceMaster); // Cast enum to int
                    break;
                default:
                    throw new ArgumentException("Invalid entity type provided.");
            }
            // Execute stored procedure
            var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
            // Check result for messages and status
            if (result != null)
            {
                ((dynamic)companyCorrespondanceMaster).StatusMessage = result.ApplicationMessage;
                ((dynamic)companyCorrespondanceMaster).MessageType = result.ApplicationMessageType;
            }
        }
        public async Task<IEnumerable<CompanyTypeRequest>> GetAllCompanyTypeAsync(string procedureName)
        {
            return await _dbConnection.QueryAsync<CompanyTypeRequest>(procedureName, commandType: CommandType.StoredProcedure);
        }
        public async Task<IEnumerable<CurrencyRequest>> GetAllCurrencyAsync(string procedureName, DynamicParameters parameters)
        {
            return await _dbConnection.QueryAsync<CurrencyRequest>(procedureName, parameters, commandType: CommandType.StoredProcedure);
        }
        #endregion
        #region[Company Configuration CRUD] [Added By Harshida]
        public async Task AddCompanyConfigurationAsync(string procedureName, object companyconfiguration)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@Company_Id", ((dynamic)companyconfiguration).CompanyId);
            parameters.Add("@StartDate", ((dynamic)companyconfiguration).StartDate);
            parameters.Add("@EndDate", ((dynamic)companyconfiguration).EndDate);
            parameters.Add("@Currency_ID", ((dynamic)companyconfiguration).CurrencyId);
            parameters.Add("@CreatedBy", ((dynamic)companyconfiguration).CreatedBy);
            // Additional parameters for messages and status using enum values
            parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information); // Cast enum to int
            parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Insert); // Cast enum to int
            parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.CompanyConfiguration); // Cast enum to int

            // Execute stored procedure
            var result = await _dbConnection.QueryFirstOrDefaultAsync<dynamic>(procedureName, parameters, commandType: CommandType.StoredProcedure);
            if (result != null)
            {
                ((dynamic)companyconfiguration).StatusMessage = result.ApplicationMessage;
                ((dynamic)companyconfiguration).MessageType = result.ApplicationMessageType;
            }
        }
        #endregion
        #region Added By Krunali 
        public async Task<IEnumerable<SubsidiaryMaster>> GetAllSubsidiaryMaster(string procedureName)
        {
            return await _dbConnection.QueryAsync<SubsidiaryMaster>(procedureName, commandType: CommandType.StoredProcedure);
        }

        public async Task<SubsidiaryMaster> AddSubsidiaryMasterAsync(string procedureName, SubsidiaryMaster subsidiaryMaster)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@Subsidiary_Id", ((dynamic)subsidiaryMaster).Subsidiary_Id);
            parameters.Add("@SubsidiaryType_Id", ((dynamic)subsidiaryMaster).SubsidiaryType_Id);
            parameters.Add("@Subsidiary_Code", ((dynamic)subsidiaryMaster).Subsidiary_Code);
            parameters.Add("@Subsidiary_Name", ((dynamic)subsidiaryMaster).Subsidiary_Name);
            parameters.Add("@Company_Id", ((dynamic)subsidiaryMaster).Company_Id);
            parameters.Add("@Location_ID", ((dynamic)subsidiaryMaster).Location_ID);
            parameters.Add("@Area_ID", ((dynamic)subsidiaryMaster).Area_id);


            parameters.Add("@IsActive", ((dynamic)subsidiaryMaster).IsActive);
            parameters.Add("@CreatedBy", ((dynamic)subsidiaryMaster).CreatedBy);
            // Additional parameters for messages and status using enum values
            parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information); // Cast enum to int
            parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Insert); // Cast enum to int
            parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.SubsidiaryMaster); // Cast enum to int
                                                                                       // Add output parameters for messages
                                                                                       // Execute stored procedure
            var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
            // Check result for messages and status
            if (result != null)
            {
                subsidiaryMaster.StatusMessage = result.ApplicationMessage;
                subsidiaryMaster.MessageType = result.ApplicationMessageType;
            }
            return subsidiaryMaster;
        }


        public async Task<SubsidiaryMaster> UpdateSubsidiaryMasterAsync(string procedureName, SubsidiaryMaster subsidiaryMaster)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Subsidiary_Id", subsidiaryMaster.Subsidiary_Id); // 0 for insert, greater than 0 for update
            parameters.Add("@SubsidiaryType_Id", subsidiaryMaster.SubsidiaryType_Id);
            parameters.Add("@Subsidiary_Code", subsidiaryMaster.Subsidiary_Code);
            parameters.Add("@Subsidiary_Name", subsidiaryMaster.Subsidiary_Name);
            parameters.Add("@Company_Id", subsidiaryMaster.Company_Id);
            parameters.Add("@Location_ID", subsidiaryMaster.Location_ID);
            parameters.Add("@Area_id", subsidiaryMaster.Area_id);
            parameters.Add("@IsActive", subsidiaryMaster.IsActive);
            parameters.Add("@CreatedBy", subsidiaryMaster.CreatedBy);
            parameters.Add("@UpdatedBy", subsidiaryMaster.UpdatedBy);
            // Additional parameters for messages and status using enum values
            parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information); // Cast enum to int
            parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Update); // Cast enum to int
            parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.SubsidiaryMaster); // Cast enum to int

            var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
            // Check result for messages and status
            if (result != null)
            {
                subsidiaryMaster.StatusMessage = result.ApplicationMessage;
                subsidiaryMaster.MessageType = result.ApplicationMessageType;
            }
            return subsidiaryMaster;
        }

        public async Task<SubsidiaryMaster> GetAllSubsidiaryMasterById(string procedureName, object parameters)
        {
            var dynamicParameters = new DynamicParameters(parameters);
            return await _dbConnection.QuerySingleOrDefaultAsync<SubsidiaryMaster>(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure);
        }



        public async Task<SubsidiaryMaster> DeleteSubsidiaryMasterAsync(string procedureName, SubsidiaryMaster subsidiaryMaster)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Subsidiary_Id", subsidiaryMaster.Subsidiary_Id);
            parameters.Add("@UpdatedBy", subsidiaryMaster.UpdatedBy);
            parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information); // Cast enum to int
            parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Delete); // Cast enum to int
            parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.SubsidiaryMaster); // Cast enum to

            var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
            // Check result for messages and status
            if (result != null)
            {
                ((dynamic)subsidiaryMaster).StatusMessage = result.ApplicationMessage;
                ((dynamic)subsidiaryMaster).MessageType = result.ApplicationMessageType;
            }
            return (SubsidiaryMaster)subsidiaryMaster;
        }

        #endregion
    }
}
