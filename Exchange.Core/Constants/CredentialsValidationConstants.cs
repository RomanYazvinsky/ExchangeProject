namespace Exchange.Core.Constants
{
    public static class CredentialsValidationConstants
    {
        public const int MinimalUsernameLength = 3;
        public const int MaximumUsernameLength = 64;
        public const int MinimalPasswordLength = 6;
        public const int MaximumPasswordLength = 64;
        /// <summary>
        /// http://emailregex.com/
        /// </summary>
        public const string EmailRegex = "^(?(\")(\".+?(?<!\\\\)\"@)|(([0-9a-z]((\\.(?!\\.))|[-!#\\$%&'\\*\\+/=\\?\\^`\\{\\}\\|~\\w])*)(?<=[0-9a-z])@))(?(\\[)(\\[(\\d{1,3}\\.){3}\\d{1,3}\\])|(([0-9a-z][-\\w]*[0-9a-z]*\\.)+[a-z0-9][\\-a-z0-9]{0,22}[a-z0-9]))$";
        public const string UsernameRegex = "^[a-zA-Z0-9]+$";

    }
}
