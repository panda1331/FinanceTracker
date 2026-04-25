using FinanceTracker.Domain.Enums;

namespace FinanceTracker.Application.DTOs.Responses
{
    public class CategoryResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public OperationType Type { get; set; }
        public bool IsDefault { get; set; }
    }
}
