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
    [Route("api/analytics")]
    [Authorize]
    public class AnalyticsController : ControllerBase
    {
        private readonly IAnalyticsService _analyticsService;

        public AnalyticsController(IAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        /// <summary>Returns expense summary grouped by categories for the given month and year.</summary>
        /// <param name="month">Month (1-12).</param>
        /// <param name="year">Year.</param>
        [HttpGet]
        public async Task<IActionResult> GetAnalytics([FromQuery] int month, [FromQuery] int year)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _analyticsService.GetAnalyticsAsync(userId, month, year);
            return Ok(ApiResponse<AnalyticsResponse>.SuccessResponse(response));
        }
    }
}
