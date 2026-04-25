using FinanceTracker.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceTracker.Application.Repository
{
    public interface IRepository<T> where T : Entity
    {
        Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);
        Task AddAsync(T entity, CancellationToken cancellationToken = default);
        void Update(T entity);
        void Delete(T  entity);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
