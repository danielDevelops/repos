using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace danielDevelops.CommonInterfaces.GenericRepository
{
    public interface IDeleteRepository<T> where T : class, IEntity, new()
    {
        void Delete(T entityToDelete);
        void Delete(object id);
    }
}
