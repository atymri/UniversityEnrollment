using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniversityEnrollment.Core.Common.Results;
using UniversityEnrollment.Core.DTOs.CourseDTOs;

namespace UniversityEnrollment.Core.ServiceContracts
{
    public interface ICourseService
    {
        Task<Result<CourseDTO>?> GetByIdAsync(Guid id);
        Task<Result<List<CourseDTO>>?> GetAllAsync();
        Task<Result<CourseDTO>?> CreateAsync(CreateCourseDTO createCourseDto);
        Task<Result<CourseDTO>?> UpdateAsync(UpdateCourseDTO updateCourseDto);
        Task<Result> DeleteAsync(Guid id);
    }
}
