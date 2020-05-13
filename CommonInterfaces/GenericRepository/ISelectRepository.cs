using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace danielDevelops.CommonInterfaces.GenericRepository
{
    public interface ISelectRepository<T> where T : class, new()
    {
        Task<IEnumerable<T>> GetAsync(
           Expression<Func<T, bool>> filter = null,
           Func<IQueryable<T>, IIncludableQueryable<T, object>> includeProperties = null,
           Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
           int skip = 0,
           int? take = default(int?),
           bool disableCacheForQueryPlan = false);
        IQueryable<T> GetQueryable(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> includeProperties = null,
            bool disableCacheForQueryPlan = false);
    }
}
