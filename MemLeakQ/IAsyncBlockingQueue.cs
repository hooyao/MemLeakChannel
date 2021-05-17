using System.Threading;
using System.Threading.Tasks;

namespace MemLeakQ
{
    /// <summary>
    /// The interface that describe an async blocking queue./>
    /// This queue may be replaced by Akka implementation later.
    /// </summary>
    /// <typeparam name="T">Type of the elements stored in the queue.</typeparam>
    public interface IAsyncBlockingQueue<T>
    {
        /// <summary>
        /// Write (produce) data to the queue.
        /// </summary>
        /// <param name="data">The data to write.</param>
        /// <param name="timeoutInMs">Time out duration of this operation.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>If enqueue is successful.</returns>
        Task<bool> EnqueueAsync(
            T data,
            int timeoutInMs = -1,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Read (consume) data from this queue.
        /// </summary>
        /// <param name="timeoutInMs">Time out duration of this operation in milliseconds.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The retrieved data if there is.</returns>
        Task<T> DequeueAsync(
            int timeoutInMs = -1,
            CancellationToken cancellationToken = default);
    }
}
