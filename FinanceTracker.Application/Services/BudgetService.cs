using FinanceTracker.Application.DTOs.Requests;
using FinanceTracker.Application.DTOs.Responses;
using FinanceTracker.Application.Interfaces.Services;
using FinanceTracker.Application.Repository;
using FinanceTracker.Domain.Enums;
using FinanceTracker.Domain.Models;
using FinanceTracker.Shared.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace FinanceTracker.Application.Services
{
    public class BudgetService : IBudgetService
    {
        private readonly IBudgetRepository _budgetRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly ICategoryRepository _categoryRepository;

        public BudgetService(IBudgetRepository budgetRepository, ITransactionRepository transactionRepository, ICategoryRepository categoryRepository)
        {
            _budgetRepository = budgetRepository;
            _transactionRepository = transactionRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task CheckBudgetExceedAsync(Guid userId, Guid categoryId, int month, int year)
        {
            var budget = await _budgetRepository.GetByCategoryAndMonthAsync(userId, categoryId, month, year);
            if (budget == null) 
                return;

            var transactions = await _transactionRepository.GetByCategoryAndPeriodAsync(userId, categoryId, month, year);
            var total = transactions.Sum(t => t.Amount);
            if (total > budget.Limit)
                throw new BudgetExceededException("Budget is exceeded");
        }

        public async Task<BudgetResponse> CreateOrUpdateBudgetAsync(Guid userId, CreateBudgetRequest budgetRequest)
        {
            var category = await _categoryRepository.GetByIdAsync(budgetRequest.CategoryId);
            if (category == null)
                throw new NotFoundException("No such category");
            if (category.Type == OperationType.Income)
                throw new ValidationException("Limits can not be on put on income categoreis");

            var budget = await _budgetRepository.GetByCategoryAndMonthAsync(userId, budgetRequest.CategoryId, budgetRequest.Month, budgetRequest.Year);
            if (budget != null)
            {
                budget.Update(budgetRequest.Limit);
                _budgetRepository.Update(budget);
            }
            else
            {
                budget = new Budget(userId, budgetRequest.CategoryId, budgetRequest.Limit, budgetRequest.Month, budgetRequest.Year);
                await _budgetRepository.AddAsync(budget);
            }
            await _budgetRepository.SaveChangesAsync();

            return new BudgetResponse
            {
                Id = budget.Id,
                Limit = budget.Limit,
                Year = budget.Year,
                Month = budget.Month,
            };
        }

        public async Task DeleteBudgetAsync(Guid userId, Guid budgetId)
        {
            var budget = await _budgetRepository.GetByIdAsync(budgetId);
            if (budget == null)
                throw new NotFoundException("No such budget");

            if (budget.UserId != userId)
                throw new ValidationException("You can only delete your own budgets");

            _budgetRepository.Delete(budget);
            await _budgetRepository.SaveChangesAsync();
        }

        public async Task<List<BudgetResponse>> GetBudgetsByUserIdAsync(Guid userId)
        {
            var budgets = await _budgetRepository.GetByUserIdAsync(userId);
            var responses = new List<BudgetResponse>();

            foreach (var budget in budgets)
            {
                var transactions = await _transactionRepository.GetByCategoryAndPeriodAsync(userId, budget.CategoryId, budget.Month, budget.Year);
                var totalExpense = transactions.Sum(t => t.Amount);
                var category = await _categoryRepository.GetByIdAsync(budget.CategoryId);

                responses.Add(new BudgetResponse
                {
                    Id = budget.Id,
                    CategoryName = category?.Name ?? "unknown",
                    CategoryId = budget.CategoryId,
                    Limit = budget.Limit,
                    Month = budget.Month,
                    Year = budget.Year,
                    IsExceeded = totalExpense > budget.Limit
                });
            }
            return responses;
        }
    }
}
