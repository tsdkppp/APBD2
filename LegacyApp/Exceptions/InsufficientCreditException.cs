using System;

namespace LegacyApp
{
    public class InsufficientCreditException : Exception
    {
        public InsufficientCreditException(string message) :
            base(message)
        {
        }

        public InsufficientCreditException()
        {
        }
    }
}