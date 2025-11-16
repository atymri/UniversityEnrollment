using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniversityEnrollment.Core.Common.Results;
using UniversityEnrollment.Core.DTOs.UserDTOs;

namespace UniversityEnrollment.Core.ServiceContracts
{
    public interface IUserService
    {
        Task<Result<UserDTO>> GetByIdAsync(Guid id);
        Task<Result<List<UserDTO>>> GetAllAsync();
        Task<Result<UserDTO>> CreateAsync(CreateUserDTO createUserDto);
        Task<Result<UserDTO>> UpdateAsync(UpdateUserDTO updateUserDto);
        Task<Result<UserDTO>> AddUserInRole(Guid userId, Guid roleId);
        Task<Result> DeleteAsync(Guid id);
    }
}
