using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceTracker.Application.DTOs.Requests
{
    public class CreateBudgetRequest
    { 
        public Guid CategoryId { get; set; }
        public decimal Limit { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
    }
}
