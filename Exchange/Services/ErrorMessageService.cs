using System;
using Exchange.Constants;

namespace Exchange.Services
{
    public class LocalizedError: Exception
    {
        public ErrorTypes Type { get; }
        public string Message { get; }

        public LocalizedError(ErrorTypes type, string message)
        {
            Type = type;
            Message = message;
        }
    }

    public class ErrorMessageService
    {
        public string GetErrorMessage(ErrorTypes types)
        {
            return types.ToString();
        }
        public LocalizedError BuildError(ErrorTypes type)
        {
            return new LocalizedError(type, GetErrorMessage(type));
        }
    }
}
