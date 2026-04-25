using FinanceTracker.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceTracker.Application.Repository
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<List<Category>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<List<Category>> GetDefaultCategoriesAsync(CancellationToken cancellationToken = default);
    }
}
