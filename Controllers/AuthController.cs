using BaseApi.Common;
using BaseApi.DTOs.Requests;
using BaseApi.DTOs.Response;
using BaseApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BaseApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var result = await _authService.LoginAsync(request);

            if (result == null)
            {
                return Unauthorized(
                    ApiResponse<object>
                        .ErrorResponse("Invalid email or password.")
                );
            }

            return Ok(
                ApiResponse<LoginResponse>
                    .SuccessResponse(result, "Login successfully.")
            );
        }
    }
}