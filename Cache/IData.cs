using danielDevelops.CommonInterfaces.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace danielDevelops.Cache
{
    public interface IData<T>
    {
        Task<T> GetAsync(ICustomContext context, bool forceReload = false);
        Task ReloadDataAsync(ICustomContext context);
        DateTime Updated { get; }
        bool IsExpired { get; }
    }
}
