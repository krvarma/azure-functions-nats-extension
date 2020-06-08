using System;
using System.Threading.Tasks;
using MyNatsClient;
using MyNatsClient.Ops;

namespace WebJobs.Extension.Nats
{
    /// <summary>
    /// Nats Clinet utility class. This is wrapper class around the
    /// MyNatsClient library.
    /// </summary>
    public class NatsClient
    {
        /// <summary>
        /// MyNatsClient.NatsClient client instance
        /// </summary>
        private MyNatsClient.NatsClient _client = null;

        /// <summary>
        /// Connect to Nats server
        /// </summary>
        /// <param name="connstring">Nats Connection string</param>
        public void Connect(string connstring)
        {
            // Parse connection string
            var connparams = NatsUrlParser.Parse(connstring);

            MyNatsClient.Host host = new MyNatsClient.Host(connparams.host, connparams.port);

            // If the connection string has username and password, then use
            // that to authenticate Nats server
            if (connparams.HasCredentials())
            {
                host.Credentials = new MyNatsClient.Credentials(connparams.username, connparams.password);
            }

            // Create client
            MyNatsClient.ConnectionInfo _connectionInfo = new MyNatsClient.ConnectionInfo(host);
            _client = new MyNatsClient.NatsClient(_connectionInfo);

            Task conn = _client.ConnectAsync();

            conn.Wait();
        }

        /// <summary>
        /// Publish messages
        /// </summary>
        /// <param name="subject">Channel string</param>
        /// <param name="body">Message body</param>
        /// <param name="replyTo">Reply to address, null by default</param>
        /// <returns></returns>
        public Task Publish(string subject, string body, string replyTo = null)
        {
            return _client.PubAsync(subject, body, replyTo);
        }

        /// <summary>
        /// Subscribe to Nats channel
        /// </summary>
        /// <param name="subject">Channel string</param>
        /// <param name="subscription">Subscription lambda</param>
        /// <param name="queueGroup">QueueGroup name</param>
        /// <returns>Returns a Task that completes when the subscription ends</returns>
        public Task<ISubscription> Subscribe(string subject, string queueGroup, Func<INatsObservable<MsgOp>, IDisposable> subscription)
        {
            SubscriptionInfo sinfo = new SubscriptionInfo(subject, queueGroup);

            return _client.SubAsync(sinfo, subscription);
        }

        /// <summary>
        /// Disconnect from server
        /// </summary>
        public void Disconnect()
        {
            _client.Disconnect();
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            _client.Dispose();
        }
    }
}
    