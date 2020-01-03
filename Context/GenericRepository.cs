using danielDevelops.CommonInterfaces;
using danielDevelops.CommonInterfaces.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;


namespace danielDevelops.Infrastructure
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class, IEntity,  new()
    {
        readonly DbSet<T> dbSet;
        readonly ICustomContext context;
        readonly string CurrentUser;
        public GenericRepository(ICustomContext context, string currentUser)
        {
            CurrentUser = currentUser;
            this.context = context;
            dbSet = this.context.Set<T>();
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
            IEnumerable<Expression<Func<T,object>>> includeProperties = null, 
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
            IEnumerable<Expression<Func<T, object>>> includeProperties = null, 
            bool disableCacheForQueryPlan = false)
        {
            IQueryable<T> query = dbSet;
            if (filter != null)
                query = query.Where(filter);
            foreach (var item in includeProperties ?? Enumerable.Empty<Expression<Func<T, object>>>())
            {
                query = query.Include(item);
            }
            if (disableCacheForQueryPlan)
                throw new NotImplementedException("Setting disable cache for query plan hasn't been impletemented yet.  Will come at a later date.  Please do not use this value.");
            return query;
        }

        public void Update(T entity,params Expression<Func<T,object>>[] changedProperties)
        {
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
            await context.Database.ExecuteSqlCommandAsync(sql);
        }
        public async Task ExecuteStoredProcedureAsync(string sql,int timeoutInSecond = 30, params object[] parameters)
        {
            context.Database.SetCommandTimeout(new TimeSpan(0, 0, timeoutInSecond));
            await context.Database.ExecuteSqlCommandAsync(sql, parameters);
        }

        public async Task<IEnumerable<TT>> ExecuteStoredProcedureAsync<TT>(string sql,int timeoutInSeconds = 30, params SqlParameter[] parameters) where TT : class, new()
        {
            context.Database.SetCommandTimeout(new TimeSpan(0, 0, timeoutInSeconds));
            var query = context.Set<TT>().FromSql(sql, parameters);
            return await query.ToListAsync();
        }

        public async Task<IEnumerable<TT>> ExecuteDynamicStoredProcedureAsync<TT>(string sprocName,int timeoutInSeconds = 30, params object[] parameters) where TT : class, new()
        {
            var parms = Extensions.CreateParameterList(parameters);
            return await ExecuteStoredProcedureAsync<TT>($"{sprocName} {string.Join(",", parms.Select(t => $"@{t.ParameterName}"))}", timeoutInSeconds, parms.ToArray());
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
        public void DetachAll()
        {
            foreach (var entry in context.ChangeTracker.Entries().ToList())
            {
                context.Entry(entry.Entity).State = EntityState.Detached;
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

        public void Attach(T entity)
        {
            dbSet.Attach(entity);
        }
        public void Detach(T entity)
        {
            context.Entry(entity).State = EntityState.Detached;
        }
        public void DetachT()
        {
            var allItems = context.ChangeTracker.Entries<T>().ToList();
            foreach (var item in allItems)
                item.State = EntityState.Detached;
        }
     
        public T CreateDetachedEntity(T entity)
        {
            var newEntity = (T)Activator.CreateInstance(typeof(T));
            foreach (var property in newEntity.GetType().GetProperties())
            {
                property.SetValue(newEntity, property.GetValue(entity));
            }
            return newEntity;
        }
    }
}
