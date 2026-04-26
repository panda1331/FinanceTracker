using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceTracker.Shared.Exceptions
{
    public class ValidationException : Exception
    {
        public ValidationException() { }
        public ValidationException(string message) : base(message) { }
    }
}
