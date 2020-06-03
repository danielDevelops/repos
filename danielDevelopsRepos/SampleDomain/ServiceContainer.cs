using danielDevelops.CommonInterfaces;
using danielDevelops.Service;
using SampleDomain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleDomain
{
    public class ServiceContainer : GenericServiceContainer, IServiceContainer
    {
        readonly ICurrentUser currentUser;
        public ServiceContainer(ICurrentUser currentUser)
            : base(new MyDatabase())
        {
            this.currentUser = currentUser;
        }
        public CourseService CourseService
            => GetService(() => new CourseService(context, currentUser.Username));

        public StudentService StudentService
            => GetService(() => new StudentService(context, currentUser.Username));
    }
}
