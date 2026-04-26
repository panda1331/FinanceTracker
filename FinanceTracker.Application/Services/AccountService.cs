using FinanceTracker.Application.DTOs.Requests;
using FinanceTracker.Application.DTOs.Responses;
using FinanceTracker.Application.Interfaces.Services;
using FinanceTracker.Application.Repository;
using FinanceTracker.Domain.Models;
using FinanceTracker.Shared.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceTracker.Application.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _repository;

        public AccountService(IAccountRepository repository)
        {
            _repository = repository;
        }

        public async Task<AccountResponse> CreateAccountAsync(Guid userId, CreateAccountRequest request)
        {
            var account = new Account(request.Type, request.Name, userId);
            await _repository.AddAsync(account);
            await _repository.SaveChangesAsync();
            return new AccountResponse
            {
                Id = account.Id,
                Name = account.Name,
                Type = account.Type,
                Balance = account.Balance,
            };
        }

        public async Task DeleteAccountAsync(Guid userId, Guid accountId)
        {
            var account = await _repository.GetByIdAsync(accountId);
            if (account == null)
                throw new NotFoundException("No such account");

            if (account.UserId != userId)
                throw new ValidationException("You can only delete your own accounts");

            _repository.Delete(account);
            await _repository.SaveChangesAsync();
        }

        public async Task<List<AccountResponse>> GetAccountsByUserIdAsync(Guid userId)
        {
            var accounts = await _repository.GetByUserIdAsync(userId);
            return MapAccountsToResponses(accounts);
        }

        private List<AccountResponse> MapAccountsToResponses(List<Account> accounts)
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
    }
}
