using danielDevelops.Cache;
using danielDevelops.CommonInterfaces;
using danielDevelops.CommonInterfaces.Infrastructure;
using danielDevelops.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace danielDevelops.Service
{
    public enum Status
    {
        Success,
        Failure,
        Warning
    }
    public abstract class BaseService<T> : IDisposable where T : class, IEntity, new()
    {
        protected readonly ICustomContext Context;
        protected readonly bool IsSharedContext = false;
        private readonly ICacheContainer cacheContainer;
        private readonly Expression<Func<T, bool>> cacheLoadFilter;
        private readonly IEnumerable<Expression<Func<T,object>>> includePropertiesInCache;
        private readonly int cacheTimeoutInMinutes;
        protected readonly string CurrentUserName;
        protected readonly IGenericRepository<T> Repo;
        private readonly string cacheName = typeof(T).FullName;
        private readonly Func<ICustomContext, Task<IEnumerable<T>>> reloadMethod;

        protected BaseService(IServiceConstructor<T> constructor)
        {
            CurrentUserName = constructor.Username;
            Context = constructor.Context;
            IsSharedContext = constructor.IsSharedContext;
            Repo = new GenericRepository<T>(this.Context, CurrentUserName);
            cacheContainer = constructor.CacheContainer;
            cacheLoadFilter = constructor.CacheLoadFilter;
            cacheTimeoutInMinutes = constructor.CacheTimoutInMinutes;
            includePropertiesInCache = constructor.IncludePropertiesInCache;
            reloadMethod = async (context) =>
            {
                IGenericRepository<T> repo = new GenericRepository<T>(context, CurrentUserName);
                
                var data = await repo.GetAsync(cacheLoadFilter, includePropertiesInCache);
                var detachedData = data.Select(t => repo.CreateDetachedEntity(t)).ToList();
                return detachedData;
            };

        }
        /// <summary>
        /// Get value from Cache Engine
        /// </summary>
        /// <param name="queryFilter">Optional filter to run against data coming from cache</param>
        /// <param name="forceImmediateReload">Setting this to true will tell the cache to reload the data at the point it is retrieved</param>
        /// <returns>An IEnumerable of the DB Type.</returns>
        protected async Task<IEnumerable<T>> GetFromCacheAsync(
            Func<T, bool> queryFilter = null,
            bool forceImmediateReload = false)
        {
            var cachedData = await cacheContainer.GetAndLoadCacheItemAsync(this.Context,
                cacheName,
                reloadMethod,
                forceImmediateReload,
                cacheTimeoutInMinutes);
            if (queryFilter == null)
                return cachedData;
            return cachedData.Where(queryFilter);
        }

        /// <summary>
        /// Get single from cache 
        /// </summary>
        /// <param name="id">ID parameter for the data.</param>
        /// <returns>Returns a single value of T</returns>
        protected async Task<T> GetFromCacheByIdAsync(
          int id)
        {
            var val = await GetFromCacheAsync(t => t.Id == id);
            return val.SingleOrDefault();
        }

        /// <summary>
        /// Calls a reload of the data for the Cache for based on the current context.
        /// </summary>
        /// <returns>An awaitable task</returns>
        protected async Task ReloadCacheAsync() 
            => await cacheContainer.ReloadCacheAsync(Context, cacheName);

        /// <summary>
        /// Create a list of parameters based on T.  This is used to create a list of properties that have changed.
        /// </summary>
        /// <param name="properties">Params of properties</param>
        /// <returns>An IEnumerable of properties.</returns>
        protected IEnumerable<Expression<Func<T, object>>> CreateParametersList(params Expression<Func<T, object>>[] properties)
        {
            foreach (var item in properties)
            {
                yield return item;
            }
        }

        /// <summary>
        /// Call a save against the common Repo / Context.
        /// </summary>
        /// <typeparam name="TT">The return type for the data being saved.  This is to transform the data to an Interface Type.</typeparam>
        /// <param name="value">The value of the data being saved for the SqlResult Return Type.</param>
        /// <param name="reloadCache">Call Cache reload for the Current Cache.</param>
        /// <param name="reloadMethodOverload">Overload for the Reload Method, if reloading cache from another cache.</param>
        /// <returns>SqlResult with the data being added to the SqlResult.</returns>
        protected async Task<SqlResult<TT>> SaveAsync<TT>(TT value = default, bool reloadCache = false, Func<Task> reloadMethodOverload = null) 
        {
            try
            {
                await Repo.SaveAsync();
                if (reloadCache)
                {
                    Repo.DetachT();
                    if (reloadMethodOverload == null)
                        await ReloadCacheAsync();
                    else
                        await reloadMethodOverload();
                }
                return new SqlResult<TT>(value, "Save completed successfully", Status.Success);
            }
            catch (Exception x)
            {
                if (System.Diagnostics.Debugger.IsAttached)
                    System.Diagnostics.Debug.WriteLine(x);
                return new SqlResult<TT>(value, "An error occurred during the save operation", Status.Failure);
            }
        }

        public virtual void Dispose()
        {
            if (Repo != null && !Context.IsDisposed)
                Repo.Save();
            if (!Context.IsDisposed && !IsSharedContext)
                Context.Dispose();
        }
    }

    public class SqlResult<T>
    {
        public T ReturnedData { get; set; }
        public string Message { get; set; }
        public Status Result { get; set; }

        public SqlResult()
        {

        }

        public SqlResult(T data, string message, Status status)
        {
            ReturnedData = data;
            Message = message;
            Result = status;
        }
    }
}
