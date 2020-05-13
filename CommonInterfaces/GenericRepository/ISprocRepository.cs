using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace danielDevelops.CommonInterfaces.GenericRepository
{
    public interface ISprocRepository
    {
        Task ExecuteStoredProcedureAsync(string sql, int timeoutInSeconds = 30, params object[] parameters);
        Task<IEnumerable<TT>> ExecuteStoredProcedureAsync<TT>(string sql, int timeoutInSeconds = 30, params SqlParameter[] parameters) where TT : class, new();
        Task<IEnumerable<TT>> ExecuteDynamicStoredProcedureAsync<TT>(string sprocName, int timeoutInSeconds = 30, params object[] parameterObjects) where TT : class, new();
    }
}
