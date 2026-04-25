using FinanceTracker.Domain.Enums;

namespace FinanceTracker.Application.DTOs.Requests
{
    public class CreateCategoryRequest
    {
        public string Name { get; set; }
        public OperationType Type { get; set; }
    }
}
