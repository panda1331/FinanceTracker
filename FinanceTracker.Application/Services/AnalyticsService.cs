using FinanceTracker.Application.DTOs.Responses;
using FinanceTracker.Application.Interfaces.Services;
using FinanceTracker.Application.Repository;
using FinanceTracker.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceTracker.Application.Services
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IBudgetRepository _budgetRepository;

        public AnalyticsService(ITransactionRepository transactionRepository, ICategoryRepository categoryRepository, IBudgetRepository budgetRepository)
        {
            _transactionRepository = transactionRepository;
            _categoryRepository = categoryRepository;
            _budgetRepository = budgetRepository;
        }

        public async Task<AnalyticsResponse> GetAnalyticsAsync(Guid userId, int month, int year)
        {
            var transactions = await _transactionRepository.GetByUserIdAndPeriodAsync(userId, month, year);
            var expences = transactions.Where(t => t.Type == Domain.Enums.OperationType.Expense).GroupBy(t => t.CategoryId);
            var categories = await _categoryRepository.GetByUserIdAsync(userId);
            var items = new List<AnalyticsItem>();

            foreach (var expense in expences)
            {
                var category = categories.FirstOrDefault(c => c.Id == expense.Key);
                if (category == null)
                    continue;

                var categoryName = category.Name;
                var total = expense.Sum(e => e.Amount);
                var budget = await _budgetRepository.GetByCategoryAndMonthAsync(userId, category.Id, month, year);

                var limit = budget?.Limit;
                var isExceeded = budget != null && total > limit;

                items.Add(new AnalyticsItem
                {
                    CategoryName = categoryName,
                    IsExceeded = isExceeded,
                    Limit = limit,
                    TotalAmount = total,
                });
            }

            var totalExpences = items.Sum(i => i.TotalAmount);
            return new AnalyticsResponse
            {
                TotalExpense = totalExpences,
                Items = items,
            };
        }
    }
}
