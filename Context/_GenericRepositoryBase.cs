using danielDevelops.CommonInterfaces.GenericRepository;
using danielDevelops.CommonInterfaces.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace danielDevelops.Context
{
    public abstract class GenericRepositoryBase<T> : IEntityOperationsRepository<T> where T :class, new()
    {
        protected readonly ICustomContext context;
        protected readonly DbSet<T> dbSet;
        public GenericRepositoryBase(ICustomContext context)
        {
            this.context = context;
            this.dbSet = this.context.Set<T>();
        }
        public void DetachAll()
        {
            foreach (var entry in context.ChangeTracker.Entries().ToList())
            {
                context.Entry(entry.Entity).State = EntityState.Detached;
            }
        }

        public void Attach(T entity)
        {
            dbSet.Attach(entity);
        }
        public void Detach(T entity)
        {
            context.Entry(entity).State = EntityState.Detached;
        }
        public void DetachT()
        {
            var allItems = context.ChangeTracker.Entries<T>().ToList();
            foreach (var item in allItems)
                item.State = EntityState.Detached;
        }

        public T CreateDetachedEntity(T entity)
        {
            var newEntity = (T)Activator.CreateInstance(typeof(T));
            foreach (var property in newEntity.GetType().GetProperties())
            {
                property.SetValue(newEntity, property.GetValue(entity));
            }
            return newEntity;
        }
    }
}
