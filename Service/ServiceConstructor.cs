using danielDevelops.Cache;
using danielDevelops.CommonInterfaces.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace danielDevelops.Service
{
    public class ServiceConstructor<T> : IServiceConstructor<T> where T : class
    {
        public ServiceConstructor(ICacheContainer cacheContainer, 
            ICustomContext context, 
            string username, 
            Expression<Func<T, bool>> cacheLoadFilter = null, 
            IEnumerable<Expression<Func<T,object>>> loadAdditionalPropertiesInCache = null,
            int cacheTimeoutInMinutes = 10,
            bool isSharedContext = true)
        {
            CacheContainer = cacheContainer;
            Context = context;
            IsSharedContext = isSharedContext;
            CacheLoadFilter = cacheLoadFilter;
            CacheTimoutInMinutes = cacheTimeoutInMinutes;
            Username = username;
            IncludePropertiesInCache = loadAdditionalPropertiesInCache;
        }

        public ICustomContext Context { get; private set; }

        public bool IsSharedContext { get; private set; } = false;

        public ICacheContainer CacheContainer { get; private set; }

        public Expression<Func<T, bool>> CacheLoadFilter { get; private set; }
        public int CacheTimoutInMinutes { get; private set; } = 10;
        public string Username { get; private set; }

        public IEnumerable<Expression<Func<T, object>>> IncludePropertiesInCache { get; private set; }
    }
}
