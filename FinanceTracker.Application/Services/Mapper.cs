using FinanceTracker.Application.DTOs.Responses;
using FinanceTracker.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceTracker.Application.Services
{
    public static class Mapper
    {
        public static List<AccountResponse> ToResponses(IReadOnlyList<Account> accounts)
        {
            var responses = new List<AccountResponse>();
            foreach (var account in accounts)
            {
                var response = new AccountResponse
                {
                    Id = account.Id,
                    Name = account.Name,
                    Type = account.Type,
                    Balance = account.Balance,
                };
                responses.Add(response);
            }
            return responses;
        }

        public static List<UserResponse> ToResponses(IReadOnlyList<User> users)
        {
            var responses = new List<UserResponse>();
            foreach (var user in users)
            {
                var response = new UserResponse
                {
                    Id = user.Id,
                    CreatedAt = user.CreatedAt,
                    Email = user.Email,
                    Role = user.Type.ToString(),
                };
                responses.Add(response);
            }
            return responses;
        }

        public static List<CategoryResponse> ToResponses(IReadOnlyList<Category> categories)
        {
            var responses = new List<CategoryResponse>();
            foreach (var category in categories)
            {
                var response = new CategoryResponse()
                {
                    Id = category.Id,
                    Name = category.Name,
                    Type = category.Type,
                    IsDefault = category.IsDefault,
                };
                responses.Add(response);
            }
            return responses;
        }

        public static List<TransactionResponse> ToResponses(IReadOnlyList<Transaction> transactions)
        {
            var responses = new List<TransactionResponse>();
            foreach (var transaction in transactions)
            {
                var response = new TransactionResponse
                {
                    Id = transaction.Id,
                    AccountId = transaction.AccountId,
                    Amount = transaction.Amount,
                    CategoryId = transaction.CategoryId,
                    Date = transaction.Date,
                    Description = transaction.Description,
                    Type = transaction.Type,
                };
                responses.Add(response);
            }
            return responses;
        }


        public static List<BudgetResponse> ToResponses(IReadOnlyList<Budget> budgets)
        {
            var responses = new List<BudgetResponse>();
            foreach(var budget in budgets)
            {
                var response = new BudgetResponse
                {
                    Id = budget.Id,
                    CategoryId = budget.CategoryId,
                    Limit = budget.Limit,
                    Month = budget.Month,
                    Year = budget.Year,
                };
                responses.Add(response);
            }
            return responses;
        }
    }
}
