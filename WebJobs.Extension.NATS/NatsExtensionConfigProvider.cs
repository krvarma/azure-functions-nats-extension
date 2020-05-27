using System.Collections.Concurrent;
using Microsoft.Azure.WebJobs.Host.Config;
using WebJobs.Extension.Nats.Bindings;

namespace WebJobs.Extension.Nats
{
    /// <summary>
    /// Extension Config Provider class
    /// </summary>
    public class NatsExtensionConfigProvider : IExtensionConfigProvider
    {
        /// <summary>
        /// Nats Service Factory, used to create context
        /// </summary>
        public INatsServiceFactory _serviceFactory;
        /// <summary>
        /// A ConcurrentDictionary to cache the clients. The clients are chached
        /// based on the connection string
        /// </summary>
        private ConcurrentDictionary<string, NatsClient> ClientCache { get; } = new ConcurrentDictionary<string, NatsClient>();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serviceFactory">Nats Service Factory instance</param>
        public NatsExtensionConfigProvider(INatsServiceFactory serviceFactory)
        {
            _serviceFactory = serviceFactory;
        }

        /// <summary>
        /// Initialize the extensions
        /// </summary>
        /// <param name="context">Extension config context</param>
        public void Initialize(ExtensionConfigContext context)
        {
            // Add trigger first
            var triggerRule = context.AddBindingRule<NatsTriggerAttribute>();
            triggerRule.BindToTrigger(new NatsTriggerBindingProvider(this));

            // Then add binding
            var bindingRule = context.AddBindingRule<NatsAttribute>();
            bindingRule.BindToCollector<string>(typeof(NatsBindingConverter<>), this);
        }

        /// <summary>
        /// Create Trigger context from a new NatsClient and the attribute
        /// supplied
        /// </summary>
        /// <param name="attribute">NatsTriggerAttribute instance</param>
        /// <returns>NatsTriggerContext instance</returns>
        public NatsTriggerContext CreateContext(NatsTriggerAttribute attribute)
        {
            return new NatsTriggerContext(attribute, _serviceFactory.CreateNatsClient(attribute.GetConnectionString()));
        }

        /// <summary>
        /// Create Binding Context from a new or cached NatsClient and attribute
        /// supplied
        /// </summary>
        /// <param name="attribute">Nats Attribute</param>
        /// <returns>Retruns NatsBindingContext instance. The </returns>
        public NatsBindingContext CreateContext(NatsAttribute attribute)
        {
            // The NatsClient will be cached for performance optimization
            return new NatsBindingContext(attribute,
                ClientCache.GetOrAdd(attribute.GetConnectionString(), (c) => _serviceFactory.CreateNatsClient(attribute.GetConnectionString()))
            );
        }
    }
}
