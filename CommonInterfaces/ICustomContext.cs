using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace danielDevelops.CommonInterfaces.Infrastructure
{
    public interface ICustomContext : IDisposable
    {
        DbSet<TEntity> Set<TEntity>() where TEntity : class;
        //DbQuery<TEntity> Query<TEntity>() where TEntity : class;
        IModel Model { get; }
        ChangeTracker ChangeTracker { get; }
        EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
        DatabaseFacade Database { get; }
        Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task BulkInsertAsync<TEntity>(IList<TEntity> entities) where TEntity : class;
        bool IsDisposed { get; }

    }
}
