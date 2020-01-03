using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace danielDevelops.CommonInterfaces.Infrastructure
{
    public interface IGenericRepository<T> where T: class, IEntity, new()
    {
        T Insert(T entity);
        Task<IEnumerable<T>> BulkInsertAsync(IEnumerable<T> entites);

        Task<T> GetByIDAsync(object id);
        Task<IEnumerable<T>> GetAsync(
            Expression<Func<T, bool>> filter = null,
            IEnumerable<Expression<Func<T, object>>> includeProperties = null, 
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, 
            int skip = 0, 
            int? take = default(int?), 
            bool disableCacheForQueryPlan = false);
        IQueryable<T> GetQueryable(
            Expression<Func<T, bool>> filter = null,
            IEnumerable<Expression<Func<T, object>>> includeProperties = null, 
            bool disableCacheForQueryPlan = false);

        void Update(T entity, params Expression<Func<T, object>>[] changedProperties);

        void Delete(T entityToDelete);
        void Delete(object id);

        Task ExecuteStoredProcedureAsync(string sql, int timeoutInSeconds = 30, params object[] parameters);
        Task<IEnumerable<TT>> ExecuteStoredProcedureAsync<TT>(string sql, int timeoutInSeconds = 30, params SqlParameter[] parameters) where TT : class, new();
        Task<IEnumerable<TT>> ExecuteDynamicStoredProcedureAsync<TT>(string sprocName, int timeoutInSeconds = 30, params object[] parameterObjects) where TT : class, new();

        Task ExecuteSQLAsync(string sql, int timeoutInSeconds = 30);
        Task TruncateAsync();
        void Save();
        Task SaveAsync();
        T CreateDetachedEntity(T entity);
        void Detach(T entity);
        void DetachT();
        void Attach(T entity);
    }
}
