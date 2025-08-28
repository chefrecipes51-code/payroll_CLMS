using PayrollTransactionService.DAL.Interface;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PayrollTransactionService.BAL.Models;
using Dapper;

namespace PayrollTransactionService.DAL.Service
{
    public class NotificationRepository : INotificationRepository
    {
       
        #region Constructor 
       private readonly IGenericRepository<PayrollProcessStatus> _notificationRepository;
      
        private readonly IDbConnection _dbConnection;

        public NotificationRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
        #endregion

        public async Task<IEnumerable<PayrollProcessusingSignalR>> GetActiveNotificationsAsync()
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Cmp_Id", 32);
            parameters.Add("@Month_Id", 5);
            parameters.Add("@Year_Id", 2025);
            return await _dbConnection.QueryAsync<PayrollProcessusingSignalR>(
                "SP_SelectPayrollProcessusingSignalR",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }
        public async Task<Notification> GetByIdAsync(string procedureName, object parameters)
        {
            // Implementation if needed
            throw new NotImplementedException();
        }

        public async Task<Notification> UpdateAsync(string procedureName, Notification model)
        {
            // Implementation if needed
            throw new NotImplementedException();
        }

        public async Task<Notification> AddAsync(string procedureName, Notification model)
        {
            // Implementation if needed
            throw new NotImplementedException();
        }

        public async Task<Notification> DeleteAsync(string procedureName, object parameters)
        {
            // Implementation if needed
            throw new NotImplementedException();
        }

        
    }
}
