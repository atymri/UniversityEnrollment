using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniversityEnrollment.Core.Domain.Entities;

namespace UniversityEnrollment.Core.Domain.RepositoryContracts
{
    public interface IEnrollmentRepository : IRepository<Enrollment>
    {
        Task<List<Enrollment>?> GetEnrollmentsByUserAsync(Guid userId);
        Task<List<Enrollment>?> GetEnrollmentsByCourseAsync(Guid courseId);

        Task<bool> IsUserAlreadyEnrolledAsync(Guid userId, Guid courseId);
        Task<int> GetTotalUnitsForUserAsync(Guid userId);
    }
}
