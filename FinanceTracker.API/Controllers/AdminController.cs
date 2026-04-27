using FinanceTracker.Application.DTOs.Responses;
using FinanceTracker.Application.Interfaces.Services;
using FinanceTracker.Application.Repository;
using FinanceTracker.Domain.Models;
using FinanceTracker.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceTracker.API.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [Authorize(Roles ="Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        /// <summary>Returns all registered users.</summary>
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _adminService.GetAllUsersAsync();
            return Ok(ApiResponse<List<UserResponse>>.SuccessResponse(users));
        }

        /// <summary>Returns all accounts or filtered by user ID.</summary>
        /// <param name="userId">Optional user ID.</param>
        [HttpGet("accounts")]
        public async Task<IActionResult> GetAccounts([FromQuery] Guid? userId)
        {
            if (userId == null)
                return Ok(ApiResponse<List<AccountResponse>>.SuccessResponse(await _adminService.GetAllAccountsAsync()));
            else
                return Ok(ApiResponse<List<AccountResponse>>.SuccessResponse(await _adminService.GetUserAccountsAsync(userId.Value)));
        }

        /// <summary>Returns all budgets or filtered by user ID.</summary>
        /// <param name="userId">Optional user ID.</param>
        [HttpGet("budgets")]
        public async Task<IActionResult> GetBudgets([FromQuery] Guid? userId)
        {
            if (userId == null)
                return Ok(ApiResponse<List<BudgetResponse>>.SuccessResponse(await _adminService.GetAllBudgetsAsync()));
            else
                return Ok(ApiResponse<List<BudgetResponse>>.SuccessResponse(await _adminService.GetUserBudgetsAsync(userId.Value)));
        }

        /// <summary>Returns all categories or filtered by user ID.</summary>
        /// <param name="userId">Optional user ID.</param>
        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories([FromQuery] Guid? userId)
        {
            if (userId == null)
                return Ok(ApiResponse<List<CategoryResponse>>.SuccessResponse(await _adminService.GetAllCategoriesAsync()));
            else
                return Ok(ApiResponse<List<CategoryResponse>>.SuccessResponse(await _adminService.GetUserCategoriesAsync(userId.Value)));
        }

        /// <summary>Returns all transactions or filtered by user ID.</summary>
        /// <param name="userId">Optional user ID.</param>
        [HttpGet("transactions")]
        public async Task<IActionResult> GetTransactions([FromQuery] Guid? userId)
        {
            if (userId == null)
                return Ok(ApiResponse<List<TransactionResponse>>.SuccessResponse(await _adminService.GetAllTransactionsAsync()));
            else
                return Ok(ApiResponse<List<TransactionResponse>>.SuccessResponse(await _adminService.GetUserTransactionsAsync(userId.Value)));
        }

        /// <summary>Deletes any account by ID.</summary>
        /// <param name="id">Account ID.</param>
        [HttpDelete("accounts/{id}")]
        public async Task<IActionResult> DeleteAccount(Guid id)
        {
            await _adminService.DeleteUserAccountAsync(id);
            return Ok(ApiResponse<string>.SuccessResponse("Account deleted"));
        }

        /// <summary>Deletes any budget by ID.</summary>
        /// <param name="id">Budget ID.</param>
        [HttpDelete("budgets/{id}")]
        public async Task<IActionResult> DeleteBudget(Guid id)
        {
            await _adminService.DeleteUserBudgetAsync(id);
            return Ok(ApiResponse<string>.SuccessResponse("Budget deleted"));
        }

        /// <summary>Deletes any category by ID.</summary>
        /// <param name="id">Category ID.</param>
        [HttpDelete("categories/{id}")]
        public async Task<IActionResult> DeleteCategory(Guid id)
        {
            await _adminService.DeleteUserCategoryAsync(id);
            return Ok(ApiResponse<string>.SuccessResponse("Category deleted"));
        }

        /// <summary>Deletes any transaction by ID.</summary>
        /// <param name="id">Transaction ID.</param>
        [HttpDelete("transactions/{id}")]
        public async Task<IActionResult> DeleteTransaction(Guid id)
        {
            await _adminService.DeleteUserTransactionAsync(id);
            return Ok(ApiResponse<string>.SuccessResponse("Transaction deleted"));
        }
    }
}
