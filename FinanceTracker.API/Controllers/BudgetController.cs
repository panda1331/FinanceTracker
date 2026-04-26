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

        [HttpPost]
        public async Task<IActionResult> CreateOrUpdateBudget(CreateBudgetRequest request)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _budgetService.CreateOrUpdateBudgetAsync(userId, request);
            return Ok(ApiResponse<BudgetResponse>.SuccessResponse(response));
        }

        [HttpGet]
        public async Task<IActionResult> GetBudgets()
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _budgetService.GetBudgetsByUserIdAsync(userId);
            return Ok(ApiResponse<List<BudgetResponse>>.SuccessResponse(response));
        }
    }
}
