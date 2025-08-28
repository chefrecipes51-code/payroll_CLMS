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
    public class CompanyLocationServiceRepository : ICompanyLocationRepository
    {
        private readonly IDbConnection _dbConnection;

        public CompanyLocationServiceRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<CompanyLocationMapDto> GetCompanyLocationMapAsync(string procedureName, object parameters)
        {
            var dynamicParameters = new DynamicParameters(parameters);

            using (var multi = await _dbConnection.QueryMultipleAsync(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure))
            {
                var result = new CompanyLocationMapDto
                {
                    Countries = (await multi.ReadAsync<CountryDto>()).ToList(),
                    States = (await multi.ReadAsync<StateDto>()).ToList(),
                    Cities = (await multi.ReadAsync<CityDto>()).ToList(),
                    Locations = (await multi.ReadAsync<LocationDto>()).ToList(),
                    Roles= (await multi.ReadAsync<RoleDto>()).ToList(),
                    AreaLocations = (await multi.ReadAsync<AreaLocationDto>()).ToList(),
                    Areas = (await multi.ReadAsync<AreaGeographicDto>()).ToList(),
                };

                return result;
            }
        }

        
    }
}
