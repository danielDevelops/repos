using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleDomain
{
    public interface IServiceContainer
    {
        Services.CourseService CourseService { get; }
        Services.StudentService StudentService { get; }
    }
}
