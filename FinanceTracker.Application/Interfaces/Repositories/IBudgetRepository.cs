using FinanceTracker.Domain.Models;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace FinanceTracker.Application.Repository
{
    public interface IBudgetRepository : IRepository<Budget>
    {
        Task<List<Budget>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<Budget?> GetByCategoryAndMonthAsync(Guid userId, Guid categoryId, int month, int year, CancellationToken cancellationToken = default);
    }
}
