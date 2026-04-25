using FinanceTracker.Application.Repository;
using FinanceTracker.Domain.Models;
using FinanceTracker.Infrastructure.Data;
using FinanceTracker.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceTracker.Infrastructure.Repositories
{
    public class BudgetRepository : Repository<Budget>, IBudgetRepository
    {
        public BudgetRepository(AppDbContext context) : base(context) { }

        public async Task<Budget?> GetByCategoryAndMonthAsync(Guid userId, Guid categoryId, int month, int year, CancellationToken cancellationToken = default)
        {
            return await _context.Budgets
                .FirstOrDefaultAsync(b => b.UserId == userId
                    && b.CategoryId == categoryId
                    && b.Month == month
                    && b.Year == year, cancellationToken);
        }

        public async Task<List<Budget>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _context.Budgets
                .Where(b => b.UserId == userId)
                .ToListAsync(cancellationToken);
        }
    }
}
