using BaseApi.Common;
using BaseApi.DTOs.Requests;
using BaseApi.DTOs.Response;
using BaseApi.Models;
using BaseApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BaseApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: api/users
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllAsync();
            var response = users.Select(user => new UserResponse
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role,
                IsActive = user.IsActive
            });
            return Ok(
                 ApiResponse<IEnumerable<UserResponse>>
                     .SuccessResponse(response, "Get users successfully.")
             );
        }


        // GET: api/users/1
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null){
                return NotFound(
                        ApiResponse<object>
                            .ErrorResponse("User not found.")
                    );
            }

            var response = new UserResponse
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role,
                IsActive = user.IsActive
            };
            return Ok(
                    ApiResponse<UserResponse>
                        .SuccessResponse(response, "Get user successfully.")
);
        }


        // POST: api/users
        [HttpPost]
        public async Task<IActionResult> Create(CreateUserRequest request)
        {
            var existingUser = await _userService.GetByEmailAsync(request.Email);
            
            if (existingUser != null)
                return BadRequest(
                    ApiResponse<object>
                        .ErrorResponse("Email already exists.")
                );

            var user = new User
            {
                FullName = request.FullName,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role = request.Role
            };

            var createdUser = await _userService.CreateAsync(user);

            var response = new UserResponse
            {
                Id = createdUser.Id,
                FullName = createdUser.FullName,
                Email = createdUser.Email,
                Role = createdUser.Role,
                IsActive = createdUser.IsActive
            };
            return Ok(
                ApiResponse<UserResponse>
                    .SuccessResponse(response, "Create user successfully.")
            );
        }

        // PUT: api/users/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateUserRequest request)
        {
            var user = await _userService.GetByIdAsync(id);

            if (user == null)
            {
                return NotFound(
                    ApiResponse<object>
                        .ErrorResponse("User not found.")
                );
            }

            var existingUser = await _userService.GetByEmailAsync(request.Email);

            if (existingUser != null && existingUser.Id != id)
            {
                return BadRequest(
                    ApiResponse<object>
                        .ErrorResponse("Email already exists.")
                );
            }

            // Update 
            user.FullName = request.FullName;
            user.Email = request.Email;
            user.Role = request.Role;
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var updatedUser = await _userService.UpdateAsync(user);

            var response = new UserResponse
            {
                Id = updatedUser.Id,
                FullName = updatedUser.FullName,
                Email = updatedUser.Email,
                Role = updatedUser.Role,
                IsActive = updatedUser.IsActive
            };

            return Ok(
                ApiResponse<UserResponse>
                    .SuccessResponse(response, "Update user successfully.")
            );
        }

        // DELETE: api/users/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _userService.DeleteAsync(id);

            if (!deleted) {
                return NotFound(
                    ApiResponse<object>
                        .ErrorResponse("User not found.")
                );
            }
            return Ok(
                ApiResponse<object>
                    .SuccessResponse(null, "Delete user successfully.")
            );
        }
    }
}
