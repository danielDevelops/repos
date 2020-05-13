using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace danielDevelops.CommonInterfaces.GenericRepository
{
    public interface IBulkInsertRepository<T> where T : class, new()
    {
        Task<IEnumerable<T>> BulkInsertAsync(IEnumerable<T> entites);
    }
}
