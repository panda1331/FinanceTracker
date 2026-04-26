using FinanceTracker.Application.DTOs.Requests;
using FinanceTracker.Application.DTOs.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceTracker.Application.Interfaces.Services
{
    public interface IBudgetService
    {
        Task<BudgetResponse> CreateOrUpdateBudgetAsync(Guid userId, CreateBudgetRequest budgetRequest);
        Task<List<BudgetResponse>> GetBudgetsByUserIdAsync(Guid userId);
        Task CheckBudgetExceedAsync(Guid userId, Guid categoryId, int month, int year);
        Task DeleteBudgetAsync(Guid userId, Guid budgetId);
    }
}
