using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace danielDevelops.CommonInterfaces.GenericRepository
{
    public interface IUpdateRepository<T> where T : class, new()
    {
        T Insert(T entity);

        void Update(T entity, params Expression<Func<T, object>>[] changedProperties);

    }
}
