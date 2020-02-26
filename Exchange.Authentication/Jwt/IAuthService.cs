using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using System.Threading.Tasks;
using Exchange.Authentication.Jwt.Models;
using Exchange.Core.Models.Dto;
using Exchange.Core.ViewModels;

namespace Exchange.Authentication.Jwt
{
    public interface IAuthService
    {
        Task<UserVm?> GetCurrentUserAsync([NotNull] IEnumerable<Claim> claims);
        Task<AuthDto> LoginAsync([NotNull] string username, [NotNull] string password);
        Task<AuthDto> RefreshTokenAsync(string? refreshToken);
    }
}
