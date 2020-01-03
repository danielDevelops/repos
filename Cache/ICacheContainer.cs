using danielDevelops.CommonInterfaces.Infrastructure;
using System;
using System.Threading.Tasks;

namespace danielDevelops.Cache
{
    public interface ICacheContainer
    {
        void AddItemToCache<T>(string cacheName, 
            T data, 
            Func<ICustomContext, Task<T>> reloadMethodAsync, 
            bool forceImmediateReload,
            int timeoutInMinutes = 10) where T : class;
        Task<T> GetAndLoadCacheItemAsync<T>(ICustomContext context, 
            string cacheName, 
            Func<ICustomContext, Task<T>> reloadMethodAsync, 
            bool forceImmediateReload = false, 
            int timeoutInMinutes = 10) where T : class;
        Task<T> GetItemFromCache<T>(ICustomContext context, 
            string cacheName,
            bool forceImmediateReload = false) where T : class;
        Task ReloadCacheAsync(ICustomContext context);
        Task ReloadCacheAsync(ICustomContext context, string cacheName);
    }
}