namespace Exchange.Core.Constants
{
    public static class RoleRestrictions
    {
        public const string OnlyAdmin = "Administrator";
        public const string Privileged = "Administrator, Operator";
        public const string ContentEditors = "Administrator, Operator, Provider";
        public const string AnyEnabled = "Administrator, Operator, Customer, Provider";

    }
}
