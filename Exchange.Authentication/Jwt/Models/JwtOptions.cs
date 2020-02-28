﻿using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;

namespace Exchange.Authentication.Jwt.Models
{
    public class JwtOptions : AuthenticationSchemeOptions
    {
        public string SigningKey { get; set; }

        public TimeSpan RefreshTokenExpiration { get; set; }
        public TimeSpan AccessTokenExpiration { get; set; }
        public TokenValidationParameters? TokenValidationParameters { get; set; }
    }
}