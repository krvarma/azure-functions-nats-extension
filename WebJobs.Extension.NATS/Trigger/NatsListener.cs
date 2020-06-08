using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Executors;
using Microsoft.Azure.WebJobs.Host.Listeners;
using MyNatsClient.Rx;

namespace WebJobs.Extension.Nats
{
    /*
        The NatsListner class
        Implements the <c>IListener</c> interface. Contains the code to connect
        to a NATS server and subscribe a Channel.
     */
    public class NatsListener: IListener
    {
        private readonly ITriggeredFunctionExecutor _executor;
        private readonly NatsTriggerContext _context;

        /// <summary>
        /// NatsListener constructor
        /// </summary>
        ///
        /// <param name="executor"><c>ITriggeredFunctionExecutor</c> instance</param>
        /// <param name="context"><c>NatsTriggerContext</c> instance</param>
        ///
        public NatsListener(ITriggeredFunctionExecutor executor, NatsTriggerContext context)
        {
            _executor = executor;
            _context = context;
        }

        /// <summary>
        /// Cancel any pending operation
        /// </summary>
        public void Cancel()
        {
            if (_context == null || _context.Client == null) return;

            _context.Client.Disconnect();
        }

        /// <summary>
        ///  Dispose method
        /// </summary>
        public void Dispose()
        {
            _context.Client.Dispose();
        }

        /// <summary>
        /// Start the listener asynchronously. Subscribe to NATS channel and
        /// wait for message. When a message is received, execute the function
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>A Task returned from Subscribe method</returns>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            return _context.Client.Subscribe(_context.TriggerAttribute.Channel, _context.TriggerAttribute.QueueGroup, stream => stream.Subscribe(msg => {
                var triggerData = new TriggeredFunctionData
                {
                    TriggerValue = msg.GetPayloadAsString()
                };

                var task = _executor.TryExecuteAsync(triggerData, CancellationToken.None);
                task.Wait();
            }));
        }

        /// <summary>
        /// Stop current listening operation
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns></returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.Run(() =>{
                _context.Client.Disconnect();
            });
        }
    }
}
