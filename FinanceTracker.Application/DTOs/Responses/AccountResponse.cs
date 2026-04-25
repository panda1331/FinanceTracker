using FinanceTracker.Domain.Enums;

namespace FinanceTracker.Application.DTOs.Responses
{
    public class AccountResponse
    {
        public Guid Id { get; set; }
        public AccountType Type { get; set; }
        public string Name { get; set; }
        public decimal Balance { get; set; }
    }
}
