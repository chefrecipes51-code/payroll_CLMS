using Microsoft.Extensions.Caching.Memory;

namespace Payroll.Common.Repository.Interface
{
    public interface ICachingServiceRepository
    {
        T GetOrCreate<T>(string cacheKey, Func<ICacheEntry, T> createItem, TimeSpan? absoluteExpirationRelativeToNow = null);
        void Remove(string cacheKey);
    }
}
