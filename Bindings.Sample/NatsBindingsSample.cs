using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using WebJobs.Extension.Nats;

namespace Bindings.Sample
{
    public static class NatsBindingsSample
    {
        [FunctionName("NatsBindingsSample")]
        public static void Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            [Nats(Connection = "NatsConnection", Channel = "SampleChannelOut")] out string message,
            ILogger log)
        {
            string msg = req.Query["message"];

            log.LogInformation("C# HTTP trigger function processed a request.");
            log.LogInformation($"Received message {msg}");

            message = msg;
        }
    }
}
