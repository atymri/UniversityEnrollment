using AutoMapper;
using FluentAssertions;
using Moq;
using UniversityEnrollment.Core.Common.Results;
using UniversityEnrollment.Core.Domain.Entities;
using UniversityEnrollment.Core.Domain.RepositoryContracts;
using UniversityEnrollment.Core.DTOs.CourseDTOs;
using UniversityEnrollment.Core.Services;
using System.Linq.Expressions;

namespace UniversityEnrollment.Tests.ServiceTests
{
    public class CourseServiceTests
    {
        private readonly Mock<ICourseRepository> _courseRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly CourseService _courseService;

        public CourseServiceTests()
        {
            _courseRepositoryMock = new Mock<ICourseRepository>();
            _mapperMock = new Mock<IMapper>();
            _courseService = new CourseService(_courseRepositoryMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task CreateAsync_WhenCourseDoesNotExist_ShouldCreateCourse()
        {
            // Arrange
            var createCourseDto = new CreateCourseDTO
            {
                CourseCode = "CS101",
                Title = "Introduction to Computer Science",
                Units = 3
            };

            var course = new Course
            {
                Id = Guid.NewGuid(),
                CourseCode = "CS101",
                Title = "Introduction to Computer Science",
                Units = 3
            };

            var courseDto = new CourseDTO
            {
                Id = course.Id,
                CourseCode = "CS101",
                Title = "Introduction to Computer Science",
                Units = 3
            };

            _courseRepositoryMock
                .Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<Course, bool>>>()))
                .ReturnsAsync(false);

            _mapperMock
                .Setup(mapper => mapper.Map<Course>(createCourseDto))
                .Returns(course);

            _mapperMock
                .Setup(mapper => mapper.Map<CourseDTO>(course))
                .Returns(courseDto);

            // Act
            var result = await _courseService.CreateAsync(createCourseDto);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEquivalentTo(courseDto);
            _courseRepositoryMock.Verify(repo => repo.AddAsync(course), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_WhenCourseCodeAlreadyExists_ShouldReturnFailure()
        {
            // Arrange
            var createCourseDto = new CreateCourseDTO
            {
                CourseCode = "CS101",
                Title = "Computer Science",
                Units = 3
            };

            _courseRepositoryMock
                .Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<Course, bool>>>()))
                .ReturnsAsync(true);

            // Act
            var result = await _courseService.CreateAsync(createCourseDto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(CourseErrors.CourseAlreadyExists);
            _courseRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Course>()), Times.Never);
        }

        [Fact]
        public async Task GetByIdAsync_WhenCourseExists_ShouldReturnCourse()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var course = new Course
            {
                Id = courseId,
                CourseCode = "MATH101",
                Title = "Calculus I",
                Units = 4
            };

            var courseDto = new CourseDTO
            {
                Id = courseId,
                CourseCode = "MATH101",
                Title = "Calculus I",
                Units = 4
            };

            _courseRepositoryMock
                .Setup(repo => repo.GetByIdAsync(courseId))
                .ReturnsAsync(course);

            _mapperMock
                .Setup(mapper => mapper.Map<CourseDTO>(course))
                .Returns(courseDto);

