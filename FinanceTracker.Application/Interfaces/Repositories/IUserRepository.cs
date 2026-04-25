using FinanceTracker.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceTracker.Application.Repository
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    }
}
