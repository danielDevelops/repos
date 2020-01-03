
using danielDevelops.CommonInterfaces.Infrastructure;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace danielDevelops.Infrastructure
{
    public abstract class CustomContext : DbContext, ICustomContext
    {
        readonly string _connectionString;
        public CustomContext(string nameOrconnectionString) : base()
        {
            if (!nameOrconnectionString.Contains(";"))
                nameOrconnectionString = ConnectionStringManager.Manager.GetConnectionString(nameOrconnectionString);
            _connectionString = nameOrconnectionString;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetAssembly(GetType()));

            var cascadeFKs = modelBuilder.Model.GetEntityTypes()
                            .SelectMany(t => t.GetForeignKeys())
                            .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

            foreach (var fk in cascadeFKs)
                fk.DeleteBehavior = DeleteBehavior.Restrict;

            base.OnModelCreating(modelBuilder);
        }
        public Task BulkInsertAsync<TEntity>(IList<TEntity> entities) where TEntity : class
        {
            return DbContextBulkExtensions.BulkInsertAsync(this,entities);
        }
        public bool IsDisposed { get; private set; } = false;
        public override void Dispose()
        {
            IsDisposed = true;
            base.Dispose();
        }

       
    }

    class ConnectionStringManager
    {
        public static readonly ConnectionStringManager Manager = new ConnectionStringManager();
        readonly ConcurrentDictionary<string, string> _connectionStrings = new ConcurrentDictionary<string, string>();

        public string GetConnectionString(string connectionStringName)
        {
            if (_connectionStrings.TryGetValue(connectionStringName, out var connectionString))
                return connectionString;
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json", optional: false,false);
            var configuration = builder.Build();
            var connectionStringFromFile = configuration.GetConnectionString(connectionStringName);
            _connectionStrings.TryAdd(connectionStringName, connectionStringFromFile);
            return connectionStringFromFile;
        }
    }
    //internal class CustomDbOptions : DbContextOptions
    //{
    //    public override Type ContextType => throw new NotImplementedException();

    //    public override DbContextOptions WithExtension<TExtension>([NotNullAttribute] TExtension extension)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

}
