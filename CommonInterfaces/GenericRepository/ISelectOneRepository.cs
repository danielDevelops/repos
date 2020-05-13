using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace danielDevelops.CommonInterfaces.GenericRepository
{
    public interface ISelectOneRepository<T> where T : class, IEntity, new()
    {
        Task<T> GetByIDAsync(object id);
    }
}
