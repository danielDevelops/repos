using danielDevelops.Cache;
using danielDevelops.CommonInterfaces.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace danielDevelops.Service
{
    public interface IServiceConstructor<T> where T : class
    {
        ICustomContext Context { get; }
        bool IsSharedContext { get; }
        ICacheContainer CacheContainer { get; }
        Expression<Func<T, bool>> CacheLoadFilter { get; }
        int CacheTimoutInMinutes { get; } 
        string Username { get; }
        Func<IQueryable<T>, IIncludableQueryable<T, object>> IncludePropertiesInCache { get; }
    }
}
