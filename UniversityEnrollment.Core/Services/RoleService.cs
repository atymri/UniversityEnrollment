using AutoMapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniversityEnrollment.Core.Common.Results;
using UniversityEnrollment.Core.Domain.Entities;
using UniversityEnrollment.Core.Domain.RepositoryContracts;
using UniversityEnrollment.Core.DTOs.RoleDTOs;
using UniversityEnrollment.Core.ServiceContracts;

namespace UniversityEnrollment.Core.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;

        public RoleService(IRoleRepository roleRepository, IMapper mapper)
        {
            _roleRepository = roleRepository;
            _mapper = mapper;
        }

        public async Task<Result<RoleDTO>> CreateAsync(CreateRoleDTO createRoleDto)
        {
            var existingRole = await _roleRepository.GetRoleByNameAsync(createRoleDto.Name);
            if (existingRole != null)
                return Result.Failure<RoleDTO>(RoleErrors.RoleAlreadyExists);

            var role = _mapper.Map<Role>(createRoleDto);
            await _roleRepository.AddAsync(role);

            var roleDto = _mapper.Map<RoleDTO>(role);
            return Result.Success(roleDto);
        }

        public async Task<Result<RoleDTO>> GetByIdAsync(Guid id)
        {
            var role = await _roleRepository.GetRoleWithUsersAsync(id);
            if (role == null)
                return Result.Failure<RoleDTO>(RoleErrors.RoleNotFound);

            var roleDto = _mapper.Map<RoleDTO>(role);
            return Result.Success(roleDto);
        }

        public async Task<Result<RoleDTO>> GetByNameAsync(string name)
        {
            var role = await _roleRepository.GetRoleByNameAsync(name);
            if (role == null)
                return Result.Failure<RoleDTO>(RoleErrors.RoleNotFound);

            var roleDto = _mapper.Map<RoleDTO>(role);
            return Result.Success(roleDto);
        }

        public async Task<Result<List<RoleDTO>>> GetAllAsync()
        {
            var roles = await _roleRepository.GetAllAsync();
            var roleDtos = _mapper.Map<List<RoleDTO>>(roles);
            return Result.Success(roleDtos);
        }

        public async Task<Result<List<RoleDTO>>> GetUserRolesAsync(Guid userId)
        {
            var roles = await _roleRepository.GetUserRolesAsync(userId);
            var roleDtos = _mapper.Map<List<RoleDTO>>(roles);
            return Result.Success(roleDtos);
        }

        public async Task<Result<RoleDTO>> UpdateAsync(UpdateRoleDTO updateRoleDto)
        {
            var role = await _roleRepository.GetByIdAsync(updateRoleDto.Id);
            if (role == null)
                return Result.Failure<RoleDTO>(RoleErrors.RoleNotFound);

            _mapper.Map(updateRoleDto, role);
            await _roleRepository.UpdateAsync(role);

            var roleDto = _mapper.Map<RoleDTO>(role);
            return Result.Success(roleDto);
        }

        public async Task<Result> DeleteAsync(Guid id)
        {
            var role = await _roleRepository.GetByIdAsync(id);
            if (role == null)
                return Result.Failure(RoleErrors.RoleNotFound);

            await _roleRepository.DeleteAsync(role);
            return Result.Success();
        }
    }
}
