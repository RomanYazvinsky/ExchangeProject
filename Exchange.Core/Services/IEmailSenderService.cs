using System.Threading;
using System.Threading.Tasks;

namespace Exchange.Core.Services
{
    public interface IEmailSenderService
    {
        Task StartAsync(CancellationToken cancellationToken);
        Task StopAsync(CancellationToken cancellationToken);
        void Dispose();
    }
}
