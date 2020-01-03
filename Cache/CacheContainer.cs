using danielDevelops.CommonInterfaces.Infrastructure;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace danielDevelops.Cache
{
    public static class Container
    { 
        
    }
    public class CacheContainer : ICacheContainer
    {
        readonly ConcurrentDictionary<string, IData<dynamic>> _cache = new ConcurrentDictionary<string, IData<dynamic>>();

        public void AddItemToCache<T>(string cacheName,
            T data, Func<ICustomContext, Task<T>> reloadMethodAsync,
            bool forceImmediateReload = false,
            int timeoutInMinutes = 10) where T : class
        {
            if (timeoutInMinutes < 1)
                throw new InvalidOperationException("You cannot set a cache timeout of less than one minute!!!!");
            Func<ICustomContext, Task<dynamic>> reload = async (ICustomContext ctx) => await reloadMethodAsync(ctx);
            IData<dynamic> cacheItem = new Data<dynamic>(data, reload, new TimeSpan(0, timeoutInMinutes, 0));
            _cache.TryAdd(cacheName, cacheItem);
        }
        public async Task<T> GetItemFromCache<T>(ICustomContext context,
            string cacheName,
            bool forceImmediateReload = false) where T : class
        {
            if (_cache.TryGetValue(cacheName, out IData<dynamic> cachedDataObject))
                return await cachedDataObject.GetAsync(context, forceImmediateReload);
            return null;
        }
        public async Task<T> GetAndLoadCacheItemAsync<T>(ICustomContext context,
            string cacheName,
            Func<ICustomContext, Task<T>> reloadMethodAsync,
            bool forceImmediateReload = false,
            int timeoutInMinutes = 10) where T : class
        {
            var cachedRec = await GetItemFromCache<T>(context, cacheName, forceImmediateReload);
            if (cachedRec != null)
                return cachedRec;
            var data = await reloadMethodAsync(context);
            AddItemToCache(cacheName, data, reloadMethodAsync, forceImmediateReload, timeoutInMinutes);
            return data;
        }
        public async Task ReloadCacheAsync(ICustomContext context)
        {
            foreach (var item in _cache)
            {
                await item.Value.ReloadDataAsync(context);
            }
        }
        public async Task ReloadCacheAsync(ICustomContext context, string cacheName)
        {
            if (_cache.TryGetValue(cacheName, out IData<dynamic> cache))
            {
                await cache.ReloadDataAsync(context);
            }

        }
    }
}
