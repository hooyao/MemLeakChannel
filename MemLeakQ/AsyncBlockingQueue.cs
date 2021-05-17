using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace MemLeakQ
{
    /// <summary>
    /// Queue for producer-consumer pattern
    /// Safely supporting writing (producing) and reading (consuming) data asynchronously by different objects.
    /// </summary>
    /// <typeparam name="T">The type of data to transmit.</typeparam>
    public class AsyncBlockingQueue<T> : QueueHolder, IDisposable, IAsyncBlockingQueue<T>
    {
        private readonly Channel<T> _channel;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncBlockingQueue{T}"/> class.
        /// </summary>
        /// <param name="name">The queue name (identifier).</param>
        /// <param name="capacity">The desired capacity of the queue.</param>
        public AsyncBlockingQueue(string name, int capacity)
        {
            this.Name = name;
            this._channel = Channel.CreateBounded<T>(
                new BoundedChannelOptions(capacity) { FullMode = BoundedChannelFullMode.Wait, SingleWriter = false, SingleReader = false });
        }

        /// <summary>
        /// Gets the queue name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Dispose this queue.
        /// </summary>
        public void Dispose()
        {
            this._channel.Writer.TryComplete();
        }

        public async Task<bool> EnqueueAsync(
            T data,
            int timeoutInMs = -1,
            CancellationToken cancellationToken = default)
        {
            using (CancellationTokenSource cts = this.GetCancellationTokenSource(timeoutInMs, cancellationToken))
            {
                try
                {
                    await this._channel.Writer.WriteAsync(data, cancellationToken);
                    return true;
                }
                catch (ChannelClosedException cce)
                {
                    await Console.Error.WriteLineAsync("Channel is closed.");
                }
                catch (Exception ex)
                {
                    await Console.Error.WriteLineAsync("Enqueue failed.");
                }

                return false;
            }
        }

        public async Task<T> DequeueAsync(
            int timeoutInMs = -1,
            CancellationToken cancellationToken = default)
        {
            try
            {
                using (CancellationTokenSource cts = this.GetCancellationTokenSource(timeoutInMs, cancellationToken))
                {
                    T value = await this._channel.Reader.ReadAsync(cts?.Token ?? cancellationToken).ConfigureAwait(false);
                    return value;
                }
            }
            catch (ChannelClosedException cce)
            {
                await Console.Error.WriteLineAsync("Channel is closed.");
                throw new ObjectDisposedException("Queue is disposed");
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                await Console.Error.WriteLineAsync("Dequeue failed.");
                throw;
            }
        }

        /// <summary>
        /// Creates a <see cref="CancellationTokenSource"/> for timeout.
        /// </summary>
        /// <param name="timeoutInMs">The timeout value in milliseconds.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="CancellationTokenSource"/> with timeout.</returns>
        private CancellationTokenSource GetCancellationTokenSource(
            int timeoutInMs,
            CancellationToken cancellationToken)
        {
            if (timeoutInMs <= 0)
            {
                return null;
            }

            CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(TimeSpan.FromMilliseconds(timeoutInMs));
            return cts;
        }
    }
}