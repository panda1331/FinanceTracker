using FinanceTracker.Domain.Enums;
using FinanceTracker.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceTracker.Application.Interfaces.Infrastructure
{
    public interface ITokenGenerator
    {
        string GenerateToken(Guid userId, string email, RoleType role);
    }
}
