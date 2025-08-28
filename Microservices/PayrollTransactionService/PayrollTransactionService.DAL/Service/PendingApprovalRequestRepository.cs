using Dapper;
using PayrollTransactionService.BAL.Models;
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
    /// Created Date   :- 05-11-'24
    /// Message detail :- PendingApprovalRequest Service Repository perform CRUD
    /// </summary>
    public class PendingApprovalRequestRepository : IPendingApprovalRequestRepository
    {
        #region Constructor 
        /// <summary>
        /// Initializes a new instance of the <see cref="PendingApprovalRequestRepository"/> class with the specified database connection.
        /// </summary>
        /// <param name="dbConnection">The database connection to be used by the repository.</param>

        private readonly IDbConnection _dbConnection;
        public PendingApprovalRequestRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
        #endregion
        #region GetData
        /// <summary>
        /// Retrieves a list of pending approval requests based on the specified stored procedure and filter criteria.
        /// </summary>
        /// <param name="procedureName">The name of the stored procedure to execute.</param>
        /// <param name="model">The filter model containing criteria such as CompanyId and UserId.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable of <see cref="PendingApprovalRequest"/>.</returns>

        public async Task<IEnumerable<PendingApprovalRequest>> GetServiceApprovalRejectionsAsync(string procedureName, PendingApprovalRejectionFilter model)
        {
            var parameters = new
            {
                Company_Id = model.CompanyId,
                User_ID = model.UserId
            };

            return await _dbConnection.QueryAsync<PendingApprovalRequest>(
                procedureName,
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }
        #endregion
    }
}
