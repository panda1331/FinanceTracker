using FinanceTracker.Application.DTOs.Requests;
using FinanceTracker.Application.DTOs.Responses;
using FinanceTracker.Application.Interfaces.Services;
using FinanceTracker.Shared.Responses;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace FinanceTracker.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser(RegisterRequest request)
        {
            var response = await _authService.RegisterAsync(request);
            return Ok(ApiResponse<AuthResponse>.SuccessResponse(response));
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginUser(LoginRequest request)
        {
            var response = await _authService.LoginAsync(request);
            return Ok(ApiResponse<AuthResponse>.SuccessResponse(response));
        }
    }
}
