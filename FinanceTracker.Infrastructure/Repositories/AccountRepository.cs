using FinanceTracker.Application.Repository;
using FinanceTracker.Domain.Models;
using FinanceTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceTracker.Infrastructure.Repositories
{
    public class AccountRepository : Repository<Account>, IAccountRepository
    {
        public AccountRepository(AppDbContext context) : base(context) { }

        public async Task<List<Account>> GetByUserIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Accounts
                .Where(a  => a.UserId == id)
                .ToListAsync(cancellationToken);
        }
    }
}
