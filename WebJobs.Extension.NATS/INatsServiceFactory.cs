namespace WebJobs.Extension.Nats
{
    /// <summary>
    /// Nats Service factory. Create Nats Client and return
    /// </summary>
    public interface INatsServiceFactory
    {
        /// <summary>
        /// Create Nats Client from connection string
        /// </summary>
        /// <param name="connstring">Nats Connection string</param>
        /// <returns>Returns NatsClient instance</returns>
        public NatsClient CreateNatsClient(string connstring);
    }
}
