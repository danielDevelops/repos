using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace danielDevelops.CommonInterfaces.GenericRepository
{
    public interface IQueryExecutionRepository
    {
        Task ExecuteSQLAsync(string sql, int timeoutInSeconds = 30);
        Task TruncateAsync();
    }
}
