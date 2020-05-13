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
    public interface ISqlGenericRepository<T> : 
        ISelectRepository<T>, 
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
