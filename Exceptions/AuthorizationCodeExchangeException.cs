using System;
using PMAuth.Exceptions.Models;
#pragma warning disable 1591

namespace PMAuth.Exceptions
{
    public class AuthorizationCodeExchangeException : Exception
    {
        public ErrorModel Description { get; }
        public AuthorizationCodeExchangeException()
        {
        }

        public AuthorizationCodeExchangeException(string message)
            : base(message)
        {
        }

        public AuthorizationCodeExchangeException(string message, Exception inner)
            : base(message, inner)
        {
        }

        public AuthorizationCodeExchangeException(string message, ErrorModel model) 
            : base(message)
        {
            Description = model;
        }
        public AuthorizationCodeExchangeException(string message, ErrorModel model, Exception inner) 
            : base(message, inner)
        {
            Description = model;
        }
    }
}