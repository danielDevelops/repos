using danielDevelops.CommonInterfaces.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace danielDevelops.Cache
{
    internal class Data<T> : IData<T> where T : class
    {
        readonly Func<ICustomContext, Task<T>> _reloadMethodAsync;
        readonly TimeSpan _cacheTimeout;
        T _cachedData = null;
        public Data(T data, Func<ICustomContext, Task<T>> reloadMethodAsync, TimeSpan cacheTimeout)
        {
            _cachedData = data;
            _cacheTimeout = cacheTimeout;
            _reloadMethodAsync = reloadMethodAsync;
        }
        public DateTime Updated { get; private set; } = DateTime.Now;
        public bool IsExpired => Updated.Add(_cacheTimeout) < DateTime.Now;
        
        public async Task ReloadDataAsync(ICustomContext context)
        {
            _cachedData = await _reloadMethodAsync(context );
            Updated = DateTime.Now;
        }

        public async Task<T> GetAsync(ICustomContext context, bool forceReload = false)
        {
            if (IsExpired || forceReload)
                await ReloadDataAsync(context);
            var data = DeepCopy.ObjectCloner.Clone(_cachedData, false);
            return data;
        }
    }
}
