using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json.Serialization;
using Exchange.Controllers;
using Exchange.Services;
using Exchange.Utils.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace Exchange
{
    public class Startup
    {
        static Startup()
        {
            IdentityModelEventSource.ShowPII = true;
        }

        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(options =>
            {
                options.UseGeneralRoutePrefix("api");
            }).AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
            services.AddSingleton<JwtSecurityTokenHandler>();
            services.AddSingleton<ErrorMessageService>();
            services.AddCors(options =>
                options.AddPolicy("Dev", builder => { builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); }));
            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtAuthHandler.JwtAuthScheme;
                    options.DefaultChallengeScheme = JwtAuthHandler.JwtAuthScheme;
                    options.DefaultForbidScheme = JwtAuthHandler.JwtAuthScheme;
                    options.DefaultSignInScheme = JwtAuthHandler.JwtAuthScheme;
                    options.DefaultSignOutScheme = JwtAuthHandler.JwtAuthScheme;
                })
                .AddScheme<JwtOptions, JwtAuthHandler>(JwtAuthHandler.JwtAuthScheme, a =>
                {
                    a.TokenValidationParameters = new TokenValidationParameters
                    {
                        IssuerSigningKey =
                            new SymmetricSecurityKey(Encoding.ASCII.GetBytes("01234567012345670123456701234567")),
                        ValidateAudience = true,
                        ValidAudience = "ExchangeApp",
                        ValidateIssuer = true,
                        ValidIssuer = "Exchange",
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };
                });
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "My API", Version = "v1"
                });
                var openApiSecurityScheme = new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                };
                options.AddSecurityDefinition("Bearer", openApiSecurityScheme);
                var openApiSecurityRequirement = new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        },
                        new List<string>()
                    }
                };
                options.AddSecurityRequirement(openApiSecurityRequirement);
            });
            services.AddDbContext<ExchangeDbContext>(builder =>
                {
                    builder.UseMySql("server=localhost;database=Exchange;user=root;password=12345678");
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseCors("Dev");
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Exchange");
                c.RoutePrefix = string.Empty;
            });
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
