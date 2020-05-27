using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.DependencyInjection;

namespace WebJobs.Extension.Nats
{
    /// <summary>
    /// WebJobBuilder extension to add NATS extensions
    /// </summary>
    public static class NatsWebJobsBuilderExtensions
    {
        /// <summary>
        /// Extension method to add our custom extensions
        /// </summary>
        /// <param name="builder"><c>IWebJobsBuilder</c> instance</param>
        /// <returns><c>IWebJobsBuilder</c> instance</returns>
        /// <exception>Throws ArgumentNullException if builder is null</exception>
        public static IWebJobsBuilder AddNatsExtension(this IWebJobsBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }


            builder.AddExtension<NatsExtensionConfigProvider>();

            builder.Services.AddSingleton<INatsServiceFactory, NatsServiceFactory>();

            return builder;
        }
    }
}
