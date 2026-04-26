using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceTracker.Application.DTOs.Responses
{
    public class AnalyticsResponse
    {
        public decimal TotalExpense { get; set; }
        public List<AnalyticsItem> Items { get; set; } 
    }

    public class AnalyticsItem
    {
        public string CategoryName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public decimal? Limit { get; set; }
        public bool IsExceeded { get; set; }
    }
}
