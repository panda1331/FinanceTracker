using FinanceTracker.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceTracker.Domain.Models
{
    public class Account : Entity
    {
        public AccountType Type { get; private set; }
        public string Name { get; private set; }
        public Guid UserId { get; private set; }
        public decimal Balance { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public Account() 
        {
            Name = string.Empty;
        }
        public Account(AccountType type, string name, Guid userId)
        {
            Type = type;
            Name = name;
            UserId = userId;
            Balance = 0m;
            CreatedAt = DateTime.UtcNow;
        }

        public void AddToBalance(decimal amount)
        {
            Balance += amount;
        }
        public void SubtractFromBalance(decimal amount)
        {
            Balance -= amount;
        }

        public void UpdateName(string name)
        {
            if (!string.IsNullOrEmpty(name)) 
                Name = name;
        }
    }
}
