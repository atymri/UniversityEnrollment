using AutoMapper;
using FluentAssertions;
using Moq;
using UniversityEnrollment.Core.Common.Results;
using UniversityEnrollment.Core.Domain.Entities;
using UniversityEnrollment.Core.Domain.RepositoryContracts;
using UniversityEnrollment.Core.DTOs.RoleDTOs;
using UniversityEnrollment.Core.Services;

namespace UniversityEnrollment.Tests.ServiceTests
{
    public class RoleServiceTests
    {
        private readonly Mock<IRoleRepository> _roleRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly RoleService _roleService;

        public RoleServiceTests()
        {
            _roleRepositoryMock = new Mock<IRoleRepository>();
            _mapperMock = new Mock<IMapper>();
            _roleService = new RoleService(_roleRepositoryMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task CreateAsync_WhenRoleDoesNotExist_ShouldCreateRole()
        {
            // Arrange
            var createRoleDto = new CreateRoleDTO
            {
                Name = "Admin",
                Description = "Administrator role"
            };

            var role = new Role { Id = Guid.NewGuid(), Name = "Admin", Description = "Administrator role" };
            var roleDto = new RoleDTO { Id = role.Id, Name = "Admin", Description = "Administrator role" };

            _roleRepositoryMock.Setup(x => x.GetRoleByNameAsync(createRoleDto.Name))
                .ReturnsAsync((Role)null);

            _mapperMock.Setup(x => x.Map<Role>(createRoleDto))
                .Returns(role);

            _mapperMock.Setup(x => x.Map<RoleDTO>(role))
                .Returns(roleDto);

            // Act
            var result = await _roleService.CreateAsync(createRoleDto);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEquivalentTo(roleDto);
            _roleRepositoryMock.Verify(x => x.AddAsync(role), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_WhenRoleNameAlreadyExists_ShouldReturnFailure()
        {
            // Arrange
            var createRoleDto = new CreateRoleDTO { Name = "Admin" };
            var existingRole = new Role { Id = Guid.NewGuid(), Name = "Admin" };

            _roleRepositoryMock.Setup(x => x.GetRoleByNameAsync(createRoleDto.Name))
                .ReturnsAsync(existingRole);

            // Act
            var result = await _roleService.CreateAsync(createRoleDto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(RoleErrors.RoleAlreadyExists);
            _roleRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Role>()), Times.Never);
        }

        [Fact]
        public async Task GetByIdAsync_WhenRoleExists_ShouldReturnRole()
        {
            // Arrange
            var roleId = Guid.NewGuid();
            var role = new Role { Id = roleId, Name = "Admin", Description = "Administrator" };
            var roleDto = new RoleDTO { Id = roleId, Name = "Admin", Description = "Administrator" };

            _roleRepositoryMock.Setup(x => x.GetRoleWithUsersAsync(roleId))
                .ReturnsAsync(role);

            _mapperMock.Setup(x => x.Map<RoleDTO>(role))
                .Returns(roleDto);

            // Act
            var result = await _roleService.GetByIdAsync(roleId);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEquivalentTo(roleDto);
        }

        [Fact]
        public async Task GetByIdAsync_WhenRoleDoesNotExist_ShouldReturnFailure()
        {
            // Arrange
            var roleId = Guid.NewGuid();

            _roleRepositoryMock.Setup(x => x.GetRoleWithUsersAsync(roleId))
                .ReturnsAsync((Role)null);

            // Act
            var result = await _roleService.GetByIdAsync(roleId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(RoleErrors.RoleNotFound);
        }

        [Fact]
        public async Task GetByNameAsync_WhenRoleExists_ShouldReturnRole()
        {
            // Arrange
            var roleName = "Admin";
            var role = new Role { Id = Guid.NewGuid(), Name = roleName, Description = "Administrator" };
            var roleDto = new RoleDTO { Id = role.Id, Name = roleName, Description = "Administrator" };

            _roleRepositoryMock.Setup(x => x.GetRoleByNameAsync(roleName))
                .ReturnsAsync(role);

            _mapperMock.Setup(x => x.Map<RoleDTO>(role))
                .Returns(roleDto);

            // Act
            var result = await _roleService.GetByNameAsync(roleName);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEquivalentTo(roleDto);
        }

        [Fact]
        public async Task GetByNameAsync_WhenRoleDoesNotExist_ShouldReturnFailure()
        {
            // Arrange
            var roleName = "NonExistentRole";

            _roleRepositoryMock.Setup(x => x.GetRoleByNameAsync(roleName))
                .ReturnsAsync((Role)null);

            // Act
            var result = await _roleService.GetByNameAsync(roleName);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(RoleErrors.RoleNotFound);
        }

        [Fact]
        public async Task GetAllAsync_WhenRolesExist_ShouldReturnAllRoles()
        {
            // Arrange
            var roles = new List<Role>
            {
                new Role { Id = Guid.NewGuid(), Name = "Admin", Description = "Administrator" },
                new Role { Id = Guid.NewGuid(), Name = "Student", Description = "Student role" },
                new Role { Id = Guid.NewGuid(), Name = "Teacher", Description = "Teacher role" }
            };

            var roleDtos = new List<RoleDTO>
            {
                new RoleDTO { Id = roles[0].Id, Name = "Admin", Description = "Administrator" },
                new RoleDTO { Id = roles[1].Id, Name = "Student", Description = "Student role" },
                new RoleDTO { Id = roles[2].Id, Name = "Teacher", Description = "Teacher role" }
            };

            _roleRepositoryMock.Setup(x => x.GetAllAsync())
                .ReturnsAsync(roles);

            _mapperMock.Setup(x => x.Map<List<RoleDTO>>(roles))
                .Returns(roleDtos);

            // Act
            var result = await _roleService.GetAllAsync();

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().HaveCount(3);
            result.Value.Should().BeEquivalentTo(roleDtos);
        }

        [Fact]
        public async Task GetUserRolesAsync_WhenUserHasRoles_ShouldReturnUserRoles()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var roles = new List<Role>
            {
                new Role { Id = Guid.NewGuid(), Name = "Student", Description = "Student role" },
                new Role { Id = Guid.NewGuid(), Name = "Teacher", Description = "Teacher role" }
            };

            var roleDtos = new List<RoleDTO>
            {
                new RoleDTO { Id = roles[0].Id, Name = "Student", Description = "Student role" },
                new RoleDTO { Id = roles[1].Id, Name = "Teacher", Description = "Teacher role" }
            };

            _roleRepositoryMock.Setup(x => x.GetUserRolesAsync(userId))
                .ReturnsAsync(roles);

            _mapperMock.Setup(x => x.Map<List<RoleDTO>>(roles))
                .Returns(roleDtos);

            // Act
            var result = await _roleService.GetUserRolesAsync(userId);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().HaveCount(2);
            result.Value.Should().BeEquivalentTo(roleDtos);
        }

        [Fact]
        public async Task UpdateAsync_WhenRoleExists_ShouldUpdateRole()
        {
            // Arrange
            var updateRoleDto = new UpdateRoleDTO
            {
                Id = Guid.NewGuid(),
                Name = "UpdatedAdmin",
                Description = "Updated Administrator role"
            };

            var existingRole = new Role { Id = updateRoleDto.Id, Name = "Admin", Description = "Administrator" };
            var roleDto = new RoleDTO { Id = updateRoleDto.Id, Name = "UpdatedAdmin", Description = "Updated Administrator role" };

            _roleRepositoryMock.Setup(x => x.GetByIdAsync(updateRoleDto.Id))
                .ReturnsAsync(existingRole);

            _mapperMock.Setup(x => x.Map<RoleDTO>(existingRole))
                .Returns(roleDto);

            // Act
            var result = await _roleService.UpdateAsync(updateRoleDto);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEquivalentTo(roleDto);
            _mapperMock.Verify(x => x.Map(updateRoleDto, existingRole), Times.Once);
            _roleRepositoryMock.Verify(x => x.UpdateAsync(existingRole), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_WhenRoleDoesNotExist_ShouldReturnFailure()
        {
            // Arrange
            var updateRoleDto = new UpdateRoleDTO { Id = Guid.NewGuid() };

            _roleRepositoryMock.Setup(x => x.GetByIdAsync(updateRoleDto.Id))
                .ReturnsAsync((Role)null);

            // Act
            var result = await _roleService.UpdateAsync(updateRoleDto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(RoleErrors.RoleNotFound);
            _roleRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Role>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_WhenRoleExists_ShouldDeleteRole()
        {
            // Arrange
            var roleId = Guid.NewGuid();
            var role = new Role { Id = roleId, Name = "Admin" };

            _roleRepositoryMock.Setup(x => x.GetByIdAsync(roleId))
                .ReturnsAsync(role);

            // Act
            var result = await _roleService.DeleteAsync(roleId);

            // Assert
            result.IsSuccess.Should().BeTrue();
            _roleRepositoryMock.Verify(x => x.DeleteAsync(role), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_WhenRoleDoesNotExist_ShouldReturnFailure()
        {
            // Arrange
            var roleId = Guid.NewGuid();

            _roleRepositoryMock.Setup(x => x.GetByIdAsync(roleId))
                .ReturnsAsync((Role)null);

            // Act
            var result = await _roleService.DeleteAsync(roleId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(RoleErrors.RoleNotFound);
            _roleRepositoryMock.Verify(x => x.DeleteAsync(It.IsAny<Role>()), Times.Never);
        }

        [Fact]
        public async Task GetAllAsync_WhenNoRolesExist_ShouldReturnEmptyList()
        {
            // Arrange
            var emptyRoles = new List<Role>();
            var emptyRoleDtos = new List<RoleDTO>();

            _roleRepositoryMock.Setup(x => x.GetAllAsync())
                .ReturnsAsync(emptyRoles);

            _mapperMock.Setup(x => x.Map<List<RoleDTO>>(emptyRoles))
                .Returns(emptyRoleDtos);

            // Act
            var result = await _roleService.GetAllAsync();

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEmpty();
        }

        [Fact]
        public async Task GetUserRolesAsync_WhenUserHasNoRoles_ShouldReturnEmptyList()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var emptyRoles = new List<Role>();
            var emptyRoleDtos = new List<RoleDTO>();

            _roleRepositoryMock.Setup(x => x.GetUserRolesAsync(userId))
                .ReturnsAsync(emptyRoles);

            _mapperMock.Setup(x => x.Map<List<RoleDTO>>(emptyRoles))
                .Returns(emptyRoleDtos);

            // Act
            var result = await _roleService.GetUserRolesAsync(userId);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEmpty();
        }
    }
}