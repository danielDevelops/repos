using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace danielDevelops.CommonInterfaces.GenericRepository
{
    public interface IEntityOperationsRepository<T> where T : class, new()
    {
        T CreateDetachedEntity(T entity);
        void Detach(T entity);
        void DetachT();
        void Attach(T entity);
    }
}
