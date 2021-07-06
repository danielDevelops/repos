using danielDevelops.CommonInterfaces.GenericRepository;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace danielDevelops.CommonInterfaces.Infrastructure.GenericRepository
{
    public interface IGenericRepository<T, EntityKeyType> : 
        ISelectRepository<T>, 
        ISelectOneRepository<T, EntityKeyType>, 
        IQueryExecutionRepository, 
        IBulkInsertRepository<T>,
        IDeleteRepository<T, EntityKeyType>,
        ISaveRespository,
        ISprocRepository,
        IUpdateRepository<T>,
        IEntityOperationsRepository<T>
        where T: class, IEntity<EntityKeyType>, new()
    {

    }
}
