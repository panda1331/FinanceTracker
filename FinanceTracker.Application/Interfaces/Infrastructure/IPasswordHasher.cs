using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceTracker.Application.Interfaces.Infrastructure
{
    public interface IPasswordHasher
    {
        string HashPassword(string password);
        bool VerifyPassword(string password, string hash);
    }
}
