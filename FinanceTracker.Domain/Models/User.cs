using FinanceTracker.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceTracker.Domain.Models
{
    public class User : Entity
    {
        public string Email { get; private set; }
        public string PasswordHash { get; private set; }
        public RoleType Type { get; private set; }
        public DateTime CreatedAt { get; private set; }

        public User() 
        {
            PasswordHash = string.Empty;
        }
        public User(string email, RoleType type)
        {
            Email = email;
            PasswordHash = string.Empty;
            Type = type;
            CreatedAt = DateTime.UtcNow;
        }

        public void SetPasswordHash(string passwordHash)
        {
            if (!string.IsNullOrEmpty(passwordHash)) 
                PasswordHash = passwordHash;
        }
    }
}
