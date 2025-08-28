/****************************************************************************************************
 *  Jira Task Ticket : PAYROLL-92                                                                                                *
 *  Description:                                                                                    *
 *  This repository implements caching functionalities using IMemoryCache.                          *
 *  It provides methods to retrieve or create cache entries and remove entries from the cache.      *
 *  Designed for efficient data storage and retrieval, ensuring optimal application performance.    *
 *                                                                                                  *
 *  Methods:                                                                                        *
 *  - GetOrCreate<T> : Retrieves an item from the cache or creates it if it doesn't exist.          *
 *                     Includes optional expiration time (default: 5 minutes).                      *
 *  - Remove         : Removes an item from the cache based on its unique cache key.                *
 *                                                                                                  *
 *  Key Features:                                                                                   *
 *  - Uses IMemoryCache for in-memory caching operations.                                           *
 *  - Supports flexible cache expiration options.                                                   *
 *  - Ensures thread-safe access to cached data.                                                    *
 *                                                                                                  *
 *  Author: Priyanshi Jain                                                                          *
 *  Date  : 04-Oct-2024                                                                             *
 *                                                                                                  *
 ****************************************************************************************************/
using Microsoft.Extensions.Caching.Memory;
using Payroll.Common.Repository.Interface;

namespace Payroll.Common.Repository.Service
{
    /// <summary>
    /// Developer Name  :- Priyanshi Jain
    /// Message detail  :- Implements caching functionalities using IMemoryCache, including methods to get or create cache entries and remove cache entries.
    /// Created Date    :- 04-Oct-2024
    /// Last Modified   :- 04-Oct-2024
    /// Modification    :- Initial creation of caching methods (GetOrCreate and Remove).
    /// </summary>
    public class CachingServiceRepository : ICachingServiceRepository
    {
        private readonly IMemoryCache _memoryCache;
        /// <summary>
        /// Developer Name  :- Priyanshi Jain
        /// Message detail  :- Constructor that initializes the IMemoryCache instance.
        /// Created Date    :- 04-Oct-2024
        /// Last Modified   :- 04-Oct-2024
        /// Modification    :- Initial implementation.
        /// </summary>
        /// <param name="memoryCache">The IMemoryCache instance injected via dependency injection.</param>
        public CachingServiceRepository(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }
        /// <summary>
        /// Developer Name  :- Priyanshi Jain
        /// Message detail  :- Retrieves an item from the cache or creates it if it doesn't exist, with optional expiration time.
        /// Created Date    :- 04-Oct-2024
        /// Last Modified   :- 04-Oct-2024
        /// Modification    :- Added default cache expiration of 5 minutes if no expiration is provided.
        /// </summary>
        /// <typeparam name="T">The type of the object to be cached.</typeparam>
        /// <param name="cacheKey">The unique key identifying the cache entry.</param>
        /// <param name="createItem">A function to create the cache entry if it doesn't exist.</param>
        /// <param name="absoluteExpirationRelativeToNow">Optional expiration time relative to now. Default is 5 minutes.</param>
        /// <returns>The cached item or a newly created one.</returns>
        public T GetOrCreate<T>(string cacheKey, Func<ICacheEntry, T> createItem, TimeSpan? absoluteExpirationRelativeToNow = null)
        {
            if (!_memoryCache.TryGetValue(cacheKey, out T cacheEntry))
            {
                // Create cache entry and options
                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow ?? TimeSpan.FromMinutes(5) // Default 5 minutes
                };

                // Use the provided function to create the item
                cacheEntry = createItem(_memoryCache.CreateEntry(cacheKey));

                // Set the item in the cache with the specified options
                _memoryCache.Set(cacheKey, cacheEntry, cacheEntryOptions);
            }
            return cacheEntry;
        }
        /// <summary>
        /// Developer Name  :- Priyanshi Jain
        /// Message detail  :- Removes an item from the cache using its cache key.
        /// Created Date    :- 04-Oct-2024
        /// Last Modified   :- 04-Oct-2024
        /// Modification    :- Initial implementation of the Remove method.
        /// </summary>
        /// <param name="cacheKey">The unique key identifying the cache entry to be removed.</param>
        public void Remove(string cacheKey)
        {
            _memoryCache.Remove(cacheKey);
        }
    }
}
