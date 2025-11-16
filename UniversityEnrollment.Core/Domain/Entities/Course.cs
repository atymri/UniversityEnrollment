using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UniversityEnrollment.Core.Domain.Entities
{
    public class Course : BaseEntity<Guid>
    {
        [Required]
        [StringLength(20)]
        public string CourseCode { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [Range(1, 10)]
        public int Units { get; set; }

        public List<Enrollment> Enrollments { get; set; } = new();
    }
}
