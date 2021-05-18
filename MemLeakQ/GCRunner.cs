using System;
using System.Threading;
using System.Threading.Tasks;

namespace MemLeakQ
{
    public class GCRunner : AbstractTaskRunnable
    {

        public GCRunner()
        {
        }

        protected override async Task Loop(CancellationToken cancellationToken)
        {
            System.GC.Collect();
            Console.WriteLine("Performed GC");
            await Task.Delay(TimeSpan.FromSeconds(5));
        }

        protected override async Task HandleException(Exception ex, CancellationToken cancellationToken)
        {
        }
    }
}