using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json.Serialization;
using DatabaseModel;
using Exchange.Configuration;
using Exchange.Constants;
using Exchange.Services;
using Exchange.Utils.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;

namespace Exchange
{
    public class Startup
    {
        static Startup()
        {
            IdentityModelEventSource.ShowPII = true;
        }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(options => options.UseGeneralRoutePrefix("api"))
                .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
            services.AddSingleton<JwtSecurityTokenHandler>();
            services.AddSingleton<ErrorMessageService>();

            services.AddCors(options =>
                options.AddPolicy(AuthenticationConstants.CorsPolicyName, builder => {
                    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                }));
            services.SetJwtAuthenticationAsDefault()
                .AddJwtAuthorization(Configuration);
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
            services.AddDbContext<ExchangeDbContext>(builder =>
            {
                builder.UseMySql(Configuration["DatabaseConnectionString"]);
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
            app.UseCors(AuthenticationConstants.CorsPolicyName);
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
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}
