using Dapper;
using PayrollMasterService.BAL.Models;
using PayrollMasterService.DAL.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollMasterService.DAL.Service
{
    public class GeographicalLocationServiceRepository : IGeographicalLocationRepository
    {
        private readonly IDbConnection _dbConnection;
        public GeographicalLocationServiceRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<IEnumerable<CityMaster>> GetAllCityAsync(string procedureName, object parameters)
        {
            var dynamicParameters = new DynamicParameters(parameters); // Pass the entire object directly to DynamicParameters
            return await _dbConnection.QueryAsync<CityMaster>(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure);

        }

        public async Task<IEnumerable<LocationMaster>> GetAllCityWiseLocationAsync(string procedureName, object parameters)
        {
            var dynamicParameters = new DynamicParameters(parameters); // Pass the entire object directly to DynamicParameters
            return await _dbConnection.QueryAsync<LocationMaster>(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<StateMaster>> GetAllStateAsync(string procedureName, object parameters)
        {
            var dynamicParameters = new DynamicParameters(parameters); // Pass the entire object directly to DynamicParameters
            return await _dbConnection.QueryAsync<StateMaster>(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure);
        }
    }
}
