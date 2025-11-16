using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversityEnrollment.Core.DTOs.EnrollmentDTOs
{
    public class CreateEnrollmentDTO
    {
        public Guid UserId { get; set; }
        public Guid CourseId { get; set; }
    }
}
