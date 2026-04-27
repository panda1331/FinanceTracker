using FinanceTracker.Application.DTOs.Requests;
using FinanceTracker.Application.DTOs.Responses;
using FinanceTracker.Application.Interfaces.Services;
using FinanceTracker.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;
using System.Security.Claims;

namespace FinanceTracker.API.Controllers
{
    [ApiController]
    [Route("api/budgets")]
    [Authorize]
    public class BudgetController : ControllerBase
    {
        private readonly IBudgetService _budgetService;

        public BudgetController(IBudgetService budgetService)
        {
            _budgetService = budgetService;
        }

        /// <summary>Creates a new budget or updates an existing one for a category in a given month.</summary>
        /// <param name="request">Category ID, limit, month, year.</param>
        [HttpPost]
        public async Task<IActionResult> CreateOrUpdateBudget(CreateBudgetRequest request)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _budgetService.CreateOrUpdateBudgetAsync(userId, request);
            return Ok(ApiResponse<BudgetResponse>.SuccessResponse(response));
        }

        /// <summary>Returns all budgets of the current user with exceeded status.</summary>
        [HttpGet]
        public async Task<IActionResult> GetBudgets()
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _budgetService.GetBudgetsByUserIdAsync(userId);
            return Ok(ApiResponse<List<BudgetResponse>>.SuccessResponse(response));
        }

        /// <summary>Deletes a budget by ID.</summary>
        /// <param name="id">Budget ID.</param>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBudget(Guid id)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            await _budgetService.DeleteBudgetAsync(userId, id);
            return Ok(ApiResponse<string>.SuccessResponse("Budget deleted"));
        }
    }
}
