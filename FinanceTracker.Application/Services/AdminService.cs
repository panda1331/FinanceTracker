using FinanceTracker.Application.DTOs.Responses;
using FinanceTracker.Application.Interfaces.Services;
using FinanceTracker.Application.Repository;
using FinanceTracker.Domain.Models;
using FinanceTracker.Shared.Exceptions;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Reflection.PortableExecutable;
using System.Text;

namespace FinanceTracker.Application.Services
{
    public class AdminService : IAdminService
    {
        private readonly IUserRepository _userRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IBudgetRepository _budgetRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ITransactionRepository _transactionRepository;

        public AdminService(IUserRepository userRepository, IAccountRepository accountRepository, IBudgetRepository budgetRepository, ICategoryRepository categoryRepository, ITransactionRepository transactionRepository)
        {
            _userRepository = userRepository;
            _accountRepository = accountRepository;
            _budgetRepository = budgetRepository;
            _categoryRepository = categoryRepository;
            _transactionRepository = transactionRepository;
        }

        public async Task DeleteUserAccountAsync(Guid accountId)
        {
            var account = await _accountRepository.GetByIdAsync(accountId);
            if (account == null)
                throw new NotFoundException("No such account");
            _accountRepository.Delete(account);
            await _accountRepository.SaveChangesAsync();
        }

        public async Task DeleteUserBudgetAsync(Guid budgetId)
        {
            var budget = await _budgetRepository.GetByIdAsync(budgetId);
            if (budget == null)
                throw new NotFoundException("No such budget");
            _budgetRepository.Delete(budget);
            await _budgetRepository.SaveChangesAsync();
        }

        public async Task DeleteUserTransactionAsync(Guid transactionId)
        {
            var transaction = await _transactionRepository.GetByIdAsync(transactionId);
            if (transaction == null)
                throw new NotFoundException("No such transaction");
            _transactionRepository.Delete(transaction);
            await _transactionRepository.SaveChangesAsync();
        }

        public async Task DeleteUserCategoryAsync(Guid categoryId)
        {
            var category = await _categoryRepository.GetByIdAsync(categoryId);
            if (category == null)
                throw new NotFoundException("No such category");
            _categoryRepository.Delete(category);
            await _categoryRepository.SaveChangesAsync();
        }

        public async Task<List<UserResponse>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return Mapper.ToResponses(users);
        }

        public async Task<List<AccountResponse>> GetUserAccountsAsync(Guid userId)
        {
            var accounts = await _accountRepository.GetByUserIdAsync(userId);
            return Mapper.ToResponses(accounts);
        }

        public async Task<List<BudgetResponse>> GetUserBudgetsAsync(Guid userId)
        {
            var budgets = await _budgetRepository.GetByUserIdAsync(userId);
            var categories = await _categoryRepository.GetAllAsync();
            var categoryNames = categories.ToDictionary(c => c.Id, c => c.Name);

            return budgets.Select(b => new BudgetResponse
            {
                Id = b.Id,
                CategoryId = b.CategoryId,
                CategoryName = categoryNames.GetValueOrDefault(b.CategoryId, "Unknown"),
                Limit = b.Limit,
                Month = b.Month,
                Year = b.Year
            }).ToList();
        }

        public async Task<List<CategoryResponse>> GetUserCategoriesAsync(Guid userId)
        {
            var categories = await _categoryRepository.GetByUserIdAsync(userId);
            return Mapper.ToResponses(categories);
        }

        public async Task<List<TransactionResponse>> GetUserTransactionsAsync(Guid userId)
        {
            var transactions = await _transactionRepository.GetByUserIdAsync(userId);
            return Mapper.ToResponses(transactions);
        }

        public async Task<List<AccountResponse>> GetAllAccountsAsync()
        {
            var accounts = await _accountRepository.GetAllAsync();
            return Mapper.ToResponses(accounts);
        }

        public async Task<List<TransactionResponse>> GetAllTransactionsAsync()
        {
            var transactions = await _transactionRepository.GetAllAsync();
            return Mapper.ToResponses(transactions);
        }

        public async Task<List<BudgetResponse>> GetAllBudgetsAsync()
        {
            var budgets = await _budgetRepository.GetAllAsync();
            var categories = await _categoryRepository.GetAllAsync();
            var categoryNames = categories.ToDictionary(c => c.Id, c => c.Name);

            return budgets.Select(b => new BudgetResponse
            {
                Id = b.Id,
                CategoryId = b.CategoryId,
                CategoryName = categoryNames.GetValueOrDefault(b.CategoryId, "Unknown"),
                Limit = b.Limit,
                Month = b.Month,
                Year = b.Year
            }).ToList();
        }

        public async Task<List<CategoryResponse>> GetAllCategoriesAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();
            return Mapper.ToResponses(categories);
        }
    }
}
