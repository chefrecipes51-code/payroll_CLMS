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
    public class ServiceMasterServiceRepository : IServiceMasterRepository
    {
        private readonly IDbConnection _dbConnection;
        public ServiceMasterServiceRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<IEnumerable<ServiceMaster>> GetServicesByModuleId(string procedureName, object parameters)
        {
            try
            {
                var result = await _dbConnection.QueryAsync<ServiceMaster>(procedureName,parameters, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as necessary.
                throw new Exception($"An error occurred while requesting data from {procedureName}", ex);
            }

        }
        public async Task<ServiceMaster> GetServiceById( int? ServiceId)
        {
            try
            {
                string sql = "select ServiceID,ModuleID,ServiceName,NumberOfLevels from tbl_mst_services where IsActive=1 and ServiceId=" + ServiceId;
                var result = await _dbConnection.QueryAsync<ServiceMaster>(sql);
                return result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as necessary.
                throw new Exception($"An error occurred while requesting data from tbl_mst_services", ex);
            }

        }


    }
}
