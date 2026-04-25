using FinanceTracker.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceTracker.Application.Repository
{
    public interface ITransactionRepository : IRepository<Transaction>
    {
        Task<List<Transaction>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<List<Transaction>> GetByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default);
        Task<List<Transaction>> GetByCategoryAndPeriodAsync(Guid userId, Guid categoryId, int month, int year, CancellationToken cancellationToken = default);
    }
}
