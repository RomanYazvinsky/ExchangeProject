using System;
using Exchange.Constants;

namespace Exchange.Services
{
    public class LocalizedError: Exception
    {
        public ErrorTypes Type { get; }
        public override string Message { get; }

        public LocalizedError(ErrorTypes type, string message, Exception? innerException): base(message, innerException)
        {
            Type = type;
            Message = innerException == null ? message : message + Environment.NewLine + innerException.Message;
        }


    }

    public class ErrorMessageService
    {
        public string GetErrorMessage(ErrorTypes types)
        {
            return types.ToString();
        }
        public LocalizedError BuildError(ErrorTypes type, Exception innerException = null)
        {
            return new LocalizedError(type, GetErrorMessage(type), innerException);
        }
    }
}
