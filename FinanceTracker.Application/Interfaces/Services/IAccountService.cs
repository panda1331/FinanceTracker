using FinanceTracker.Application.DTOs.Requests;
using FinanceTracker.Application.DTOs.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceTracker.Application.Interfaces.Services
{
    public interface IAccountService
    {
        Task<AccountResponse> CreateAccountAsync(Guid userId, CreateAccountRequest request);
        Task<List<AccountResponse>> GetAccountsByUserIdAsync(Guid userId);
    }
}