            // Act
            var result = await _courseService.GetByIdAsync(courseId);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEquivalentTo(courseDto);
        }

        [Fact]
        public async Task GetByIdAsync_WhenCourseDoesNotExist_ShouldReturnFailure()
        {
            // Arrange
            var courseId = Guid.NewGuid();

            _courseRepositoryMock
                .Setup(repo => repo.GetByIdAsync(courseId))
                .ReturnsAsync((Course)null);

            // Act
            var result = await _courseService.GetByIdAsync(courseId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(CourseErrors.CourseNotFound);
        }

        [Fact]
        public async Task GetAllAsync_WhenCoursesExist_ShouldReturnAllCourses()
        {
            // Arrange
            var courses = new List<Course>
            {
                new Course
                {
                    Id = Guid.NewGuid(),
                    CourseCode = "CS101",
                    Title = "Computer Science",
                    Units = 3
                },
                new Course
                {
                    Id = Guid.NewGuid(),
                    CourseCode = "MATH101",
                    Title = "Mathematics",
                    Units = 4
                }
            };

            var courseDTOs = new List<CourseDTO>
            {
                new CourseDTO
                {
                    Id = courses[0].Id,
                    CourseCode = "CS101",
                    Title = "Computer Science",
                    Units = 3
                },
                new CourseDTO
                {
                    Id = courses[1].Id,
                    CourseCode = "MATH101",
                    Title = "Mathematics",
                    Units = 4
                }
            };

            _courseRepositoryMock
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(courses);

            _mapperMock
                .Setup(mapper => mapper.Map<List<CourseDTO>>(courses))
                .Returns(courseDTOs);

            // Act
            var result = await _courseService.GetAllAsync();

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().HaveCount(2);
            result.Value.Should().BeEquivalentTo(courseDTOs);
        }

        [Fact]
        public async Task UpdateAsync_WhenCourseExists_ShouldUpdateCourse()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var updateCourseDto = new UpdateCourseDTO
            {
                Id = courseId,
                CourseCode = "CS102",
                Title = "Advanced Computer Science",
                Units = 4
            };

            var existingCourse = new Course
            {
                Id = courseId,
                CourseCode = "CS101",
                Title = "Computer Science",
                Units = 3
            };

            var updatedCourseDto = new CourseDTO
            {
                Id = courseId,
                CourseCode = "CS102",
                Title = "Advanced Computer Science",
                Units = 4
            };

            _courseRepositoryMock
                .Setup(repo => repo.GetByIdAsync(courseId))
                .ReturnsAsync(existingCourse);

            _mapperMock
                .Setup(mapper => mapper.Map<CourseDTO>(existingCourse))
                .Returns(updatedCourseDto);

            // Act
            var result = await _courseService.UpdateAsync(updateCourseDto);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEquivalentTo(updatedCourseDto);
            _mapperMock.Verify(mapper => mapper.Map(updateCourseDto, existingCourse), Times.Once);
            _courseRepositoryMock.Verify(repo => repo.UpdateAsync(existingCourse), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_WhenCourseDoesNotExist_ShouldReturnFailure()
        {
            // Arrange
            var updateCourseDto = new UpdateCourseDTO
            {
                Id = Guid.NewGuid(),
                CourseCode = "CS101",
                Title = "Computer Science",
                Units = 3
            };

            _courseRepositoryMock
                .Setup(repo => repo.GetByIdAsync(updateCourseDto.Id))
                .ReturnsAsync((Course)null);

            // Act
            var result = await _courseService.UpdateAsync(updateCourseDto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(CourseErrors.CourseNotFound);
        }

        [Fact]
        public async Task DeleteAsync_WhenCourseExists_ShouldDeleteCourse()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var course = new Course
            {
                Id = courseId,
                CourseCode = "CS101",
                Title = "Computer Science",
                Units = 3
            };

            _courseRepositoryMock
                .Setup(repo => repo.GetByIdAsync(courseId))
                .ReturnsAsync(course);

            // Act
            var result = await _courseService.DeleteAsync(courseId);

            // Assert
            result.IsSuccess.Should().BeTrue();
            _courseRepositoryMock.Verify(repo => repo.DeleteAsync(course), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_WhenCourseDoesNotExist_ShouldReturnFailure()
        {
            // Arrange
            var courseId = Guid.NewGuid();

            _courseRepositoryMock
                .Setup(repo => repo.GetByIdAsync(courseId))
                .ReturnsAsync((Course)null);

            // Act
            var result = await _courseService.DeleteAsync(courseId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(CourseErrors.CourseNotFound);
        }

        [Fact]
        public async Task GetAllAsync_WhenNoCoursesExist_ShouldReturnEmptyList()
        {
            // Arrange
            var emptyCourses = new List<Course>();
            var emptyCourseDTOs = new List<CourseDTO>();

            _courseRepositoryMock
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(emptyCourses);

            _mapperMock
                .Setup(mapper => mapper.Map<List<CourseDTO>>(emptyCourses))
                .Returns(emptyCourseDTOs);

            // Act
            var result = await _courseService.GetAllAsync();

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEmpty();
        }
    }
}