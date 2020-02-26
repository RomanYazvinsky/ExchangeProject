using System;
using Exchange.Data.Constants;

namespace Exchange.Authentication.Jwt
{
    public interface ITokenFactory
    {
        string BuildAccessToken(Guid userId, Role role);
        string BuildRefreshToken(Guid userId);
    }
}
