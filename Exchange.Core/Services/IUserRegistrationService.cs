using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Exchange.Core.Constants;
using Exchange.Core.Constants.Errors;
using Exchange.Core.ViewModels;

namespace Exchange.Core.Services
{
    public interface IUserRegistrationService
    {
        Task<(UserVm? User, Dictionary<CredentialProperty, IEnumerable<string>> ErrorMessages)> RegisterUser(
            string? username,
            string? password,
            string? email
        );

        MailConfirmationResult SendConfirmationEmail([NotNull] UserVm user);
        Task<MailConfirmationResult> ConfirmEmail(string? userIdString);
    }
}
