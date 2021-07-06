using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace danielDevelops.CommonInterfaces.GenericRepository
{
    public interface ISelectOneRepository<T, EntityKeyType> where T : class, IEntity<EntityKeyType>, new()
    {
        Task<T> GetByIDAsync(object id);
    }
}
