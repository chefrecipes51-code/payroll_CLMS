using Dapper;
using Microsoft.Extensions.Configuration;
using Payroll.Common.ApplicationConstant;
using Payroll.Common.EnumUtility;
using PayrollTransactionService.BAL.Models;
using PayrollTransactionService.DAL.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollTransactionService.DAL.Service
{
    public class MapEntityTaxRegimeServiceRepository : IMapEntityTaxRegimeRepository
    {
        private readonly IDbConnection _dbConnection;
        private readonly IConfiguration _configuration;
        public MapEntityTaxRegimeServiceRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            _dbConnection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }
        public async Task<IEnumerable<MapEntityTaxRegime>> GetAllAsync(string procedureName)
        {
            throw new NotImplementedException();
        }
        public async Task<MapEntityTaxRegime> GetByIdAsync(string procedureName, object parameters)
        {
            throw new NotImplementedException();
        }
        public async Task<EntityFilterResponse> GetAllEntityFiltersAsync(string procedureName, EntityFilterRequest request)
        {
            int safePageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
            int safePageSize = request.PageSize < 1 ? 10 : request.PageSize;

            
            var parameters = new DynamicParameters();
            parameters.Add("@Select_type", request.SelectType);
            parameters.Add("@Company_ID", request.CompanyId);
            parameters.Add("@Correspondance_ID", request.CorrespondanceId);
            parameters.Add("@Contractor_ID", request.ContractorId);
            parameters.Add("@Entity_ID", request.EntityId);
            parameters.Add("@Entity_Code", request.EntityCode);
            parameters.Add("@Entity_Name", request.EntityName);
            parameters.Add("@SelectFilter", request.SelectFilter);
            parameters.Add("@Regime_Id", request.Regime_Id);
            parameters.Add("@FinYear_ID", request.FinYear_ID);
            parameters.Add("@Trade_Id", request.Trade_Id);
            parameters.Add("@Skill_ID", request.Skill_ID);
            parameters.Add("@Grade_ID", request.Grade_ID);
            //parameters.Add("@PageNumber", request.PageNumber);
            //parameters.Add("@PageSize", request.PageSize);
            parameters.Add("@PageNumber", safePageNumber);
            parameters.Add("@PageSize", safePageSize);
            var response = new EntityFilterResponse();
            switch (request.SelectType)
            {
                case "C": // Contractor List
                    response.Contractors = (await _dbConnection.QueryAsync<ContractorResponse>(
               procedureName, parameters, commandType: CommandType.StoredProcedure)).ToList();
                    break;

                case "ED": // Entity Code List
                    response.EntityCodes = (await _dbConnection.QueryAsync<EntityCodeResponse>(
                 procedureName, parameters, commandType: CommandType.StoredProcedure)).ToList();
                    break;

                case "EM": // Entity Name List
                    response.EntityNames = (await _dbConnection.QueryAsync<EntityNameResponse>(
               procedureName, parameters, commandType: CommandType.StoredProcedure)).ToList();
                    break;

                case "CO": // Contractor-Entity Details
                    response.ContractorEntities = (await _dbConnection.QueryAsync<ContractorEntityResponse>(
               procedureName, parameters, commandType: CommandType.StoredProcedure)).ToList();
                    break;

                case "ET": // Grade-Entity Details
                    using (var multi = await _dbConnection.QueryMultipleAsync(procedureName, parameters, commandType: CommandType.StoredProcedure))
                    {
                        response.TotalCount = await multi.ReadFirstOrDefaultAsync<int>(); // Count from @sqlrtcnt
                        response.GradeMapEntities = (await multi.ReadAsync<GradeMapEntityResponse>()).ToList(); // Data from @sqlrt
                    }
                    break;

                default:
                    throw new ArgumentException("Invalid SelectType provided.");
            }
            return response;
        }
        public async Task<MapEntityTaxRegime> AddAsync(string procedureName, MapEntityTaxRegime mapEntityTaxRegime)
        {
            var parameters = new DynamicParameters();

            // Prepare UDT DataTable
            var entityTaxRegimeDataTable = new DataTable();
            entityTaxRegimeDataTable.Columns.Add("Contractor_Id", typeof(int));
            entityTaxRegimeDataTable.Columns.Add("Contractor_Code", typeof(char)); // string for char(1)
            entityTaxRegimeDataTable.Columns.Add("Entity_ID", typeof(int));
            entityTaxRegimeDataTable.Columns.Add("Entity_Code", typeof(string));
            entityTaxRegimeDataTable.Columns.Add("Regime_Id", typeof(int));
            entityTaxRegimeDataTable.Columns.Add("FinYear_ID", typeof(int));
            entityTaxRegimeDataTable.Columns.Add("IsActive", typeof(bool));

            // Populate UDT
            foreach (var entitytaxregime in mapEntityTaxRegime.EntityTaxRegime)
            {
                entityTaxRegimeDataTable.Rows.Add(
                    entitytaxregime.Contractor_Id,
                    entitytaxregime.Contractor_Code.ToString(),
                    entitytaxregime.Entity_ID,
                    entitytaxregime.Entity_Code,
                    entitytaxregime.Regime_Id,
                    entitytaxregime.FinYear_ID,
                    entitytaxregime.IsActive
                );
            }

            // Add parameters
            parameters.Add("@EntityTaxRegimeUDT", entityTaxRegimeDataTable.AsTableValuedParameter(DbConstants.UDTMapEntityTaxRegime));
            parameters.Add("@CreatedBy", mapEntityTaxRegime.CreatedBy);
            parameters.Add("@UpdatedBy", mapEntityTaxRegime.UpdatedBy); // Make sure UpdatedBy is included
            parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information);
            parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Insert);
            parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.MapTaxRegime);

            try
            {
                var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
                if (result != null)
                {
                    mapEntityTaxRegime.StatusMessage = result.ApplicationMessage;
                    mapEntityTaxRegime.MessageType = result.ApplicationMessageType;
                }
            }
            catch (Exception ex)
            {
                // Optionally log the error
                mapEntityTaxRegime.StatusMessage = "An error occurred while saving data.";
                mapEntityTaxRegime.MessageType = (int)EnumUtility.ApplicationMessageTypeEnum.Error;
                // You could log ex here if needed
            }

            return mapEntityTaxRegime;
        }
        public async Task<MapEntityTaxRegime> UpdateAsync(string procedureName, MapEntityTaxRegime model)
        {
            throw new NotImplementedException();
        }
        public async Task<MapEntityTaxRegime> DeleteAsync(string procedureName, object parameters)
        {
            throw new NotImplementedException();
        }
        public async Task<IEnumerable<FinancialYearMaster>> GetAllFinancialYearAsync(string procedureName, object parameters)
        {
            var dynamicParameters = new DynamicParameters(parameters);
            return await _dbConnection.QueryAsync<FinancialYearMaster>(
                procedureName,
                dynamicParameters,
                commandType: CommandType.StoredProcedure
            );
        }
    }
}
