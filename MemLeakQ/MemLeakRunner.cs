using System;
using System.Threading;
using System.Threading.Tasks;

namespace MemLeakQ
{
    public class MemLeakRunner : AbstractTaskRunnable
    {
        private readonly IAsyncBlockingQueue<string> _inputQueue;

        public MemLeakRunner(IAsyncBlockingQueue<string> inputQueue)
        {
            this._inputQueue = inputQueue;
        }

        protected override async Task Loop(CancellationToken cancellationToken)
        {
            string message = await this._inputQueue.DequeueAsync(10, cancellationToken).ConfigureAwait(false);
            Console.Write(message);
        }

        protected override async Task HandleException(Exception ex, CancellationToken cancellationToken)
        {
            switch (ex)
            {
                case ObjectDisposedException _:
                case OperationCanceledException _:
                {
                    // do something
                }

                    break;
                default:
                {
                    await Console.Error.WriteLineAsync("Run into unexpected exception.");
                    break;
                }
            }
        }
    }
}