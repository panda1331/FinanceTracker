using FinanceTracker.Application.DTOs.Requests;
using FinanceTracker.Application.DTOs.Responses;
using FinanceTracker.Application.Interfaces.Services;
using FinanceTracker.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FinanceTracker.API.Controllers
{
    [ApiController]
    [Route("api/accounts")]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        /// <summary>Creates a new account.</summary>
        [HttpPost]
        public async Task<IActionResult> CreateAccount(CreateAccountRequest request)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _accountService.CreateAccountAsync(userId, request);
            return Ok(ApiResponse<AccountResponse>.SuccessResponse(response));
        }

        /// <summary>Gets all accounts for current user.</summary>
        [HttpGet]
        public async Task<IActionResult> GetAccounts()
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _accountService.GetAccountsByUserIdAsync(userId);
            return Ok(ApiResponse<List<AccountResponse>>.SuccessResponse(response));
        }

        /// <summary>Deletes an account by ID.</summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccount(Guid id)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            await _accountService.DeleteAccountAsync(userId, id);
            return Ok(ApiResponse<string>.SuccessResponse("Account deleted"));
        }

        /// <summary>Updates account name by ID.</summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAccount(Guid id, [FromBody] UpdateAccountRequest request)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _accountService.UpdateAccountAsync(userId, id, request);
            return Ok(ApiResponse<AccountResponse>.SuccessResponse(response));
        }
    }
}
