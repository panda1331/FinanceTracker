using FinanceTracker.Application.DTOs.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceTracker.Application.Interfaces.Services
{
    public interface IAdminService
    {
        Task<List<UserResponse>> GetAllUsersAsync();
        Task<List<AccountResponse>> GetUserAccountsAsync(Guid userId);
        Task<List<TransactionResponse>> GetUserTransactionsAsync(Guid userId);
        Task<List<BudgetResponse>> GetUserBudgetsAsync(Guid userId);
        Task<List<CategoryResponse>> GetUserCategoriesAsync(Guid userId);

        Task<List<AccountResponse>> GetAllAccountsAsync();
        Task<List<TransactionResponse>> GetAllTransactionsAsync();
        Task<List<BudgetResponse>> GetAllBudgetsAsync();
        Task<List<CategoryResponse>> GetAllCategoriesAsync();

        Task DeleteUserAccountAsync(Guid accountId);
        Task DeleteUserTransactionAsync(Guid transactionId);
        Task DeleteUserBudgetAsync(Guid budgetId);
        Task DeleteUserCategoryAsync(Guid categoryId);
    }
}
