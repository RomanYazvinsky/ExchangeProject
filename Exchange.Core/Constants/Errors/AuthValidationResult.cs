namespace Exchange.Core.Constants.Errors
{
    public enum AuthValidationResult
    {
        Ok,
        UserNotFound,
        InvalidPassword,
        DeviceAuthRemoved,
        InvalidRefreshToken,
        ExpiredRefreshToken
    }
}
