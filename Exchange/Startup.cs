using System.IdentityModel.Tokens.Jwt;
using System.Text.Json.Serialization;
using Exchange.Authentication.Jwt;
using Exchange.Authentication.Jwt.Impl;
using Exchange.Configuration;
using Exchange.Core.Constants;
using Exchange.Core.Services;
using Exchange.Core.Services.Impl;
using Exchange.Core.Services.Impl.EmailConfirmation;
using Exchange.Core.Services.Impl.ErrorMessages;
using Exchange.Core.Services.Options;
using Exchange.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Exchange
{
    public class Startup
    {

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<EmailConfirmationOptions>(Configuration.GetSection(ConfigurationConstants.EmailConfirmationSection));
            var jwtSettings = Configuration.GetSection(ConfigurationConstants.AuthorizationSection);
            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
            services.SetJwtAuthenticationAsDefault().AddJwtAuthorization(jwtSettings);
            services.AddSingleton<JwtSecurityTokenHandler>();
            services.AddSingleton<IErrorMessageService, ErrorMessageService>();
            services.AddSingleton<IEmailService, EmailService>();
            services.AddScoped<ICredentialValidationService, CredentialValidationService>();
            services.AddScoped<IUserRegistrationService, UserRegistrationService>();
            services.AddScoped<IAuthService, JwtAuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddHostedService<EmailSenderService>();
            services.AddCors(options =>
                options.AddPolicy(AuthenticationConstants.CorsPolicyName,
                    builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));
            services.ConfigureSwagger();
            services.AddDbContext<ExchangeDbContext>(builder =>
            {
                builder.UseMySql(Configuration[ConfigurationConstants.DatabaseConnectionStringProperty]);
            });
        }

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
            app.UseConfiguredSwagger();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}
