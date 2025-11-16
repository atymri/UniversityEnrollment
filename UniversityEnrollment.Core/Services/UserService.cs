using AutoMapper;
using UniversityEnrollment.Core.Common.Results;
using UniversityEnrollment.Core.Domain.Entities;
using UniversityEnrollment.Core.Domain.RepositoryContracts;
using UniversityEnrollment.Core.DTOs.UserDTOs;
using UniversityEnrollment.Core.ServiceContracts;

namespace UniversityEnrollment.Core.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<Result<UserDTO>> CreateAsync(CreateUserDTO createUserDto)
        {
            if (await _userRepository.IsEmailDuplicatedAsync(createUserDto.Email))
                return Result.Failure<UserDTO>(UserErrors.EmailAlreadyExists);

            var user = _mapper.Map<User>(createUserDto);
            await _userRepository.AddAsync(user);

            var userDto = _mapper.Map<UserDTO>(user);
            return Result.Success(userDto);
        }

        public async Task<Result<UserDTO>> GetByIdAsync(Guid id)
        {
            var user = await _userRepository.GetUserWithEnrollmentsAsync(id);
            if (user == null)
                return Result.Failure<UserDTO>(UserErrors.UserNotFound);

            var userDto = _mapper.Map<UserDTO>(user);
            return Result.Success(userDto);
        }

        public async Task<Result<List<UserDTO>>> GetAllAsync()
        {
            var users = await _userRepository.GetAllAsync();
            var userDtos = _mapper.Map<List<UserDTO>>(users);
            return Result.Success(userDtos);
        }

        public async Task<Result<UserDTO>> UpdateAsync(UpdateUserDTO updateUserDto)
        {
            var user = await _userRepository.GetByIdAsync(updateUserDto.Id);
            if (user == null)
                return Result.Failure<UserDTO>(UserErrors.UserNotFound);

            _mapper.Map(updateUserDto, user);
            await _userRepository.UpdateAsync(user);

            var userDto = _mapper.Map<UserDTO>(user);
            return Result.Success(userDto);
        }

        public async Task<Result<UserDTO>> AddUserInRole(Guid userId, Guid roleId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return Result.Failure<UserDTO>(UserErrors.UserNotFound);

            var updatedUser = await _userRepository.AddInRole(userId, roleId);
            var userDto = _mapper.Map<UserDTO>(updatedUser);

            return Result.Success(userDto);
        }

        public async Task<Result> DeleteAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return Result.Failure(UserErrors.UserNotFound);

            await _userRepository.DeleteAsync(user);
            return Result.Success();
        }
    }
}
