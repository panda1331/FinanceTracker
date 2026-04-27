using FinanceTracker.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceTracker.Domain.Models
{
    public class Category : Entity
    {
        public Guid? UserId { get; private set; }
        public string Name { get; private set; }
        public OperationType Type { get; private set; }
        public bool IsDefault { get; private set; }

        public Category()
        {
            Name = string.Empty;
        }
        public Category(Guid? userId, string name, OperationType type, bool isDefault)
        {
            UserId = userId;
            Name = name;
            Type = type;
            IsDefault = isDefault;
        }

        public void UpdateName(string name)
        {
            if (!string.IsNullOrEmpty(name)) 
                Name = name;
        }
    }
}
