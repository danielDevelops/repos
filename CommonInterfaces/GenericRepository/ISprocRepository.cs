using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace danielDevelops.CommonInterfaces.GenericRepository
{
    public interface ISprocRepository
    {
        Task ExecuteStoreQueryAsync(string sql, int timeoutInSeconds = 30, params object[] parameters);
        Task<IEnumerable<TT>> ExecuteStoreQueryAsync<TT>(string sql, int timeoutInSeconds = 30) where TT : class, new();
        Task<IEnumerable<TT>> ExecuteStoreQueryAsync<TT>(string sql, int timeoutInSeconds = 30, params DbParameter[] parameters) where TT : class, new();
        Task<IEnumerable<TT>> ExecuteStoreQueryAsync<TT>(string sql, int timeoutInSeconds = 30, params object[] parameterObjects) where TT : class, new();
    }
}
