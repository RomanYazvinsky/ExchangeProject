using System;
using System.Text;
using Exchange.Constants;
using Exchange.Services.Authentication;
using Exchange.Services.Authentication.Options;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Exchange.Configuration
{
    public static class AuthenticationConfigurationExtensions
    {
        public static AuthenticationBuilder SetJwtAuthenticationAsDefault(this IServiceCollection collection)
        {
            return collection.AddAuthentication(options =>
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
            IConfigurationSection configurationSection
        )
        {
            var key = configurationSection["SigningKey"];
            var keyBytes = Encoding.ASCII.GetBytes(key);
            return authenticationBuilder.AddScheme<JwtOptions, JwtAuthHandler>(
                AuthenticationConstants.JwtAuthenticationScheme, a =>
                {
                    a.SingingKey = key;
                    a.TokenValidationParameters = new TokenValidationParameters
                    {
                        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
                        ValidateAudience = true,
                        ValidAudience = configurationSection["ValidAudience"],
                        ValidateIssuer = true,
                        ValidIssuer = configurationSection["ValidIssuer"],
                        ValidateLifetime = true,
                    };
                });
        }
    }
}
