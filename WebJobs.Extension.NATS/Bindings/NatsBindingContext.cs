namespace WebJobs.Extension.Nats.Bindings
{
    /// <summary>
    /// Nats Binding context. Contains attribute and nats client instance
    /// </summary>
    public class NatsBindingContext
    {
        /// <summary>
        /// Nats Binding attribute
        /// </summary>
        public NatsAttribute attribute;
        /// <summary>
        /// Nats Client instance
        /// </summary>
        public NatsClient client;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="attribute">Nats Binding Attribute</param>
        /// <param name="client">Nats Client</param>
        public NatsBindingContext(NatsAttribute attribute, NatsClient client)
        {
            this.attribute = attribute;
            this.client = client;
        }
    }
}
