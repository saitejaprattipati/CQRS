using System;
using System.Collections.Generic;

namespace Author.Core.Framework.ExceptionHandling
{
    [Serializable]
    public class RulesException : Exception
    {
        private readonly List<ErrorInfo> _errors;

        public RulesException(string propertyName, string errorMessage, string prefix = "")
        {
            _errors = Errors;
            _errors.Add(new ErrorInfo($"{prefix}{propertyName}", errorMessage));
        }

        public RulesException(string propertyName, string errorMessage, object onObject, string prefix = "")
        {
            _errors = Errors;
            _errors.Add(new ErrorInfo($"{prefix}{propertyName}", errorMessage, onObject));
        }

        public RulesException()
        {
            _errors = Errors;
        }

        public RulesException(List<ErrorInfo> errors)
        {
            _errors = errors;
        }

        public List<ErrorInfo> Errors
        {
            get
            {
                return _errors ?? new List<ErrorInfo>();
            }
        }
    }
}
