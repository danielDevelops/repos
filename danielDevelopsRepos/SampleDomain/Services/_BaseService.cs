using danielDevelops.CommonInterfaces;
using danielDevelops.CommonInterfaces.Infrastructure;
using danielDevelops.CommonInterfaces.Infrastructure.GenericRepository;
using danielDevelops.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SampleDomain.Services
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
        protected readonly string CurrentUsername;
        protected readonly IGenericRepository<T> Repo;
        public BaseService(ICustomContext context, string currentUsername)
        {
            Context = context;
            CurrentUsername = currentUsername;
            Repo = new GenericRepository<T, SqlParameter>(Context, CurrentUsername);
        }


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
        protected async Task<SqlResult<TT>> SaveAsync<TT>(TT value = default, bool reloadCache = false)
        {
            try
            {
                await Repo.SaveAsync();
                if (reloadCache)
                {
                    Repo.DetachT();
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
        }
    }
    public class SqlResult
    {
        public SqlResult()
        {

        }
        public SqlResult(string message, Status status)
        {
            Message = message;
            Result = status;
        }
        public string Message { get; set; }
        public Status Result { get; set; }
    }

    public class SqlResult<T> : SqlResult
    {
        public T ReturnedData { get; set; }


        public SqlResult()
        {

        }

        public SqlResult(T data, string message, Status status)
            : base(message, status)
        {
            ReturnedData = data;
        }
    }
}
