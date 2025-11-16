using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversityEnrollment.Core.DTOs.EnrollmentDTOs
{
    public class EnrollmentDTO
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public string UserFullName { get; set; } = string.Empty;

        public Guid CourseId { get; set; }
        public string CourseTitle { get; set; } = string.Empty;

        public DateTime EnrollmentDate { get; set; }
    }
}
