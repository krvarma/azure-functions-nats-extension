# Custom Azure Function Extension - Part 2 - Bindings
![Custom Binding](https://raw.githubusercontent.com/krvarma/azure-functions-nats-extension/master/images/azfn-nats-binding.png)
This article is part two of the two-part series. In my previous article, we looked into how to create a custom trigger. In this article, we will look into how to create a custom binding. 

Since this a continuation of my previous article, I highly recommend reading part one before this one so that you get a basic idea of fundamentals of the custom extension and what are going to build.

For the sake of clarity, I will summarize what we are going to explore. In part one, we understand the basics of custom bindings and how to create a custom trigger. We developed a custom NATS Trigger and a sample application to test the trigger in part one.

In this article, we will look into how to create a custom NATS binding.

# Custom binding
To create a custom binding:

1.  Define a class that extends from Attribute.
2.  Create a class that extends the IAsyncCollector interface. This interface defines methods to AddAsync and FlushAsync. The system will call the AddAsync function to send data to external resources.
3.  Create a class that implements the IConverter interface. This interface has one method:
	-	Convert:- The system calls this method to create the AsyncCollector class.

4.  Create a class that implements the interface IExtensionConfigProvider. Similar to Triggers, the system will call the Initialize method. In this method, we bind the attribute class using the AddBindingRule method and bind to the Collector using the AddToCollector method.

Similar to Triggers, when the system starts, it searches for a class that implements IWebJobStartup. When it found a class that implements the interface:

1.  The system calls the Configure method passing the IWebJobsBuilder object. We add the extension using the AddExtension method using the class that implements the IExtensionConfigProvider interface.
2.  The system calls the Initialise method of the IExtensionConfigProvider passing ExtensionConfigContext as a parameter. Our implementation of the Initialize method adds the add the binding rule using the AddBindingRule method of the ExtensionConfigContext, which returns a BindingRule object. We call the BindToCollector to add our binding, passing the Converter as a parameter.
3.  The system calls the Convert method of the ICoverter. Our implementation creates an instance of AsyncCollector class and return.
4.  The systems call AddAsync function to send data to external services.

# Creating Custom binding

As stated before, we are creating a NATS extension here. We will use Visual Studio for development. We will also use the NATS client library [MyNatsClient](https://github.com/danielwertheim/mynatsclient). MyNatsClient is a handy library for .NET to connect to NATS.

Custom Extension is a Standard .NET Library. You need to add the following packages using NuGet. Open the NuGet manager and search for:

    Microsoft.Azure.WebJobs.Extensions
    MyNatsClient

Creating binding is relatively simple. As mentioned in the previous section, we need to create attribute class, converter class, and async collector class.

Here is our attribute class:
using System;
using Microsoft.Azure.WebJobs.Description;

    namespace WebJobs.Extension.Nats
    {
        /// <summary>
        /// <c>Attribute</c> class for Trigger
        /// </summary>
        [AttributeUsage(AttributeTargets.Parameter)]
        [Binding]
        public class NatsAttribute: Attribute
        {
            // <summary>
            // Connection string in the form of nats://<username>:<password>@<host>:<port>
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

Just like in the trigger, we have connection string and channel member variables. Next is our converter class. In this class, we create our async collector instance. Here is our async collector class. Just like in the trigger, we will generate context and pass it to the Collector. The 
Collector will use this instance to send a message to the NATS server. Here is our collector class.

    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Azure.WebJobs;
    
    namespace WebJobs.Extension.Nats.Bindings
    {
        /// <summary>
        /// Async Collector class. Responsible for publishing to a NATS channel
        /// </summary>
        /// <typeparam name="T">Data Type of value</typeparam>
        public class NatsAsyncCollector<T>: IAsyncCollector<T>
        {
            /// <summary>
            /// NatsBindingContext instance
            /// </summary>
            private readonly NatsBindingContext _context;
    
            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="context">NatsBindingContext instance</param>
            public NatsAsyncCollector(NatsBindingContext context)
            {
                _context = context;
            }
    
            /// <summary>
            /// Publish message to a NATS chanel
            /// </summary>
            /// <param name="message">Message to be published</param>
            /// <param name="cancellationToken">A Cancellation Token</param>
            /// <returns>A Task that completes when the message us published</returns>
            public Task AddAsync(T message, CancellationToken cancellationToken = default)
            {
                return _context.client.Publish(_context.attribute.Channel, message.ToString());
            }
    
            /// <summary>
            /// Flush any pending publish
            /// </summary>
            /// <param name="cancellationToken">A Cancellation token/param>
            /// <returns></returns>
            public Task FlushAsync(CancellationToken cancellationToken = default)
            {
                return Task.CompletedTask;
            }
        }
    }
## Create a sample to test the NATS Binding

Let's create a sample function to test our binding. Our sample function looks like this:

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
An HTTP call triggers the sample function. We retrieve the message query parameter and send this text to the predefined Channel.

You can use the Postman or other similar tools to test this sample function. I have created a sample node.js application to receive messages from NATS; here is the code.

    #!/usr/bin/env node
    /* jslint node: true */
    'use strict';
    
    var nats = require('nats').connect("nats://<username>:<password>@localhost:4222");
    
    nats.on('error', function(e) {
        console.log('Error [' + nats.options.url + ']: ' + e);
        process.exit();
    });
    
    nats.on('close', function() {
        console.log('CLOSED');
        process.exit();
    });
    
    var subject = "SampleChannelOut1"
    
    if (!subject) {
        console.log('Usage: node-sub <subject>');
        process.exit();
    }
    
    console.log('Listening on [' + subject + ']');
    
    nats.subscribe(subject, function(msg) {
        console.log('Received "' + msg + '"');
    });

If everything goes well, you can see the message on the console.

![NATS Binding](https://raw.githubusercontent.com/krvarma/azure-functions-nats-extension/master/images/natsbinding.gif)

I hope you enjoy this article and got a preliminary knowledge of how to create custom extensions for Azure Functions.