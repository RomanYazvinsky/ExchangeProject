using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using MimeKit;

namespace Exchange.Core.Services
{
    public interface IEmailService
    {
        IEnumerable<MimeMessage> ResetQueue();

        void ScheduleEmail(
            [NotNull] string email,
            [NotNull] string sender,
            [NotNull] string subject,
            [NotNull] string message
        );

        string ComposeConfirmationEmail([NotNull] string confirmationId);

    }
}
