using AutoMapper;
using FluentAssertions;
using Moq;
using UniversityEnrollment.Core.Common.Results;
using UniversityEnrollment.Core.Domain.Entities;
using UniversityEnrollment.Core.Domain.RepositoryContracts;
using UniversityEnrollment.Core.DTOs.UserDTOs;
using UniversityEnrollment.Core.Services;

namespace UniversityEnrollment.Tests.ServiceTests
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _mapperMock = new Mock<IMapper>();
            _userService = new UserService(_userRepositoryMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task CreateAsync_WhenEmailIsNotDuplicated_ShouldCreateUser()
        {
            // Arrange
            var createUserDto = new CreateUserDTO
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                PhoneNumber = "1234567890",
                BirthDate = new DateTime(1990, 1, 1),
                RoleId = Guid.NewGuid()
            };

            var user = new User { Id = Guid.NewGuid() };
            var userDto = new UserDTO { Id = user.Id };

            _userRepositoryMock.Setup(x => x.IsEmailDuplicatedAsync(createUserDto.Email))
                .ReturnsAsync(false);

            _mapperMock.Setup(x => x.Map<User>(createUserDto))
                .Returns(user);

            _mapperMock.Setup(x => x.Map<UserDTO>(user))
                .Returns(userDto);

            // Act
            var result = await _userService.CreateAsync(createUserDto);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEquivalentTo(userDto);
            _userRepositoryMock.Verify(x => x.AddAsync(user), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_WhenEmailIsDuplicated_ShouldReturnFailure()
        {
            // Arrange
            var createUserDto = new CreateUserDTO
            {
                Email = "duplicate@example.com"
            };

            _userRepositoryMock.Setup(x => x.IsEmailDuplicatedAsync(createUserDto.Email))
                .ReturnsAsync(true);

            // Act
            var result = await _userService.CreateAsync(createUserDto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(UserErrors.EmailAlreadyExists);
            _userRepositoryMock.Verify(x => x.AddAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task GetByIdAsync_WhenUserExists_ShouldReturnUser()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User { Id = userId };
            var userDto = new UserDTO { Id = userId };

            _userRepositoryMock.Setup(x => x.GetUserWithEnrollmentsAsync(userId))
                .ReturnsAsync(user);

            _mapperMock.Setup(x => x.Map<UserDTO>(user))
                .Returns(userDto);

            // Act
            var result = await _userService.GetByIdAsync(userId);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEquivalentTo(userDto);
        }

        [Fact]
        public async Task GetByIdAsync_WhenUserDoesNotExist_ShouldReturnFailure()
        {
            // Arrange
            var userId = Guid.NewGuid();

            _userRepositoryMock.Setup(x => x.GetUserWithEnrollmentsAsync(userId))
                .ReturnsAsync((User)null);

            // Act
            var result = await _userService.GetByIdAsync(userId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(UserErrors.UserNotFound);
        }

        [Fact]
        public async Task GetAllAsync_WhenUsersExist_ShouldReturnAllUsers()
        {
            // Arrange
            var users = new List<User>
            {
                new User { Id = Guid.NewGuid() },
                new User { Id = Guid.NewGuid() }
            };

            var userDtos = new List<UserDTO>
            {
                new UserDTO { Id = users[0].Id },
                new UserDTO { Id = users[1].Id }
            };

            _userRepositoryMock.Setup(x => x.GetAllAsync())
                .ReturnsAsync(users);

            _mapperMock.Setup(x => x.Map<List<UserDTO>>(users))
                .Returns(userDtos);

            // Act
            var result = await _userService.GetAllAsync();

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().HaveCount(2);
            result.Value.Should().BeEquivalentTo(userDtos);
        }

        [Fact]
        public async Task UpdateAsync_WhenUserExists_ShouldUpdateUser()
        {
            // Arrange
            var updateUserDto = new UpdateUserDTO
            {
                Id = Guid.NewGuid(),
                FirstName = "Updated",
                LastName = "Name",
                Email = "updated@example.com",
                PhoneNumber = "0987654321",
                BirthDate = new DateTime(1995, 1, 1),
                RoleId = Guid.NewGuid()
            };

            var existingUser = new User { Id = updateUserDto.Id };
            var userDto = new UserDTO { Id = updateUserDto.Id };

            _userRepositoryMock.Setup(x => x.GetByIdAsync(updateUserDto.Id))
                .ReturnsAsync(existingUser);

            _mapperMock.Setup(x => x.Map<UserDTO>(existingUser))
                .Returns(userDto);

            // Act
            var result = await _userService.UpdateAsync(updateUserDto);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEquivalentTo(userDto);
            _mapperMock.Verify(x => x.Map(updateUserDto, existingUser), Times.Once);
            _userRepositoryMock.Verify(x => x.UpdateAsync(existingUser), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_WhenUserDoesNotExist_ShouldReturnFailure()
        {
            // Arrange
            var updateUserDto = new UpdateUserDTO { Id = Guid.NewGuid() };

            _userRepositoryMock.Setup(x => x.GetByIdAsync(updateUserDto.Id))
                .ReturnsAsync((User)null);

            // Act
            var result = await _userService.UpdateAsync(updateUserDto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(UserErrors.UserNotFound);
            _userRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task AddUserInRole_WhenUserExists_ShouldAddRole()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var roleId = Guid.NewGuid();
            var user = new User { Id = userId };
            var updatedUser = new User { Id = userId };
            var userDto = new UserDTO { Id = userId };

            _userRepositoryMock.Setup(x => x.GetByIdAsync(userId))
                .ReturnsAsync(user);

            _userRepositoryMock.Setup(x => x.AddInRole(userId, roleId))
                .ReturnsAsync(updatedUser);

            _mapperMock.Setup(x => x.Map<UserDTO>(updatedUser))
                .Returns(userDto);

            // Act
            var result = await _userService.AddUserInRole(userId, roleId);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEquivalentTo(userDto);
            _userRepositoryMock.Verify(x => x.AddInRole(userId, roleId), Times.Once);
        }

        [Fact]
        public async Task AddUserInRole_WhenUserDoesNotExist_ShouldReturnFailure()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var roleId = Guid.NewGuid();

            _userRepositoryMock.Setup(x => x.GetByIdAsync(userId))
                .ReturnsAsync((User)null);

            // Act
            var result = await _userService.AddUserInRole(userId, roleId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(UserErrors.UserNotFound);
            _userRepositoryMock.Verify(x => x.AddInRole(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_WhenUserExists_ShouldDeleteUser()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User { Id = userId };

            _userRepositoryMock.Setup(x => x.GetByIdAsync(userId))
                .ReturnsAsync(user);

            // Act
            var result = await _userService.DeleteAsync(userId);

            // Assert
            result.IsSuccess.Should().BeTrue();
            _userRepositoryMock.Verify(x => x.DeleteAsync(user), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_WhenUserDoesNotExist_ShouldReturnFailure()
        {
            // Arrange
            var userId = Guid.NewGuid();

            _userRepositoryMock.Setup(x => x.GetByIdAsync(userId))
                .ReturnsAsync((User)null);

            // Act
            var result = await _userService.DeleteAsync(userId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(UserErrors.UserNotFound);
            _userRepositoryMock.Verify(x => x.DeleteAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task GetAllAsync_WhenNoUsersExist_ShouldReturnEmptyList()
        {
            // Arrange
            var emptyUsers = new List<User>();
            var emptyUserDtos = new List<UserDTO>();

            _userRepositoryMock.Setup(x => x.GetAllAsync())
                .ReturnsAsync(emptyUsers);

            _mapperMock.Setup(x => x.Map<List<UserDTO>>(emptyUsers))
                .Returns(emptyUserDtos);

            // Act
            var result = await _userService.GetAllAsync();

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEmpty();
        }
    }
}   