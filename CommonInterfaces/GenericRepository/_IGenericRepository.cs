using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace danielDevelops.CommonInterfaces.GenericRepository
{
    public interface IGenericRepository<T> :
        ISelectOneRepository<T>, 
        IQueryExecutionRepository, 
        IBulkInsertRepository<T>,
        IDeleteRepository<T>,
        ISaveRespository,
        ISprocRepository,
        IUpdateRepository<T>,
        IEntityOperationsRepository<T>
        where T: class, IEntity, new()
    {
        
    }
}
