using FinanceTracker.Application.DTOs.Requests;
using FinanceTracker.Application.DTOs.Responses;
using FinanceTracker.Application.Interfaces.Services;
using FinanceTracker.Application.Repository;
using FinanceTracker.Domain.Enums;
using FinanceTracker.Domain.Models;
using FinanceTracker.Shared.Exceptions;
using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;

namespace FinanceTracker.Application.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IBudgetService _budgetService;

        public TransactionService(ITransactionRepository transactionRepository, IAccountRepository accountRepository, ICategoryRepository categoryRepository, IBudgetService budgetService)
        {
            _transactionRepository = transactionRepository;
            _accountRepository = accountRepository;
            _categoryRepository = categoryRepository;
            _budgetService = budgetService;
        }

        public async Task<TransactionResponse> CreateTransactionAsync(Guid userId, CreateTransactionRequest request)
        {
            var account = await _accountRepository.GetByIdAsync(request.AccountId);
            if (account == null) 
                throw new NotFoundException("No such account");

            var category = await _categoryRepository.GetByIdAsync(request.CategoryId);
            if (category == null)
                throw new NotFoundException("No such category");

            if (category.Type != request.Type)
                throw new ValidationException("Category and transaction types do not match");

            if (request.Type == OperationType.Expense && account.Balance < request.Amount)
                throw new ValidationException("Insufficient funds");

            var transaction = new Transaction(userId, account.Id, category.Id, request.Type, request.Amount, request.Date);
            if (transaction.Type == OperationType.Expense)
                account.SubtractFromBalance(transaction.Amount);
            else
                account.AddToBalance(transaction.Amount);

            _accountRepository.Update(account);
            await _transactionRepository.AddAsync(transaction);
            await _transactionRepository.SaveChangesAsync();

            if (request.Type == OperationType.Expense)
                await _budgetService.CheckBudgetExceedAsync(userId, category.Id, request.Date.Month, request.Date.Year);

            return new TransactionResponse
            {
                Id = transaction.Id,
                AccountId = account.Id,
                Amount = transaction.Amount,
                CategoryId = category.Id,
                Date = transaction.Date,
                Description = transaction.Description,
                Type = transaction.Type,
            };            
        }

        public async Task DeleteTransactionAsync(Guid userId, Guid transactionId)
        {
            var transaction = await _transactionRepository.GetByIdAsync(transactionId);
            if (transaction == null)
                throw new NotFoundException("No such transaction");

            if (transaction.UserId != userId)
                throw new ValidationException("You can only delete your own transactions");

            var account = await _accountRepository.GetByIdAsync(transaction.AccountId);
            if (account == null)
                throw new NotFoundException("Account not found");

            if (transaction.Type == OperationType.Expense)
                account.AddToBalance(transaction.Amount);
            else
                account.SubtractFromBalance(transaction.Amount);

            _accountRepository.Update(account);
            _transactionRepository.Delete(transaction);
            await _transactionRepository.SaveChangesAsync();
        }

        public async Task<List<TransactionResponse>> GetTransactionsByUserIdAsync(Guid userId)
        {
            var transactions = await _transactionRepository.GetByUserIdAsync(userId);
            return Mapper.ToResponses(transactions);
        }
    }
}
