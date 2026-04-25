using FinanceTracker.Domain.Enums;

namespace FinanceTracker.Application.DTOs.Requests
{
    public class CreateAccountRequest
    {
        public string Name { get; set; }
        public AccountType Type { get; set; }
    }
}
