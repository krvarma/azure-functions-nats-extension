# Custom Azure Function Extension - Part 1 - Triggers

![NATS Extension](https://raw.githubusercontent.com/krvarma/azure-functions-nats-extension/master/images/azfn-nats-trigger.png)

In my [previous article](https://medium.com/swlh/rabbitmq-trigger-and-azure-functions-8826633bf54c), we have explored how to use Azure Function and RabbitMQ. RabbitMQ Extension is a built-in extension provided by Microsoft. Azure Function also supports writing custom extensions. We can use the WebJobs SDK to write custom extensions.

This article is part one of the two-part series on how to write a custom extension for Azure Function. In this article, we will look into how to create a custom trigger. In part two, we will look into how to create a custom binding. Combining two articles into one will make it very large and will difficult to read and understand.

In this series of articles, we will explore how to create [NATS](https://nats.io/)  custom extension. 

## NATS messaging system
NATS is a simple, secure, and highly scalable messaging system. It is a perfect fit for Microservice architecture, IoT applications, cloud-native applications, etc. 

NATS can run on a large cloud instance and also in low-end devices like edge devices or IoT devices. It is a [CNCF](https://www.cncf.io/) project and integrates with Kubernetes and other modern systems.

## Azure Function Custom Extension
We can use [Azure WebJob SDK](https://github.com/Azure/azure-webjobs-sdk) to develop Azure Functions custom extensions.

There are two types of extension, a Trigger, and Bindings. The trigger causes a function to run. In most of the cases, a trigger will have data associated with it. The Azure Function receives this trigger data as a parameter.

Binding is a method to connect other resources to the function. Input binding receives the data from the external source, and an output binding sends the data to the external source.

An Attribute class defines every Trigger and Binding. An Attribute class defines all the parameters and configuration values for the trigger or extension.

Attribute class is a crucial component in custom extension. When a consumer defines a trigger or binding, the system looks for a corresponding attribute class and initialize it. For example, if a consumer specifies a RabbitMQTrigger trigger, the system looks for RabbitMQTriggerAttribute class. When a consumer specifies a RabbitMQ binding, the system looks for RabbitMQAttribute. Similarly, for KafkaTrigger, the system looks for KafkaTriggerAttribute, for Kafka binding, the system looks for KafkaAttribute.

## Custom Trigger
To create a custom Trigger, we need to:

 -  Define a class that extends from Attribute. This class represents our attribute class. We define all the parameters and configuration values for our trigger. In our case, we define connection string and NATS channels.
 -  Define a class that implements the interface `IListener`. This class contains the logic to connect to our external event source and wait for events. In our case, we will connect to the NATS server and look for incoming messages. The IListener interface has the following functions:
	 - *StartAsync*:- The system calls this function to start our listener. This function returns one Task object that completes when our listener successfully started.
	 - *StopAsync*:- The system calls this function to stop our listener. This function returns one Task object that completes when the listener completely stopped.
	 - *Cancel*:- The system calls this function to cancel any ongoing listen operation.
	 - *Dispose*:- IDisposable's dispose function.

 -  Define a class that implements the interface `ITriggerBinding`. In this class, we create our listener and bind our trigger data. The ITriggerBinding interface has the following functions:
	-  *CreateListenerAsync*:- The system calls this function to create a listener. This function returns a Task object that has our listener.
	-  *BindAsync*:- This function is called to bind a specified value using a binding context. When our listener receives an event, we try to execute the function, passing the event data. This event data is encapsulated in a `TriggeredFunctionData` class and send to the Azure Function. In the `BindAsync`, we will bind this value to our corresponding data. This function returns a `TriggerData` class. `TriggerData` class accepts a class that implements an `IValueBinder` interface and a read-only dictionary. We will revisit this later in this article.
	-  *ToParameterDescriptor*:- The system calls this function to get a description of the binding.

 -  Define a class that implements the interface `IValueBinder`. As I explained in the `BindAsync` section, we are binding the trigger data to our data class using this class. The `IValueBinder` has three methods:

	 -  *GetValueAsync*:- Returns a task that has the value object.
	 -  *SetValueAsync*: - Returns a task that completes when the object to our data class completes.
	 -  *ToInvokeString*:- Returns object string.

 -  Define a class that implements the interface `ITriggerBindingProvider`. This class is a provider class that returns a class that implements the `ITriggerBinding` interface. This class has the following function:

	 -  *TryCreateAsync*:- The system call this function to get a class that implements the `ITriggerBinding` interface. The system will pass a `TriggerBindingProviderContext` class as a parameter. In this function, we check whether the `TriggerBindingProviderContext` object contains our custom attribute. If the `Attribute` is present, we will create TriggerBinding class and return a Task.

 -  Create a class that implements the interface `IExtensionConfigProvider`. The `IExtensionConfigProvider` defines an interface enabling third party extension to register. The interface has the following function:

	 -  *Initialize*:- In this function, we will register all our triggers and bindings.

 -  And finally, we create a class that implements the interface `IWebJobStartup`. This interface defines the configuration actions to perform when the Function Host starts up. This interface has the following function:

	 -  *Configure*:- The system call this function when the function host initializes. In this function, we will add our custom extension.

So basically what happens is when the system starts, it searches for a class that implements  `IWebJobStartup`. When it found a class that implements the interface: 

 - The system calls the Configure method passing the `IWebJobsBuilder` object. We add the extension using the `AddExtension` method using the class that implements the `IExtensionConfigProvider` interface.
- The system calls the Initialise method of the `IExtensionConfigProvider` passing `ExtensionConfigContext` as a parameter. Our implementation of the Initialize method adds the add the binding rule using the `AddBindingRule` method of the `ExtensionConfigContext`, which returns a `BindingRule` object. We call the `BindToTrigger` method to add our trigger passing `TriggerBindingProvider` as a parameter.
-  After that system calls the `TryCreateAsync` function of the `TriggerBindingProvider` passing the `TriggerBindingProviderContext` as a parameter, in this `TryCreateAsync` method, we check whether our `Attribute` class present or not. We create our class that implements the `ITriggerBinding` interface and return a Task that contains the object.
- The system then calls the `CreateListenerAsync` method of our class that implements the `ITriggerBinding` interface passing `ListenerFactoryContext` object. In our `CreateListenerAsync`, we return a class that implements the `IListener` interface. The `ListenerFactoryContext` object contains a class that implements the `ITriggeredFunctionExecutor` interface. The `ITriggeredFunctionExecutor` interface has a method called `TryExecuteAsync`. Using this method, we can execute the triggered function, passing the event data and `CancellationToken`.

## Creating NATS Custom Extension
As stated before, we are creating a NATS extension here. We will use Visual Studio for development. We will also use the NATS client library [MyNatsClient](https://github.com/danielwertheim/mynatsclient). MyNatsClient is a handy library for .NET to connect to NATS. 

Custom Extension is a Standard .NET Library. You need to add the following packages using NuGet. Open the NuGet manager and search for:

    Microsoft.Azure.WebJobs.Extensions
    MyNatsClient
## Create Trigger
As I mentioned before, we need to create an `Attribute` class first. Our attribute class NatsTriggerAttribute is as follows:

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
            // Connection string in the form of nats://<username>:<password>@localhost
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


The class has only two members, `Connection` and `Channel`. The Connection represents the NATS connection string, and the Channel represents the NATS channel to listen. There is a helper method, `GetEnvironmentVariable`, also defined in the class, which will retrieve the connection string from the environment variable and return.

Next, we need to create a `NatsListener` class. Here is our listener class:

    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Azure.WebJobs.Host.Executors;
    using Microsoft.Azure.WebJobs.Host.Listeners;
    using MyNatsClient.Rx;
    
    namespace WebJobs.Extension.Nats
    {
        /*
            The NatsListner class
            Implements the <c>IListener</c> interface. Contains the code to connect
            to a NATS server and subscribe a Channel.
         */
        public class NatsListener: IListener
        {
            private readonly ITriggeredFunctionExecutor _executor;
            private readonly NatsTriggerContext _context;
    
            /// <summary>
            /// NatsListener constructor
            /// </summary>
            ///
            /// <param name="executor"><c>ITriggeredFunctionExecutor</c> instance</param>
            /// <param name="context"><c>NatsTriggerContext</c> instance</param>
            ///
            public NatsListener(ITriggeredFunctionExecutor executor, NatsTriggerContext context)
            {
                _executor = executor;
                _context = context;
            }
    
            /// <summary>
            /// Cancel any pending operation
            /// </summary>
            public void Cancel()
            {
                if (_context == null || _context.Client == null) return;
    
                _context.Client.Disconnect();
            }
    
            /// <summary>
            ///  Dispose method
            /// </summary>
            public void Dispose()
            {
                _context.Client.Dispose();
            }
    
            /// <summary>
            /// Start the listener asynchronously. Subscribe to NATS channel and
            /// wait for message. When a message is received, execute the function
            /// </summary>
            /// <param name="cancellationToken">Cancellation token</param>
            /// <returns>A Task returned from Subscribe method</returns>
            public Task StartAsync(CancellationToken cancellationToken)
            {
                return _context.Client.Subscribe(_context.TriggerAttribute.Channel, stream => stream.Subscribe(msg => {
                    var triggerData = new TriggeredFunctionData
                    {
                        TriggerValue = msg.GetPayloadAsString()
                    };
    
                    var task = _executor.TryExecuteAsync(triggerData, CancellationToken.None);
                    task.Wait();
                }));
            }
    
            /// <summary>
            /// Stop current listening operation
            /// </summary>
            /// <param name="cancellationToken">Cancellation token</param>
            /// <returns></returns>
            public Task StopAsync(CancellationToken cancellationToken)
            {
                return Task.Run(() =>{
                    _context.Client.Disconnect();
                });
            }
        }
    }

As you can see, the Listener class receives two parameters,  `ITriggeredFunctionExecutor` and a `NatsTriggerContext` instance. 

We use the `ITriggeredFunctionExecutor` instance to execute the triggered function when we receive a message.

The  `NatsTriggerContext` object has two member variables. The `TriggerAttribute` variable is an object of our `Attribute` class. The `Client` variable is an object of the `NatsClient` class, which is a wrapper class around the `MyNatsClient` library. Here is context class:

    namespace WebJobs.Extension.Nats
    {
        /// <summary>
        /// Trigger context class
        /// </summary>
        public class NatsTriggerContext
        {
            /// <summary>
            /// <c>Attribute</c> instance
            /// </summary>
            public NatsTriggerAttribute TriggerAttribute;
            /// <summary>
            /// <c>NatsClient</c> instance to connect and subscribe to NATS
            /// </summary>
            public NatsClient Client;
    
            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="attribute">Attribute instnace</param>
            /// <param name="client">NatsClient instance</param>
            public NatsTriggerContext(NatsTriggerAttribute attribute, NatsClient client)
            {
                this.TriggerAttribute = attribute;
                this.Client = client;
            }
        }
    }

The `NatsListener` class uses the `NatsClient` object to subscribe to a NATS channel. When we receive a message, we invoke the function using the `ITriggeredFunctionExecutor` instance.

Next, we need to create `TriggerBinding` and `TriggerBindingProvider` class. There are relatively simple classes. The `TryCreateAsync` in the `NatsTriggerBindingProvider` class create `NatsTriggerBinding` instance and return. One thing to note here is, creating the context class. We create the `NatsTriggerContext` class instance by calling the `CreateContext` method of the `NatsExtensionConfigProvider` class. We will create the context class and pass it to the `NatsTriggerBinding` object.

Next, we create the `NatsExtensionConfigProvider` class. Inside the Initialize method, we create a rule using the `AddBindingRule` method and bind the binding provider to it. 

Another major thing to note here is that the `INatsServiceFactory` instance we are receiving in the constructor. We will revisit this in the next section. Just remember that the system will pass this as a parameter.

Next and finally, we need to create a startup class. Our startup class looks like this:

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

As I stated in the previous section, the `IWebJobsStartup` interface has only one method, `Configure`. The Configure method takes one parameter, an object of `IWebJobsBuilder` implementation. The system will pass this parameter to our `Configure` method.
 
You should have noticed the `AddNatsExtension` function. This function is an extension function of `IWebJobsBuilder` and is in the `NatsWebJobsBuilderExtensions` class. The `AddNatsExtension` is just a helper method. The `NatsWebJobsBuilderExtensions` class looks like this.

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

As you can see in this extension method, we are adding the extension using the `AddExtension` method of the `IWebJobsBuilder`. The `AddExtesion` method takes one parameter, our `IExtensionConfigProvier` instance. We are also adding a Singleton Service to the builder. The constructor of the `IExtensionConfigProvider` instance will receive this server as a parameter.

We can now build the library. If everything goes well, you can see the DLL file in the BIN folder.

## Create a sample to test the NATS Trigger
Now we need to create a sample function to test our trigger. Let' create a test Azure Function that uses our trigger. Our sample function looks like this:

    using Microsoft.Azure.WebJobs;
    using Microsoft.Extensions.Logging;
    using WebJobs.Extension.Nats;
    
    namespace NatsTrigger.Sample
    {
        public static class NatsTriggerSample
        {
            [FunctionName("NatsTriggerSample")]
            public static void Run(
                [NatsTrigger(Connection = "NatsConnection", Channel = "SampleChannel")] string message,
                ILogger log)
            {
                log.LogInformation($"Message Received From SampleChannel {message}");
            }
        }
    }

This function is straightforward, just log the message we are getting. The `Connection` string is from the `local.settings.json` file, and the `Channel` is hard-coded.

Before running our function, we need to run the NATS server. Run the server with the following command

    docker run -d --name nats-main -p 4222:4222 -p 6222:6222 -p 8222:8222 nats

Once it is running, let's start our function. I am using the following `node.js` application to send a message to a NATS channel. 

    #!/usr/bin/env node
    
    /* jslint node: true */
    'use strict';
    
    var nats = require('nats').connect("nats://<username>:<password>@localhost:4222");
    var args = process.argv.slice(2)
    
    nats.on('error', function(e) {
        console.log('Error [' + nats.options.url + ']: ' + e);
        process.exit();
    });
    
    var subject = args[0];
    var msg = args[1];
    
    if (!subject) {
        console.log('Usage: node-pub <subject> [msg]');
        process.exit();
    }
    
    nats.publish(subject, msg, function() {
        console.log('Published [' + subject + '] : "' + msg + '"');
        process.exit();
    });

Publish a message to the Channel using the following command:

    node.js publish.js SampleChannel "Aure Function and Nats are awesome."

If everything goes well, you can see the debug log from the function.

![enter image description here](https://raw.githubusercontent.com/krvarma/azure-functions-nats-extension/master/images/natstrigger.gif)

In the next part, we will look into how to create NATS bindings, till then Happy Coding!.
