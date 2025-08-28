using Dapper;
using Microsoft.Extensions.Configuration;
using PayrollTransactionService.BAL.Models;
using PayrollTransactionService.BAL.ReportModel;
using PayrollTransactionService.DAL.Interface;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace PayrollTransactionService.DAL.Service
{
    public class ContractorDetailsServiceRepository : IContractordetailsRepository
    {
        private readonly IDbConnection _dbConnection;
        private readonly IConfiguration _configuration;
        public ContractorDetailsServiceRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<IEnumerable<ContractorDetails>> GetAllContractorAsync(string procedureName, object parameters)
        {
            var dynamicParameters = new DynamicParameters(parameters);

            return await _dbConnection.QueryAsync<ContractorDetails>(
                procedureName,
                dynamicParameters,
                commandType: CommandType.StoredProcedure
            );

        }


        public async Task<IEnumerable<ContractorDetails>> GetContractorDetailsByCompanyIdAsync(string procedureName, object parameters)
        {
            var dynamicParameters = new DynamicParameters(parameters);

            return await _dbConnection.QueryAsync<ContractorDetails>(
                procedureName,
                dynamicParameters,
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<IEnumerable<Contractorprofile>> GetContractorProfileByCompanyIdAsync(string procedureName, object parameters)
        {
            var dynamicParameters = new DynamicParameters(parameters);

            return await _dbConnection.QueryAsync<Contractorprofile>(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure);

        }

    }
}
