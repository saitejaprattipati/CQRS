namespace Author.Core.Framework.ExceptionHandling
{
    public class ErrorInfo
    {
        private readonly string _errorMessage;
        private readonly string _propertyName;
        private readonly object _onObject;

        public ErrorInfo(string propertyName, string errorMessage)
        {
            _propertyName = propertyName;
            _errorMessage = errorMessage;
            _onObject = null;
        }

        public ErrorInfo(string propertyName, string errorMessage, object onObject)
        {
            _propertyName = propertyName;
            _errorMessage = errorMessage;
            _onObject = onObject;
        }

        public string ErrorMessage
        {
            get
            {
                return _errorMessage;
            }
        }

        public string PropertyName
        {
            get
            {
                return _propertyName;
            }
        }
    }
}
