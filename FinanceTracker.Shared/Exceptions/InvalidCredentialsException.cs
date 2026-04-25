using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceTracker.Shared.Exceptions
{
    public class InvalidCredentialsException : Exception
    {
        public InvalidCredentialsException() { }
        public InvalidCredentialsException(string message) : base(message) { }
    }
}
