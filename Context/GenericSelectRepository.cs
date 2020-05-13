using danielDevelops.CommonInterfaces.GenericRepository;
using danielDevelops.CommonInterfaces.Infrastructure;
using danielDevelops.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace danielDevelops.Context
{
    public class GenericSelectRepository<T> : GenericRepositoryBase<T>, ISelectRepository<T> where T : class, new()
    {
        public GenericSelectRepository(ICustomContext context)
            : base(context)

        {
        }

        public async Task<IEnumerable<T>> GetAsync(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> includeProperties = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            int skip = 0,
            int? take = null,
            bool disableCacheForQueryPlan = false)
        {
            var data = GetQueryable(filter, includeProperties, disableCacheForQueryPlan);
            if (orderBy == null)
                return await data.SkipAndTake(skip, take).ToListAsync();
            return await orderBy(data).SkipAndTake(skip, take).ToListAsync();
        }

        public IQueryable<T> GetQueryable(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> includeProperties = null,
            bool disableCacheForQueryPlan = false)
        {
            IQueryable<T> query = dbSet;
            if (filter != null)
                query = query.Where(filter);
            if (includeProperties != null)
                query = includeProperties(query);
            if (disableCacheForQueryPlan)
                throw new NotImplementedException("Setting disable cache for query plan hasn't been impletemented yet.  Will come at a later date.  Please do not use this value.");
            return query;
        }
    }
}
