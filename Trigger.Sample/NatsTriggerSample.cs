using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using WebJobs.Extension.Nats;

namespace NatsTrigger.Sample
{
    public static class NatsTriggerSample
    {
        [FunctionName("NatsTriggerSample")]
        public static void Run(
            [NatsTrigger(Connection = "NatsConnection", Channel = "SampleChannel", QueueGroup = "SampleGroup")] string message,
            ILogger log)
        {
            log.LogInformation($"Message Received From SampleChannel {message}");
        }
    }
}
    