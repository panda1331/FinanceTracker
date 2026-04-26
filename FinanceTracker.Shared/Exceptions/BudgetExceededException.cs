using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceTracker.Shared.Exceptions
{
    public class BudgetExceededException : Exception
    {
        public BudgetExceededException() { }
        public BudgetExceededException(string message) : base(message) { }
    }
}
