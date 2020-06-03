using danielDevelops.CommonInterfaces.Infrastructure;
using danielDevelops.Service;
using SampleModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleDomain.Services
{
    public class StudentService : BaseService<Student>
    {
        public StudentService(ICustomContext context, string currentUsername)
            : base(context, currentUsername)
        {
        }

        public async Task<IEnumerable<Student>> GetStudentsAsync()
            => await Repo.GetAsync();

        public async Task<IEnumerable<Student>> GetStudentsByLastNameAsync(string lastName)
            => await Repo.GetAsync(t => t.LastName == lastName);

        public async Task<SqlResult<Student>> CreateStudent(string firstName, string lastName)
        {
            var newStudent = new Student
            {
                FirstName = firstName,
                LastName = lastName
            };
            Repo.Insert(newStudent);
            return await SaveAsync(newStudent);
        }
    }
}
