using AutoMapper;
using UniversityEnrollment.Core.Common.Results;
using UniversityEnrollment.Core.Domain.Entities;
using UniversityEnrollment.Core.Domain.RepositoryContracts;
using UniversityEnrollment.Core.DTOs.EnrollmentDTOs;
using UniversityEnrollment.Core.ServiceContracts;

namespace UniversityEnrollment.Core.Services
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly IMapper _mapper;

        public EnrollmentService(IEnrollmentRepository enrollmentRepository, IMapper mapper)
        {
            _enrollmentRepository = enrollmentRepository;
            _mapper = mapper;
        }

        public async Task<Result<EnrollmentDTO>> CreateAsync(CreateEnrollmentDTO createEnrollmentDto)
        {
            var alreadyEnrolled = await _enrollmentRepository.IsUserAlreadyEnrolledAsync(
                createEnrollmentDto.UserId,
                createEnrollmentDto.CourseId
            );

            if (alreadyEnrolled)
                return Result.Failure<EnrollmentDTO>(EnrollmentErrors.AlreadyEnrolled);

            var enrollment = _mapper.Map<Enrollment>(createEnrollmentDto);
            await _enrollmentRepository.AddAsync(enrollment);

            var enrollmentDto = _mapper.Map<EnrollmentDTO>(enrollment);
            return Result.Success(enrollmentDto);
        }

        public async Task<Result<EnrollmentDTO>> GetByIdAsync(Guid id)
        {
            var enrollment = await _enrollmentRepository.GetByIdAsync(id);
            if (enrollment == null)
                return Result.Failure<EnrollmentDTO>(EnrollmentErrors.EnrollmentNotFound);

            var enrollmentDto = _mapper.Map<EnrollmentDTO>(enrollment);
            return Result.Success(enrollmentDto);
        }

        public async Task<Result<List<EnrollmentDTO>>> GetAllAsync()
        {
            var enrollments = await _enrollmentRepository.GetAllAsync();
            var enrollmentDtos = _mapper.Map<List<EnrollmentDTO>>(enrollments);
            return Result.Success(enrollmentDtos);
        }

        public async Task<Result> DeleteAsync(Guid id)
        {
            var enrollment = await _enrollmentRepository.GetByIdAsync(id);
            if (enrollment == null)
                return Result.Failure(EnrollmentErrors.EnrollmentNotFound);

            await _enrollmentRepository.DeleteAsync(enrollment);
            return Result.Success();
        }
    }
}
