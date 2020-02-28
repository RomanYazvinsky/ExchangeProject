namespace Exchange.Core.Constants.Errors
{
    public enum UserValidationResult
    {
        Ok,
        UserNotFound,
        InvalidPassword,
        InvalidRefreshToken,
        ExpiredRefreshToken
    }
}
