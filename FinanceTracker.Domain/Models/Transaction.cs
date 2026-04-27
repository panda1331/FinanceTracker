using FinanceTracker.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceTracker.Domain.Models
{
    public class Transaction : Entity
    {
        public Guid UserId { get; private set; }
        public Guid AccountId { get; private set; }
        public Guid CategoryId { get; private set; }
        public OperationType Type { get; private set; }
        public decimal Amount { get; private set; }
        public string Description { get; private set; }
        public DateTime Date { get; private set; }
        public DateTime CreatedAt { get; private set; }

        public Transaction() 
        {
            Description = string.Empty;
        }
        public Transaction(Guid userId, Guid accountId, Guid categoryId, OperationType type, decimal amount, DateTime date)
        {
            UserId = userId;
            AccountId = accountId;
            CategoryId = categoryId;
            Type = type;
            Amount = amount;
            Description = string.Empty;
            Date = date;
            CreatedAt = DateTime.UtcNow;
        }

        public void UpdateDescription(string description)
        {
            if (!string.IsNullOrEmpty(description))
                Description = description;
        }
    }
}
