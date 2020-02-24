using System.Collections.Generic;
using Exchange.Core.Constants;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;

namespace Exchange.Configuration
{
    public static class SwaggerConfigurationExtensions
    {
        public static void ConfigureSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "My API", Version = "v1"
                });
                var openApiSecurityScheme = new OpenApiSecurityScheme
                {
                    Name = HeaderNames.Authorization,
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = AuthenticationConstants.AuthenticationHeader
                };
                options.AddSecurityDefinition(AuthenticationConstants.AuthenticationHeader, openApiSecurityScheme);
                var openApiSecurityRequirement = new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = AuthenticationConstants.AuthenticationHeader
                            },
                            Scheme = AuthenticationConstants.OAuth2AuthenticationScheme,
                            Name = AuthenticationConstants.AuthenticationHeader,
                            In = ParameterLocation.Header,
                        },
                        new List<string>()
                    }
                };
                options.AddSecurityRequirement(openApiSecurityRequirement);
            });
        }

        public static void UseConfiguredSwagger(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Exchange");
                c.RoutePrefix = string.Empty;
            });
        }
    }
}
