using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniversityEnrollment.Core.Common.Results;
using UniversityEnrollment.Core.DTOs.EnrollmentDTOs;

namespace UniversityEnrollment.Core.ServiceContracts
{
    public interface IEnrollmentService
    {
        Task<Result<EnrollmentDTO>> GetByIdAsync(Guid id);
        Task<Result<List<EnrollmentDTO>>> GetAllAsync();
        Task<Result<EnrollmentDTO>> CreateAsync(CreateEnrollmentDTO createEnrollmentDto);
        Task<Result> DeleteAsync(Guid id);
    }
}
