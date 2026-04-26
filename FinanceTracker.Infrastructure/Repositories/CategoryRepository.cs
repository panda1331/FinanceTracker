using FinanceTracker.Application.Repository;
using FinanceTracker.Domain.Models;
using FinanceTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceTracker.Infrastructure.Repositories
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(AppDbContext context) : base(context) { }

        public async Task<List<Category>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _context.Categories
                .Where(c => c.UserId == null || c.UserId == userId)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Category>> GetDefaultCategoriesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Categories
                .Where(c => c.IsDefault)
                .ToListAsync(cancellationToken);
        }
    }
}
