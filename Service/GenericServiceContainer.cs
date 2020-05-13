using danielDevelops.CommonInterfaces.Infrastructure;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace danielDevelops.Service
{
    public abstract class GenericServiceContainer : IDisposable
    {
        protected readonly ConcurrentDictionary<string, dynamic> Services = new ConcurrentDictionary<string, dynamic>();
        protected readonly ICustomContext context;
        protected GenericServiceContainer(ICustomContext context)
        {
            this.context = context;
        }

        protected ServiceType GetService<ServiceType>(Func<ServiceType> createService)
        {
            var serviceName = typeof(ServiceType).Name;
            var service = Services.GetOrAdd(serviceName, createService());
            return service;
        }

        public void Dispose()
        {

            foreach (var item in Services)
            {
                item.Value.Dispose();
            }
            if (context != null && !context.IsDisposed)
                context.Dispose();
        }
    }
}
