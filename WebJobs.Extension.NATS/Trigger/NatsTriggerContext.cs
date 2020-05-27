namespace WebJobs.Extension.Nats
{
    /// <summary>
    /// Trigger context class
    /// </summary>
    public class NatsTriggerContext
    {
        /// <summary>
        /// <c>Attribute</c> instance
        /// </summary>
        public NatsTriggerAttribute TriggerAttribute;
        /// <summary>
        /// <c>NatsClient</c> instance to connect and subscribe to NATS
        /// </summary>
        public NatsClient Client;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="attribute">Attribute instnace</param>
        /// <param name="client">NatsClient instance</param>
        public NatsTriggerContext(NatsTriggerAttribute attribute, NatsClient client)
        {
            this.TriggerAttribute = attribute;
            this.Client = client;
        }
    }
}
