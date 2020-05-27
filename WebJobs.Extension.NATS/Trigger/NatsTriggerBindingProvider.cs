using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Triggers;

namespace WebJobs.Extension.Nats
{
    /// <summary>
    /// Binding provider
    /// </summary>
    public class NatsTriggerBindingProvider: ITriggerBindingProvider
    {
        /// <summary>
        /// NatsExtensionConfigProvider instance variable. Used to create the
        /// context
        /// </summary>
        private NatsExtensionConfigProvider _provider;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="provider"><c>NatsExtensionConfigProvider</c> instance</param>
        public NatsTriggerBindingProvider(NatsExtensionConfigProvider provider)
        {
            _provider = provider;
        }

        /// <summary>
        /// Create the trigger binding
        /// </summary>
        /// <param name="context"><c>TriggerBindingProviderContext</c> context</param>
        /// <returns>A Task that has the trigger binding</returns>
        public Task<ITriggerBinding> TryCreateAsync(TriggerBindingProviderContext context)
        {
            var parameter = context.Parameter;
            var attribute = parameter.GetCustomAttribute<NatsTriggerAttribute>(false);

            if (attribute == null) return Task.FromResult<ITriggerBinding>(null);
            if (parameter.ParameterType != typeof(string)) throw new InvalidOperationException("Invalid parameter type");

            var triggerBinding = new NatsTriggerBinding(_provider.CreateContext(attribute));

            return Task.FromResult<ITriggerBinding>(triggerBinding);
        }
    }
}   
