using System;
using Microsoft.Azure.WebJobs.Description;

namespace WebJobs.Extension.Nats
{
    /// <summary>
    /// <c>Attribute</c> class for Trigger
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    [Binding]
    public class NatsTriggerAttribute: Attribute
    {
        // <summary>
        // Connection string in the form of nats://krvarma:var753ma@localhost
        // </summary>
        public string Connection { get; set; }
        // Channel string
        public string Channel { get; set; }

        // <siummary>
        // Helper method to get connection string from environment variables
        // </summary>
        internal string GetConnectionString()
        {
            return Environment.GetEnvironmentVariable(Connection);
        }
    }
}
