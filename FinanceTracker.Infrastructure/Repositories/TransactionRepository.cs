using FinanceTracker.Application.Repository;
using FinanceTracker.Domain.Models;
using FinanceTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceTracker.Infrastructure.Repositories
{
    public class TransactionRepository : Repository<Transaction>, ITransactionRepository
    {
        public TransactionRepository(AppDbContext context) : base(context) { }

        public async Task<List<Transaction>> GetByUserIdAndPeriodAsync(Guid userId, int month, int year, CancellationToken cancellationToken = default)
        {
            return await _context.Transactions
                .Where(t => t.UserId == userId && t.Date.Month == month && t.Date.Year == year)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Transaction>> GetByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default)
        {
            return await _context.Transactions
                .Where(t => t.AccountId == accountId)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Transaction>> GetByCategoryAndPeriodAsync(Guid userId, Guid categoryId, int month, int year, CancellationToken cancellationToken = default)
        {
            return await _context.Transactions
                .Where(t => t.UserId ==  userId && t.CategoryId == categoryId && t.Date.Month == month && t.Date.Year == year)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Transaction>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _context.Transactions
                .Where(t => t.UserId == userId)
                .ToListAsync(cancellationToken);
        }
    }
}
