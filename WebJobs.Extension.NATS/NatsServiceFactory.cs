namespace WebJobs.Extension.Nats
{
    /// <summary>
    /// Nats Service Factory, responsible for creating NatsClient
    /// </summary>
    public class NatsServiceFactory : INatsServiceFactory
    {
        /// <summary>
        /// Create Nats Client from connection string
        /// </summary>
        /// <param name="connstring">Connection String</param>
        /// <returns>Returns NatsClient</returns>
        public NatsClient CreateNatsClient(string connstring)
        {
            NatsClient client = new NatsClient();

            client.Connect(connstring);

            return client;
        }
    }
}
