using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceTracker.Application.DTOs.Responses
{
    public class BudgetResponse
    {
        public Guid Id { get; set; }
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
        public decimal Limit { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public bool IsExceeded { get; set; }
    }
}
