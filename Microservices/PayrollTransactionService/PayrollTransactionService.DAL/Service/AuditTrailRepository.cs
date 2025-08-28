using Dapper;
using PayrollTransactionService.BAL.ReportModel;
using PayrollTransactionService.DAL.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollTransactionService.DAL.Service
{
    /// <summary>
    /// Developer Name :- Harshida Parmar
    /// Created Date   :- 22-10-'24
    /// Message detail :- AuditTrail Service Repository perform CRUD
    /// </summary>
    public class AuditTrailRepository : IAuditTrailRepository
    {
        #region Constructor 
        private readonly IDbConnection _dbConnection;
        public AuditTrailRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
        #endregion
        public async Task<IEnumerable<AuditTrail>> GetAuditTrailsByDateRangeAsync(string procedureName, AuditTrail model)
        {
            var parameters = new
            {
                Company_id = model.CompanyId, 
                Date_From = model.DateFrom,
                Date_To = model.DateTo
            };
            return await _dbConnection.QueryAsync<AuditTrail>(procedureName, parameters, commandType: CommandType.StoredProcedure);
        }
        public async Task<IEnumerable<AuditTrail>> GetAllAsync(string procedureName)
        {
            return await _dbConnection.QueryAsync<AuditTrail>(procedureName, commandType: CommandType.StoredProcedure);
        }

        public async Task<AuditTrail> GetByIdAsync(string procedureName, object parameters)
        {
            return await _dbConnection.QuerySingleOrDefaultAsync<AuditTrail>(procedureName, parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<AuditTrail> UpdateAsync(string procedureName, AuditTrail model)
        {
            await _dbConnection.ExecuteAsync(procedureName, model, commandType: CommandType.StoredProcedure);
            return model;
        }

        public async Task<AuditTrail> AddAsync(string procedureName, AuditTrail model)
        {
            var id = await _dbConnection.QuerySingleAsync<int>(procedureName, model, commandType: CommandType.StoredProcedure);
            model.CompanyId = id; // Assuming the stored procedure returns an ID
            return model;
        }

        public async Task<AuditTrail> DeleteAsync(string procedureName, object parameters)
        {
            await _dbConnection.ExecuteAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
            return null; // Return as needed; possibly the deleted item info
        }

       
    }
}
