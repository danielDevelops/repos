using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace danielDevelops.CommonInterfaces.GenericRepository
{
    public interface IDeleteRepository<T, EntityKeyType> where T : class, IEntity<EntityKeyType>, new()
    {
        void Delete(T entityToDelete);
        void Delete(object id);
    }
}
