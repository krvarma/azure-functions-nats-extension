using Microsoft.Azure.WebJobs;

namespace WebJobs.Extension.Nats.Bindings
{
    /// <summary>
    /// Nats Binding converter 
    /// </summary>
    /// <typeparam name="T">Paramterized data type, in our case string</typeparam>
    internal class NatsBindingConverter<T> : IConverter<NatsAttribute, IAsyncCollector<T>>
    {
        /// <summary>
        /// Extension Config Provider
        /// </summary>
        private NatsExtensionConfigProvider _provider;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="provider">Extension Config Provider instance</param>
        public NatsBindingConverter(NatsExtensionConfigProvider provider)
        {
            _provider = provider;
        }

        /// <summary>
        /// Convert, create the async collector class
        /// </summary>
        /// <param name="input">Nats attribute instance</param>
        /// <returns>Returns the async collector instance</returns>
        public IAsyncCollector<T> Convert(NatsAttribute input)
        {
            return new NatsAsyncCollector<T>(_provider.CreateContext(input));
        }
    }
}
