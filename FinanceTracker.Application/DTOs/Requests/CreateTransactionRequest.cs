using FinanceTracker.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceTracker.Application.DTOs.Requests
{
    public class CreateTransactionRequest
    {
        public Guid AccountId { get; set; }
        public Guid CategoryId { get; set; }
        public OperationType Type { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string? Description { get; set; }
    }
}
