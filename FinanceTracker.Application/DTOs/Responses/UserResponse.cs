using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceTracker.Application.DTOs.Responses
{
    public class UserResponse
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
