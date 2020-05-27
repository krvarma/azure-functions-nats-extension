using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using WebJobs.Extension.Nats;

[assembly: WebJobsStartup(typeof(NatsBinding.Startup))]
namespace WebJobs.Extension.Nats
{
    /// <summary>
    /// Starup object
    /// </summary>
    public class NatsBinding
    {
        /// <summary>
        /// IWebJobsStartup startup class
        /// </summary>
        public class Startup : IWebJobsStartup
        {
            public void Configure(IWebJobsBuilder builder)
            {
                // Add NATS extensions
                builder.AddNatsExtension();
            }
        }
    }
}
