using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;

namespace WebJobs.Extension.Nats.Bindings
{
    /// <summary>
    /// Async Collector class. Responsible for publishing to a NATS channel
    /// </summary>
    /// <typeparam name="T">Data Type of value</typeparam>
    public class NatsAsyncCollector<T>: IAsyncCollector<T>
    {
        /// <summary>
        /// NatsBindingContext instance
        /// </summary>
        private readonly NatsBindingContext _context;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">NatsBindingContext instance</param>
        public NatsAsyncCollector(NatsBindingContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Publish message to a NATS chanel
        /// </summary>
        /// <param name="message">Message to be published</param>
        /// <param name="cancellationToken">A Cancellation Token</param>
        /// <returns>A Task that completes when the message us published</returns>
        public Task AddAsync(T message, CancellationToken cancellationToken = default)
        {
            return _context.client.Publish(_context.attribute.Channel, message.ToString());
        }

        /// <summary>
        /// Flush any pending publish
        /// </summary>
        /// <param name="cancellationToken">A Cancellation token/param>
        /// <returns></returns>
        public Task FlushAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}
