using System.Data.SqlClient;
using System.Data;

namespace Payroll.Common.DapperContext
{
    public sealed class DapperContext
    {
        private static DapperContext _instance;
        private static readonly object _lock = new object();
        private readonly string _connectionString = @"Server=192.168.7.213;Initial Catalog=AdvancePayrollDB_DEV;User Id=payrolldev;password=Mantra#2024;TrustServerCertificate=true;Integrated Security=False;MultipleActiveResultSets=true;";

        // Private constructor to prevent direct instantiation
        private DapperContext()
        {
           
        }

        // Singleton instance creation method
        public static DapperContext GetInstance()
        {
            // Double-checked locking to ensure thread safety
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        
                        _instance = new DapperContext();
                    }
                }
            }
            return _instance;
        }

        // Method to create a database connection
        public IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
