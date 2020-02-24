namespace Exchange.Core.Services.EmailConfirmation.Models
{
    public enum SendEmailResult
    {
        Ok,
        ConnectFailed,
        AuthFailed,
        SendFailed
    }
}
