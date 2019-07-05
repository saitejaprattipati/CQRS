using System;

namespace Author.Core.Framework.ExceptionHandling
{
    public class CustomMessageException : Exception
    {
        private string _exceptionMessage = string.Empty;

        public string ExceptionMessage { get { return _exceptionMessage; } set { _exceptionMessage = value; } }

        public CustomMessageException() : base() { }

        public CustomMessageException(string exceptionMessage) : base(exceptionMessage)
        {
            _exceptionMessage = exceptionMessage;
        }

        public CustomMessageException(string exceptionMessage, string message) : base(message)
        {
            _exceptionMessage = exceptionMessage;
        }

        public CustomMessageException(string exceptionMessage, string message, Exception innerException) : base(message, innerException)
        {
            _exceptionMessage = exceptionMessage;
        }
    }
}
