using AutoMapper;
using FluentAssertions;
using Moq;
using UniversityEnrollment.Core.Common.Results;
using UniversityEnrollment.Core.Domain.Entities;
using UniversityEnrollment.Core.Domain.RepositoryContracts;
using UniversityEnrollment.Core.DTOs.EnrollmentDTOs;
using UniversityEnrollment.Core.Services;

namespace UniversityEnrollment.Tests.ServiceTests
{
    public class EnrollmentServiceTests
    {
        private readonly Mock<IEnrollmentRepository> _enrollmentRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly EnrollmentService _enrollmentService;

        public EnrollmentServiceTests()
        {
            _enrollmentRepositoryMock = new Mock<IEnrollmentRepository>();
            _mapperMock = new Mock<IMapper>();
            _enrollmentService = new EnrollmentService(_enrollmentRepositoryMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task CreateAsync_WhenUserNotAlreadyEnrolled_ShouldCreateEnrollment()
        {
            // Arrange
            var createEnrollmentDto = new CreateEnrollmentDTO
            {
                UserId = Guid.NewGuid(),
                CourseId = Guid.NewGuid()
            };

            var enrollment = new Enrollment
            {
                Id = Guid.NewGuid(),
                UserId = createEnrollmentDto.UserId,
                CourseId = createEnrollmentDto.CourseId,
                EnrollmentDate = DateTime.UtcNow
            };

            var enrollmentDto = new EnrollmentDTO
            {
                Id = enrollment.Id,
                UserId = enrollment.UserId,
                CourseId = enrollment.CourseId,
                EnrollmentDate = enrollment.EnrollmentDate
            };

            _enrollmentRepositoryMock
                .Setup(x => x.IsUserAlreadyEnrolledAsync(createEnrollmentDto.UserId, createEnrollmentDto.CourseId))
                .ReturnsAsync(false);

            _mapperMock
                .Setup(x => x.Map<Enrollment>(createEnrollmentDto))
                .Returns(enrollment);

            _mapperMock
                .Setup(x => x.Map<EnrollmentDTO>(enrollment))
                .Returns(enrollmentDto);

            // Act
            var result = await _enrollmentService.CreateAsync(createEnrollmentDto);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEquivalentTo(enrollmentDto);
            _enrollmentRepositoryMock.Verify(x => x.AddAsync(enrollment), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_WhenUserAlreadyEnrolled_ShouldReturnFailure()
        {
            // Arrange
            var createEnrollmentDto = new CreateEnrollmentDTO
            {
                UserId = Guid.NewGuid(),
                CourseId = Guid.NewGuid()
            };

            _enrollmentRepositoryMock
                .Setup(x => x.IsUserAlreadyEnrolledAsync(createEnrollmentDto.UserId, createEnrollmentDto.CourseId))
                .ReturnsAsync(true);

            // Act
            var result = await _enrollmentService.CreateAsync(createEnrollmentDto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(EnrollmentErrors.AlreadyEnrolled);
            _enrollmentRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Enrollment>()), Times.Never);
        }

        [Fact]
        public async Task GetByIdAsync_WhenEnrollmentExists_ShouldReturnEnrollment()
        {
            // Arrange
            var enrollmentId = Guid.NewGuid();
            var enrollment = new Enrollment
            {
                Id = enrollmentId,
                UserId = Guid.NewGuid(),
                CourseId = Guid.NewGuid(),
                EnrollmentDate = DateTime.UtcNow
            };

            var enrollmentDto = new EnrollmentDTO
            {
                Id = enrollmentId,
                UserId = enrollment.UserId,
                CourseId = enrollment.CourseId,
                EnrollmentDate = enrollment.EnrollmentDate
            };

            _enrollmentRepositoryMock
                .Setup(x => x.GetByIdAsync(enrollmentId))
                .ReturnsAsync(enrollment);

            _mapperMock
                .Setup(x => x.Map<EnrollmentDTO>(enrollment))
                .Returns(enrollmentDto);

            // Act
            var result = await _enrollmentService.GetByIdAsync(enrollmentId);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEquivalentTo(enrollmentDto);
        }

        [Fact]
        public async Task GetByIdAsync_WhenEnrollmentDoesNotExist_ShouldReturnFailure()
        {
            // Arrange
            var enrollmentId = Guid.NewGuid();

            _enrollmentRepositoryMock
                .Setup(x => x.GetByIdAsync(enrollmentId))
                .ReturnsAsync((Enrollment)null);

            // Act
            var result = await _enrollmentService.GetByIdAsync(enrollmentId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(EnrollmentErrors.EnrollmentNotFound);
        }

        [Fact]
        public async Task GetAllAsync_WhenEnrollmentsExist_ShouldReturnAllEnrollments()
        {
            // Arrange
            var enrollments = new List<Enrollment>
            {
                new Enrollment
                {
                    Id = Guid.NewGuid(),
                    UserId = Guid.NewGuid(),
                    CourseId = Guid.NewGuid(),
                    EnrollmentDate = DateTime.UtcNow
                },
                new Enrollment
                {
                    Id = Guid.NewGuid(),
                    UserId = Guid.NewGuid(),
                    CourseId = Guid.NewGuid(),
                    EnrollmentDate = DateTime.UtcNow
                }
            };

            var enrollmentDtos = new List<EnrollmentDTO>
            {
                new EnrollmentDTO
                {
                    Id = enrollments[0].Id,
                    UserId = enrollments[0].UserId,
                    CourseId = enrollments[0].CourseId,
                    EnrollmentDate = enrollments[0].EnrollmentDate
                },
                new EnrollmentDTO
                {
                    Id = enrollments[1].Id,
                    UserId = enrollments[1].UserId,
                    CourseId = enrollments[1].CourseId,
                    EnrollmentDate = enrollments[1].EnrollmentDate
                }
            };

            _enrollmentRepositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(enrollments);

            _mapperMock
                .Setup(x => x.Map<List<EnrollmentDTO>>(enrollments))
                .Returns(enrollmentDtos);

            // Act
            var result = await _enrollmentService.GetAllAsync();

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().HaveCount(2);
            result.Value.Should().BeEquivalentTo(enrollmentDtos);
        }

        [Fact]
        public async Task DeleteAsync_WhenEnrollmentExists_ShouldDeleteEnrollment()
        {
            // Arrange
            var enrollmentId = Guid.NewGuid();
            var enrollment = new Enrollment
            {
                Id = enrollmentId,
                UserId = Guid.NewGuid(),
                CourseId = Guid.NewGuid()
            };

            _enrollmentRepositoryMock
                .Setup(x => x.GetByIdAsync(enrollmentId))
                .ReturnsAsync(enrollment);

            // Act
            var result = await _enrollmentService.DeleteAsync(enrollmentId);

            // Assert
            result.IsSuccess.Should().BeTrue();
            _enrollmentRepositoryMock.Verify(x => x.DeleteAsync(enrollment), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_WhenEnrollmentDoesNotExist_ShouldReturnFailure()
        {
            // Arrange
            var enrollmentId = Guid.NewGuid();

            _enrollmentRepositoryMock
                .Setup(x => x.GetByIdAsync(enrollmentId))
                .ReturnsAsync((Enrollment)null);

            // Act
            var result = await _enrollmentService.DeleteAsync(enrollmentId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(EnrollmentErrors.EnrollmentNotFound);
            _enrollmentRepositoryMock.Verify(x => x.DeleteAsync(It.IsAny<Enrollment>()), Times.Never);
        }

        [Fact]
        public async Task GetAllAsync_WhenNoEnrollmentsExist_ShouldReturnEmptyList()
        {
            // Arrange
            var emptyEnrollments = new List<Enrollment>();
            var emptyEnrollmentDtos = new List<EnrollmentDTO>();

            _enrollmentRepositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(emptyEnrollments);

            _mapperMock
                .Setup(x => x.Map<List<EnrollmentDTO>>(emptyEnrollments))
                .Returns(emptyEnrollmentDtos);

            // Act
            var result = await _enrollmentService.GetAllAsync();

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEmpty();
        }

        [Fact]
        public async Task CreateAsync_ShouldCheckForDuplicateEnrollment()
        {
            // Arrange
            var createEnrollmentDto = new CreateEnrollmentDTO
            {
                UserId = Guid.NewGuid(),
                CourseId = Guid.NewGuid()
            };

            var enrollment = new Enrollment
            {
                Id = Guid.NewGuid(),
                UserId = createEnrollmentDto.UserId,
                CourseId = createEnrollmentDto.CourseId
            };

            var enrollmentDto = new EnrollmentDTO
            {
                Id = enrollment.Id,
                UserId = enrollment.UserId,
                CourseId = enrollment.CourseId
            };

            _enrollmentRepositoryMock
                .Setup(x => x.IsUserAlreadyEnrolledAsync(createEnrollmentDto.UserId, createEnrollmentDto.CourseId))
                .ReturnsAsync(false);

            _mapperMock
                .Setup(x => x.Map<Enrollment>(createEnrollmentDto))
                .Returns(enrollment);

            _mapperMock
                .Setup(x => x.Map<EnrollmentDTO>(enrollment))
                .Returns(enrollmentDto);

            // Act
            var result = await _enrollmentService.CreateAsync(createEnrollmentDto);

            // Assert
            _enrollmentRepositoryMock.Verify(
                x => x.IsUserAlreadyEnrolledAsync(createEnrollmentDto.UserId, createEnrollmentDto.CourseId),
                Times.Once
            );
        }
    }
}