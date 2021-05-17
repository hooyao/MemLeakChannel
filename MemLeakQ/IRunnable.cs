using System.Threading;
using System.Threading.Tasks;

namespace MemLeakQ
{
    /// <summary>
    /// Interface of a long-run service.
    /// </summary>
    public interface IRunnable
    {
        /// <summary>
        /// Start the long-reun service.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token which would stop this service once set.</param>
        /// <returns>Async Task.</returns>
        Task Start(CancellationToken cancellationToken);

        /// <summary>
        /// Stop this service.
        /// </summary>
        /// <returns>Async Task.</returns>
        Task Stop();
    }
}