using Exchange.Constants;

namespace Exchange.Models
{
    public class ErrorModel
    {
        public ErrorModel(ErrorTypes errorTypes, string rawMessage)
        {
            RawMessage = rawMessage;
            ErrorTypes = errorTypes;
        }

        public string RawMessage { get; set; }
        public ErrorTypes ErrorTypes { get; set; }
    }
}
