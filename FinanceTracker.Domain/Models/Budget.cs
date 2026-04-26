using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceTracker.Domain.Models
{
    public class Budget : Entity
    {
        public Guid UserId { get; private set; }
        public Guid CategoryId { get; private set; }
        public decimal Limit { get; private set; }
        public int Month { get; private set; }
        public int Year { get; private set; }
        public DateTime CreatedAt { get; private set; }

        public Budget() { }
        public Budget(Guid userId, Guid categoryId, decimal limit, int month, int year)
        {
            UserId = userId;
            CategoryId = categoryId;
            Limit = limit;
            Month = month;
            Year = year;
            CreatedAt = DateTime.UtcNow;
        }

        public void Update(decimal limit)
        {
            Limit = limit;
        }
    }
}
