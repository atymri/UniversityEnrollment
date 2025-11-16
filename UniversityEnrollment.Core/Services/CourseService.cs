using AutoMapper;
using UniversityEnrollment.Core.Common.Results;
using UniversityEnrollment.Core.Domain.Entities;
using UniversityEnrollment.Core.Domain.RepositoryContracts;
using UniversityEnrollment.Core.DTOs.CourseDTOs;
using UniversityEnrollment.Core.ServiceContracts;

namespace UniversityEnrollment.Core.Services
{
    public class CourseService : ICourseService
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IMapper _mapper;

        public CourseService(ICourseRepository courseRepository, IMapper mapper)
        {
            _courseRepository = courseRepository;
            _mapper = mapper;
        }

        public async Task<Result<CourseDTO>> CreateAsync(CreateCourseDTO createCourseDto)
        {
            var exists = await _courseRepository.ExistsAsync(c => c.CourseCode == createCourseDto.CourseCode);
            if (exists)
                return Result.Failure<CourseDTO>(CourseErrors.CourseAlreadyExists);

            var course = _mapper.Map<Course>(createCourseDto);
            await _courseRepository.AddAsync(course);

            var courseDto = _mapper.Map<CourseDTO>(course);
            return Result.Success(courseDto);
        }

        public async Task<Result<CourseDTO>> GetByIdAsync(Guid id)
        {
            var course = await _courseRepository.GetByIdAsync(id);
            if (course == null)
                return Result.Failure<CourseDTO>(CourseErrors.CourseNotFound);

            var courseDto = _mapper.Map<CourseDTO>(course);
            return Result.Success(courseDto);
        }

        public async Task<Result<List<CourseDTO>>> GetAllAsync()
        {
            var courses = await _courseRepository.GetAllAsync();
            var courseDTOs = _mapper.Map<List<CourseDTO>>(courses);
            return Result.Success(courseDTOs);
        }

        public async Task<Result<CourseDTO>> UpdateAsync(UpdateCourseDTO updateCourseDto)
        {
            var course = await _courseRepository.GetByIdAsync(updateCourseDto.Id);
            if (course == null)
                return Result.Failure<CourseDTO>(CourseErrors.CourseNotFound);

            _mapper.Map(updateCourseDto, course);
            await _courseRepository.UpdateAsync(course);

            var courseDto = _mapper.Map<CourseDTO>(course);
            return Result.Success(courseDto);
        }

        public async Task<Result> DeleteAsync(Guid id)
        {
            var course = await _courseRepository.GetByIdAsync(id);
            if (course == null)
                return Result.Failure(CourseErrors.CourseNotFound);

            await _courseRepository.DeleteAsync(course);
            return Result.Success();
        }
    }
}
