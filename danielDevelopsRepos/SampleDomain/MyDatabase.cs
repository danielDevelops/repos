using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleDomain
{
    internal sealed class MyDatabase
        : danielDevelops.Infrastructure.CustomContext
    {
        public MyDatabase() 
            : base(nameof(MyDatabase))
        {
        }
    }
}
