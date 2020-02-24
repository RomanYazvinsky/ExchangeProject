namespace Exchange.Core.Constants.Errors
{
    public enum MailConfirmationResult
    {
        Ok,
        InvalidConfirmationUrl,
        InvalidConfirmationId,
        AlreadyConfirmed,
        EmailConfirmationServiceUnavailable
    }
}
