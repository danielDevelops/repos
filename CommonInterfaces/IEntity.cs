using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace danielDevelops.CommonInterfaces
{
    public interface IEntity<EntityKeyType> 
    {
        EntityKeyType Id { get; set;  }
    }
}
