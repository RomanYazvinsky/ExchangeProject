﻿using System;
using System.IdentityModel.Tokens.Jwt;
using Exchange.Authentication.Jwt.Models;
using Microsoft.IdentityModel.Tokens;

namespace Exchange.Authentication.Jwt.Impl
{
    public class JwtTokenValidator: ITokenValidator
    {
        private readonly JwtSecurityTokenHandler _tokenHandler;

        public JwtTokenValidator(JwtSecurityTokenHandler tokenHandler)
        {
            _tokenHandler = tokenHandler;
        }
        public TokenValidationInfo ValidateToken(JwtOptions options, string token)
        {
            try
            {
                var claimsPrincipal =
                    _tokenHandler.ValidateToken(token, options.TokenValidationParameters.Clone(), out _);
                return new TokenValidationInfo
                {
                    ValidationResult = AuthTokenValidationResult.Ok,
                    ClaimsPrincipal = claimsPrincipal
                };
            }
            catch (SecurityTokenExpiredException)
            {
                return new TokenValidationInfo
                {
                    ValidationResult = AuthTokenValidationResult.Expired
                };
            }
            catch (Exception)
            {
                return new TokenValidationInfo
                {
                    ValidationResult = AuthTokenValidationResult.Corrupted
                };
            }
        }
    }
}
