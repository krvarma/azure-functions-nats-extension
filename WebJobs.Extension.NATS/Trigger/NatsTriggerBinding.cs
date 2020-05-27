using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Listeners;
using Microsoft.Azure.WebJobs.Host.Protocols;
using Microsoft.Azure.WebJobs.Host.Triggers;

namespace WebJobs.Extension.Nats
{
    /// <summary>
    /// Nats Binding class
    /// </summary>
    public class NatsTriggerBinding: ITriggerBinding
    {
        private readonly NatsTriggerContext _context;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">NatsTriggerContext instance</param>
        public NatsTriggerBinding(NatsTriggerContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Trigger value type string
        /// </summary>
        public Type TriggerValueType => typeof(string);
        /// <summary>
        /// BindingDataContract
        /// </summary>
        public IReadOnlyDictionary<string, Type> BindingDataContract => new Dictionary<string, Type>();

        /// <summary>
        /// Bind a value using the binding context
        /// </summary>
        /// <param name="value">Value to bind</param>
        /// <param name="context">Binding contract to use</param>
        /// <returns>Returns a Task that contains the <c>TriggerData</c></returns>
        public Task<ITriggerData> BindAsync(object value, ValueBindingContext context)
        {
            var valueProvider = new NatsValueBinder(value);
            var bindingData = new Dictionary<string, object>();
            var triggerData = new TriggerData(valueProvider, bindingData);

            return Task.FromResult<ITriggerData>(triggerData);
        }

        /// <summary>
        /// Create listener class
        /// </summary>
        /// <param name="context">Listener factory context</param>
        /// <returns>A Task that contains the listenr instance</returns>
        public Task<IListener> CreateListenerAsync(ListenerFactoryContext context)
        {
            var executor = context.Executor;
            var listener = new NatsListener(executor, _context);

            return Task.FromResult<IListener>(listener);
        }

        /// <summary>
        /// Get binding description
        /// </summary>
        /// <returns>Retruns a string that describes the binding</returns>
        public ParameterDescriptor ToParameterDescriptor()
        {
            return new TriggerParameterDescriptor
            {
                Name = "Nats",
                DisplayHints = new ParameterDisplayHints
                {
                    Prompt = "Nats",
                    Description = "Nats message trigger"
                }
            };
        }
    }
}
