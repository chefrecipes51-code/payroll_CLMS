using Dapper;
using DataMigrationService.BAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMigrationService.DAL.Interface
{
    public interface IServiceImportMasterRepository
    {
        Task<IEnumerable<object>> GetServiceImportAsync(string procedureName, ServiceImportMaster model);
        Task<IEnumerable<object>> GetServiceAsync(string procedureName, ServiceMaster model);
        Task<IEnumerable<object>> AddExecuteAsync(string storedProcedure, DynamicParameters parameters);
        Task<List<Dictionary<string, object>>> GetReturnedDataAsync(string procedureName, object parameters);
        Task<(List<Dictionary<string, object>> data, int returnCount)> GetServiceDataAsync(string procedureName, object parameters);
    }
}
