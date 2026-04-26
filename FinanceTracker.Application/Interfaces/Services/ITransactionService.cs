using FinanceTracker.Application.DTOs.Requests;
using FinanceTracker.Application.DTOs.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceTracker.Application.Interfaces.Services
{
    public interface ITransactionService
    {
        Task<TransactionResponse> CreateTransactionAsync(Guid userId, CreateTransactionRequest request);
        Task<List<TransactionResponse>> GetTransactionsByUserIdAsync(Guid userId);
    }
}
