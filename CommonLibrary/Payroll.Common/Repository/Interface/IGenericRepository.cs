namespace Payroll.Common.Repository.Interface
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
