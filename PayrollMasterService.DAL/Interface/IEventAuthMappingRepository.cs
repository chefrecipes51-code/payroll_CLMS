using PayrollMasterService.BAL.Models;

namespace PayrollMasterService.DAL.Interface
{
    public interface IEventAuthMappingRepository : IGenericRepository<EventAuthSelect>
    {
        Task<EventAuthSetUp> AddNewAsync(string storedProcedure, EventAuthSetUp eventAuthSetUp);
        Task<IEnumerable<EventAuthSelect>> GetByFilterAttributesAsync(string procedureName, object parameters);
    }
}