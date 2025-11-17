using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using UniversityEnrollment.Core.Domain.Entities;

namespace UniversityEnrollment.Core.Domain.RepositoryContracts
{
    public interface ICourseRepository : IRepository<Course>
    {

        Task<Course?> GetCourseByCodeAsync(string courseCode);
        Task<List<Course?>> GetCoursesWithEnrollmentsAsync();
        Task<bool> IsCourseCodeDuplicatedAsync(string courseCode);
    }
}
