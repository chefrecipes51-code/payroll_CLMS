using Payroll.Common.ApplicationModel.FilterModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.DAL.Interface
{
    // Note :- Added by Rohit Tiwari.
    public interface IGenericRepository<TEntity> 
    {
        Task<IEnumerable<TEntity>> GetAllAsync(string procedureName);
        Task<TEntity> GetByIdAsync(string procedureName, object parameters);
        Task<TEntity> AddAsync(string procedureName, TEntity model);
        Task<TEntity> UpdateAsync(string procedureName, TEntity model);

        // Note :- In delete I user model type as parameter because send multiple parameters like Id,UpdatedBy etc. 
        Task<TEntity> DeleteAsync(string procedureName, TEntity model);// Delete mean hard delete so for soft delete use ChangeStaus fun.. .
    }

    // Added by Harshida Parmar
    public interface IGenericRepositoryNew<T>
    {
        Task<IEnumerable<T>> GetAllAsync(string procedureName);
        // Returns a single item of type T by ID
        Task<T> GetByIdAsync(string procedureName, object parameters);
        // Returns the updated item of type T
        Task<T> UpdateAsync(string procedureName, T model);
        //Task AddAsync(string procedureName, object parameters);
        Task<T> AddAsync(string procedureName, T model);
        //Task DeleteAsync(string procedureName, object parameters);
        Task<T> DeleteAsync(string procedureName, object parameters);
    }


    // Note :- below code is commited because purojit sir not want to apply advance filter on all getallasync type methods or it is option to apply  based on modules

    //public interface IGenericRepository<TEntity, TFilter> // here is t is usermodel but in getallasync I want to pass commonfiltermodel
    //{
    //    Task<IEnumerable<TEntity>> GetAllAsync(string procedureName, TFilter model);
    //    Task<TEntity> GetByIdAsync(string procedureName, object parameters);
    //    Task<TEntity> AddAsync(string procedureName, TEntity model);
    //    Task<TEntity> UpdateAsync(string procedureName, TEntity model);
    //    Task<TEntity> DeleteAsync(string procedureName, TEntity model);
    //}

}
