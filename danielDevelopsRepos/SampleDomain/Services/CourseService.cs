using danielDevelops.CommonInterfaces.Infrastructure;
using SampleModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleDomain.Services
{
    public class CourseService : BaseService<Course>
    {
        public CourseService(ICustomContext context, string currentUsername) 
            : base(context, currentUsername)
        {
        }

        public async Task<IEnumerable<Course>> GetCoursesAsync()
            => await Repo.GetAsync();

        public async Task<Course> GetCourseByIdAsync(int id)
            => await Repo.GetByIDAsync(id);

        public async Task<SqlResult<Course>> CreateCourseAsync(string courseName)
        {
            var newCourse = new Course
            {
                CourseName = courseName
            };
            Repo.Insert(newCourse);
            return await SaveAsync(newCourse);
        }

        public async Task<SqlResult<Course>> AddStudentsToCourseAsync(int courseId, IEnumerable<Student> students)
        {
            var course = await Repo.GetByIDAsync(courseId);
            foreach (var item in students)
            {
                course.Students.Add(item);
            }
            return await SaveAsync(course);
        }
    }
}
