using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MemLeakQ
{
    class Program
    {
        static async Task Main(string[] args)
        {
            IAsyncBlockingQueue<string> inputQueue = new AsyncBlockingQueue<string>("testQ", 100);
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            List<Task> longRunTasks = new List<Task>();

            Task.WaitAll(longRunTasks.ToArray());
            for (int i = 0; i < 500; i++)
            {
                var runner = new MemLeakRunner(inputQueue);
                longRunTasks.Add(Task.Run(async () => await runner.Start(cancellationTokenSource.Token).ConfigureAwait(false)));
            }

            Task.WaitAll(longRunTasks.ToArray());
        }
    }
}