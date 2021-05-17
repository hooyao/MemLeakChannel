using System;
using System.Threading;
using System.Threading.Tasks;

namespace MemLeakQ
{
    /// <summary>
    /// The base class of the long-run services
    /// This class should be replaced by Akka.net.
    /// </summary>
    public abstract class AbstractTaskRunnable : IRunnable
    {
        private volatile bool _keepRunning = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractTaskRunnable"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        protected AbstractTaskRunnable()
        {
        }

        public async Task Start(CancellationToken cancellationToken = default)
        {
            while (this.IsContinue(cancellationToken))
            {
                try
                {
                    await this.Loop(cancellationToken).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    await this.HandleException(ex, cancellationToken).ConfigureAwait(false);
                }
                finally
                {
                    await this.Cleanup(cancellationToken).ConfigureAwait(false);
                }
            }

            await Console.Error.WriteLineAsync($"{this.GetType().Name} stops");
        }

        public Task Stop()
        {
            this._keepRunning = false;
            return Task.CompletedTask;
        }

        protected bool IsContinue(CancellationToken token)
        {
            return !token.IsCancellationRequested && this._keepRunning;
        }

        /// <summary>
        /// The loop logic to be implemented.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        protected abstract Task Loop(CancellationToken cancellationToken);

        /// <summary>
        /// The exception handling logic to be implemented.
        /// </summary>
        /// <param name="ex">The exception to be handled.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        protected abstract Task HandleException(Exception ex, CancellationToken cancellationToken);

        /// <summary>
        /// The clean up logic to be implemented.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        protected virtual Task Cleanup(CancellationToken cancellationToken)
        {
            // default no cleanup
            return Task.CompletedTask;
        }
    }
}
