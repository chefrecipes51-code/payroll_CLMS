using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.BAL.Models;
using UserService.BAL.Requests;
using UserService.DAL.Interface;

namespace UserService.DAL.Service
{
    public class CountryMasterServiceRepository : ICountryMasterRepository
    {
        private readonly IDbConnection _dbConnection;
        public CountryMasterServiceRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<IEnumerable<CountryMaster>> GetAllAsync(string procedureName)
        {
            try
            {
                var result = await _dbConnection.QueryAsync<CountryMaster>(procedureName, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as necessary.
                throw new Exception($"An error occurred while requesting data from {procedureName}", ex);
            }

        }
    }
}
