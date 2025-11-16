using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversityEnrollment.Core.Domain.Entities
{
    public class BaseEntity<T> where T : notnull
    {
        [Key]
        public T Id { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [Required]
        public DateTime UpdatedAt { get; set; }
        public void Touch() => UpdatedAt = DateTime.UtcNow;
    }
}
