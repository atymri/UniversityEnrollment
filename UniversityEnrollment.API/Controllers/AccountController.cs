using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UniversityEnrollment.Core.Domain.Entities;
using UniversityEnrollment.Core.DTOs.UserDTO;
using UniversityEnrollment.Core.DTOs.UserDTOs;
using UniversityEnrollment.Core.Enums;
using UniversityEnrollment.Core.ServiceContracts;

namespace UniversityEnrollment.API.Controllers
{
    [AllowAnonymous]
    public class AccountController : BaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly IJwtService _jwtService;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<ApplicationRole> roleManager,
            IMapper mapper,
            IJwtService jwtService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _mapper = mapper;
            _jwtService = jwtService;
        }

        [HttpPost("register")]
        [ProducesResponseType(typeof(UserDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserDTO>> Register(CreateUserDTO dto)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            var user = _mapper.Map<ApplicationUser>(dto);
            user.UserName = dto.Email;

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
                return Problem(
                    title: "User registration failed",
                    detail: string.Join(", ", result.Errors.Select(e => e.Description)),
                    statusCode: StatusCodes.Status400BadRequest);

            var roleName = dto.IsTeacher ? Roles.Teacher.ToString() : Roles.Student.ToString();

            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                var roleResult = await _roleManager.CreateAsync(new ApplicationRole { Name = roleName });
                if (!roleResult.Succeeded)
                    return Problem(
                        title: "Role creation failed",
                        detail: string.Join(", ", roleResult.Errors.Select(e => e.Description)),
                        statusCode: StatusCodes.Status500InternalServerError);
            }

            var addToRoleResult = await _userManager.AddToRoleAsync(user, roleName);
            if (!addToRoleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);
                return Problem(
                    title: "Adding user to role failed",
                    detail: string.Join(", ", addToRoleResult.Errors.Select(e => e.Description)),
                    statusCode: StatusCodes.Status500InternalServerError);
            }

            var authResponse = _jwtService.GenerateToken(user);
            var userDto = _mapper.Map<UserDTO>(user);
            return CreatedAtAction(nameof(GetUserById), new { id = userDto.Id }, authResponse);
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<LoginResponseDTO>> Login(LoginUserDTO loginDto)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
                return Problem(
                    title: "Login failed",
                    detail: "Invalid email or password",
                    statusCode: StatusCodes.Status401Unauthorized);

            var result = await _signInManager.PasswordSignInAsync(user.UserName, loginDto.Password, isPersistent: loginDto.RememberMe, lockoutOnFailure: false);

            if (!result.Succeeded)
                return Problem(
                    title: "Login failed",
                    detail: result.IsLockedOut ? "Account locked out" : "Invalid email or password",
                    statusCode: StatusCodes.Status401Unauthorized);

            var roles = await _userManager.GetRolesAsync(user);
            var userDto = _mapper.Map<UserDTO>(user);
            var authResponse = _jwtService.GenerateToken(user);

            var response = new LoginResponseDTO
            {
                AuthResponse = authResponse,
                Roles = roles.ToList(),
                Message = "Login successful"
            };

            return Ok(response);
        }

        [HttpPost("logout")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return NoContent();
        }

        [HttpGet("user/{id:guid}")]
        [ProducesResponseType(typeof(UserDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserDTO>> GetUserById(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
                return Problem(
                    title: "User not found",
                    detail: $"User with ID {id} does not exist",
                    statusCode: StatusCodes.Status404NotFound);

            var userDto = _mapper.Map<UserDTO>(user);
            return Ok(userDto);
        }

        [HttpGet("user/email/{email}")]
        [ProducesResponseType(typeof(UserDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserDTO>> GetUserByEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return Problem(
                    title: "User not found",
                    detail: $"User with email {email} does not exist",
                    statusCode: StatusCodes.Status404NotFound);

            var userDto = _mapper.Map<UserDTO>(user);
            return Ok(userDto);
        }

        [HttpPost("change-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ChangePassword(ChangePasswordDTO changePasswordDto)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            var user = await _userManager.FindByEmailAsync(changePasswordDto.Email);
            if (user == null)
                return Problem(
                    title: "User not found",
                    detail: "Invalid email",
                    statusCode: StatusCodes.Status400BadRequest);

            var result = await _userManager.ChangePasswordAsync(user, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);

            if (!result.Succeeded)
                return Problem(
                    title: "Password change failed",
                    detail: string.Join(", ", result.Errors.Select(e => e.Description)),
                    statusCode: StatusCodes.Status400BadRequest);

            return Ok(new { message = "Password changed successfully" });
        }

        [HttpGet("users")]
        [ProducesResponseType(typeof(List<UserDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<UserDTO>>> GetAllUsers()
        {
            var users = _userManager.Users.ToList();
            var userDtos = _mapper.Map<List<UserDTO>>(users);
            return Ok(userDtos);
        }
    }
}