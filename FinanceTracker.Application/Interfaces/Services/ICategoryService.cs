using FinanceTracker.Application.DTOs.Requests;
using FinanceTracker.Application.DTOs.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceTracker.Application.Interfaces.Services
{
    public interface ICategoryService
    {
        Task<CategoryResponse> CreateCategoryAsync(Guid userId, CreateCategoryRequest request);
        Task<List<CategoryResponse>> GetCategoriesByUserIdAsync(Guid userId);
        Task DeleteCategoryAsync(Guid userId, Guid categoryId);
        Task<CategoryResponse> UpdateCategoryAsync(Guid userId, Guid categoryId, UpdateCategoryRequest request);
    }
}
