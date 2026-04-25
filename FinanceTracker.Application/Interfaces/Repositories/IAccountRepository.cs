using FinanceTracker.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceTracker.Application.Repository
{
    public interface IAccountRepository : IRepository<Account>
    {
        Task<List<Account>> GetByUserIdAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
