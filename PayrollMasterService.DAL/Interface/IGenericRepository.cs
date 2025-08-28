using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollMasterService.DAL.Interface
{
    public interface IGenericRepository<T>
    {
        Task<IEnumerable<T>> GetAllAsync(string procedureName);
        Task<T> GetByIdAsync(string procedureName, object parameters);
        Task<T> AddAsync(string procedureName, T model);
        Task<T> UpdateAsync(string procedureName, T model);
        Task<T> DeleteAsync(string procedureName, object parameters);
    }
}
