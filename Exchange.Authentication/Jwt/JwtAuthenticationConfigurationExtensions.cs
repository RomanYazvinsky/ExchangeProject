using System;
using System.Text;
using Exchange.Authentication.Jwt.Impl;
using Exchange.Authentication.Jwt.Models;
using Exchange.Core.Constants;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Exchange.Authentication.Jwt
{
    public static class JwtAuthenticationConfigurationExtensions
    {
        public const string JwtOptionsSigningKey = "SigningKey";
        public const string JwtOptionsAccessExpiration = "AccessExpirationSeconds";
        public const string JwtOptionsRefreshExpiration = "RefreshExpirationSeconds";
        public const string JwtOptionsValidAudience = "ValidAudience";
        public const string JwtOptionsValidIssuer = "ValidIssuer";

        public static AuthenticationBuilder SetJwtAuthenticationAsDefault(this IServiceCollection services)
        {
            return services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = AuthenticationConstants.JwtAuthenticationScheme;
                options.DefaultChallengeScheme = AuthenticationConstants.JwtAuthenticationScheme;
                options.DefaultForbidScheme = AuthenticationConstants.JwtAuthenticationScheme;
                options.DefaultSignInScheme = AuthenticationConstants.JwtAuthenticationScheme;
                options.DefaultSignOutScheme = AuthenticationConstants.JwtAuthenticationScheme;
            });
        }

        public static AuthenticationBuilder AddJwtAuthorization(
            this AuthenticationBuilder authenticationBuilder,
            IConfiguration configurationSection
        )
        {
            var key = configurationSection[JwtOptionsSigningKey];
            var keyBytes = Encoding.ASCII.GetBytes(key);
            var accessTokenExpiration = Convert.ToInt32(configurationSection[JwtOptionsAccessExpiration]);
            var refreshTokenExpiration = Convert.ToInt32(configurationSection[JwtOptionsRefreshExpiration]);
            authenticationBuilder.Services.AddSingleton<ITokenFactory, JwtTokenFactory>();
            authenticationBuilder.Services.AddScoped<ITokenValidator, JwtTokenValidator>();
            return authenticationBuilder.AddScheme<JwtOptions, JwtAuthTokenValidationHandler>(
                AuthenticationConstants.JwtAuthenticationScheme, options =>
                {
                    options.SigningKey = key;
                    options.AccessTokenExpiration = TimeSpan.FromSeconds(accessTokenExpiration);
                    options.RefreshTokenExpiration = TimeSpan.FromSeconds(refreshTokenExpiration);
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
                        ValidateAudience = true,
                        ValidAudience = configurationSection[JwtOptionsValidAudience],
                        ValidateIssuer = true,
                        ValidIssuer = configurationSection[JwtOptionsValidIssuer],
                        ValidateLifetime = true
                    };
                });
        }
    }
}
