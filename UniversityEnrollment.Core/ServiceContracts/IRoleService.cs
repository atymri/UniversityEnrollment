using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniversityEnrollment.Core.Common.Results;
using UniversityEnrollment.Core.DTOs.RoleDTOs;

namespace UniversityEnrollment.Core.ServiceContracts
{
    public interface IRoleService
    {
        Task<Result<RoleDTO>> GetByIdAsync(Guid id);
        Task<Result<RoleDTO>> GetByNameAsync(string name);
        Task<Result<List<RoleDTO>>> GetAllAsync();
        Task<Result<List<RoleDTO>>> GetUserRolesAsync(Guid userId);
        Task<Result<RoleDTO>> CreateAsync(CreateRoleDTO createRoleDto);
        Task<Result<RoleDTO>> UpdateAsync(UpdateRoleDTO updateRoleDto);
        Task<Result> DeleteAsync(Guid id);
    }
}
