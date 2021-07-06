using danielDevelops.CommonInterfaces;
using danielDevelops.CommonInterfaces.Infrastructure;
using danielDevelops.CommonInterfaces.Infrastructure.GenericRepository;
using danielDevelops.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;


namespace danielDevelops.Infrastructure
{
    public class GenericRepository<T, RdbmsParameterType, EntityKeyType> 
        : GenericRepositoryBase<T>, IGenericRepository<T, EntityKeyType> 
        where T : class, IEntity<EntityKeyType>, new()
        where RdbmsParameterType : DbParameter, new()
    {
        readonly string CurrentUser;
        private readonly string parameterPrefix;

        public GenericRepository(ICustomContext context, string currentUser, string parameterPrefix = "@")
            : base(context)
        {
            CurrentUser = currentUser;
            this.parameterPrefix = parameterPrefix;
        }
        public T Insert(T entity)
        {
            context.UpdateAllPropertiesToValue(entity, "Created", DateTime.Now);
            context.UpdateAllPropertiesToValue(entity, "CreatedBy", CurrentUser);
            dbSet.Add(entity);
            return entity;
        }

        public async Task<IEnumerable<T>> BulkInsertAsync(IEnumerable<T> entites)
        {
            foreach (var item in entites)
            {
                context.UpdateAllPropertiesToValue(item, "Created", DateTime.Now);
                context.UpdateAllPropertiesToValue(item, "CreatedBy", CurrentUser);
            }
            await context.BulkInsertAsync(entites.ToList());
            return entites;
        }
        public async Task<T> GetByIDAsync(object id)
        {
            return await dbSet.FindAsync(id);
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
            return await orderBy(data).SkipAndTake(skip,take).ToListAsync();
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

        public void Update(T entity,params Expression<Func<T,object>>[] changedProperties)
        {
            if (!IsAttached(entity))
                dbSet.Attach(entity);
            context.UpdateAllPropertiesToValue(entity, "Updated", DateTime.Now);
            context.UpdateAllPropertiesToValue(entity, "Modified", DateTime.Now);
            context.UpdateAllPropertiesToValue(entity, "ModifiedBy", CurrentUser);
            if (changedProperties != null && changedProperties.Count() > 0)
            {
                foreach (var item in changedProperties)
                {
                    context.Entry(entity).Property(item).IsModified = true;
                }
            }
            else
            {
                context.Entry(entity).State = EntityState.Modified;
                context.SetFieldsAsNotModified(entity, new string[] { "Created", "CreatedBy" });
            }
        }


        public void Delete(object id)
        {
            var entityToDelete = dbSet.Find(id);
            Delete(entityToDelete);
        }
        public void Delete(T entityToDelete)
        {
            if (context.Entry(entityToDelete).State == EntityState.Detached)
            {
               dbSet.Attach(entityToDelete);
            }
            dbSet.Remove(entityToDelete);
        }

    
        public async Task ExecuteSQLAsync(string sql, int timeoutInSeconds = 30)
        {
            context.Database.SetCommandTimeout(new TimeSpan(0, 0, timeoutInSeconds));
            await context.Database.ExecuteSqlRawAsync(sql);
        }

        public async Task<IEnumerable<TT>> ExecuteStoreQueryAsync<TT>(string sql, int timeoutInSeconds = 30) where TT : class, new()
        {
            context.Database.SetCommandTimeout(new TimeSpan(0, 0, timeoutInSeconds));
            var query = context.Set<TT>().FromSqlRaw(sql);
            return await query.ToListAsync();
        }

        public async Task ExecuteStoreQueryAsync(string sql,int timeoutInSecond = 30, params object[] parameters)
        {
            context.Database.SetCommandTimeout(new TimeSpan(0, 0, timeoutInSecond));
            await context.Database.ExecuteSqlRawAsync(sql, parameters);
        }

        public async Task<IEnumerable<TT>> ExecuteStoreQueryAsync<TT>(string sql,int timeoutInSeconds = 30, params DbParameter[] parameters) where TT : class, new()
        {
            context.Database.SetCommandTimeout(new TimeSpan(0, 0, timeoutInSeconds));
            var query = context.Set<TT>().FromSqlRaw(sql, parameters);
            return await query.ToListAsync();
        }

        public async Task<IEnumerable<TT>> ExecuteStoreQueryAsync<TT>(string sprocName,int timeoutInSeconds = 30, params object[] parameters) where TT : class, new()
        {
            var parms = Extensions.CreateParameterList<RdbmsParameterType>(parameters);
            return await ExecuteStoreQueryAsync<TT>($"{sprocName} {string.Join(",", parms.Select(t => $"{parameterPrefix}{t.ParameterName}"))}", timeoutInSeconds, parms.ToArray());
        }



        public async Task SaveAsync()
        {
            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateException exception)
            {
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    System.Diagnostics.Debug.WriteLine(exception);
                }
                DetachAll();
                throw;
            }
            catch (Exception exception)
            {
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    System.Diagnostics.Debug.WriteLine(exception);
                }
                DetachAll();
                throw;
            }
        }
       
        public void Save()
        {
            this.SaveAsync().Wait();
        }

        public async Task TruncateAsync()
        {
            var tableName = context.GetTableName<T>();
            await ExecuteSQLAsync($"TRUNCATE TABLE {tableName}", 60);
        }
        private bool IsAttached(T entity)
        {
            return dbSet.Local.Any(t => t.Id.Equals(entity.Id));
        }

    }
}
