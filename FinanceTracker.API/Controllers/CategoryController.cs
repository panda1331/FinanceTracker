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
    [Route("api/categories")]
    [Authorize]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        /// <summary>Creates a new custom category.</summary>
        /// <param name="request">Category name and type.</param>
        [HttpPost]
        public async Task<IActionResult> CreateCategory(CreateCategoryRequest request)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _categoryService.CreateCategoryAsync(userId, request);
            return Ok(ApiResponse<CategoryResponse>.SuccessResponse(response));
        }

        /// <summary>Returns all categories (default + user's own).</summary>
        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _categoryService.GetCategoriesByUserIdAsync(userId);
            return Ok(ApiResponse<List<CategoryResponse>>.SuccessResponse(response));
        }

        /// <summary>Deletes a custom category by ID (default categories cannot be deleted).</summary>
        /// <param name="id">Category ID.</param>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(Guid id)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            await _categoryService.DeleteCategoryAsync(userId, id);
            return Ok(ApiResponse<string>.SuccessResponse("Category deleted"));
        }

        /// <summary>Updates a custom category name (default categories cannot be updated).</summary>
        /// <param name="id">Category ID.</param>
        /// <param name="request">New category name.</param>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVategory(Guid id, [FromBody] UpdateCategoryRequest request)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _categoryService.UpdateCategoryAsync(userId, id, request);
            return Ok(ApiResponse<CategoryResponse>.SuccessResponse(response));
        }
    }
}
