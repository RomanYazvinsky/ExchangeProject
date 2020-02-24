using Exchange.Constants;

namespace Exchange.Models
{
    public class ErrorModel
    {
        public ErrorModel(AuthErrorTypes authErrorTypes, string rawMessage)
        {
            RawMessage = rawMessage;
            AuthErrorTypes = authErrorTypes;
        }

        public string RawMessage { get; set; }
        public AuthErrorTypes AuthErrorTypes { get; set; }
    }
}
