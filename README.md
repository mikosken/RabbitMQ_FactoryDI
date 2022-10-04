# Dependency Injection of RabbitMQ Client

This is an example implementation of a RabbitMQ Client using Dependency
Injection and Factory Pattern in .Net/C#.

By using Dependency Injection together with Factory Pattern it is easy to configure
multiple active queues/connections both for sending and retrieving messages.

This project came about as [RabbitMQ.Client.Core.DependencyInjection](https://github.com/AntonyVorontsov/RabbitMQ.Client.Core.DependencyInjection)
was recently archived and not under active development.
The point of this project is not to be a complete package with all bells and
whistles, as that would require a concerted maintenance effort. But rather to
be a very small, simple to use, and simple to extend starting point.

## License

All applicable code is released under the MIT License, see file `LICENSE`.
Feel free to freely use, spread, modify, or extend any or all of this code as
you see fit.

## Getting started

If you wish to quickly use this code in your own project:

-   Install the nuget package for `RabbitMQ.Client`,
-   Copy the folder and contents of `/MQFactory` into your project,
-   Add `builder.Services.AddMessageQueues();` to your program.cs/startup.cs along with `using RabbitMQ_FactoryDI.MQFactory`,
-   See appsettings.json and QueueConfiguration.cs for required configuration.

To publish a string, or object as JSON, to your configured RabbitMQ queue:

```
    // To your service or controller constructor, add 'IMessageQueueFactory' to
    // get the injected message queue factory service.

    // To send message to RabbitMQ.
    var q = _messageQueues.GetQueue("MySendQueue");
    q.Publish(message);

    // To retrieve message from RabbitMQ.
    var q = _messageQueues.GetQueue("MyReceiveQueue");
    var message = q.Get();

    // You can also register a message handler for incoming messages using
    var q = _messageQueues.GetQueue("MyReceiveQueue");
    q.RegisterConsumer(...);
```

See `MessageApiController.cs` for a simple example, and check the fairly well
commented source files `MessageQueueFactory.cs` and `MessageQueue.cs` for
further documentation.

All relevant files required to implement this in other projects are stored under
`/MQFactory`, plus an example configuration in appsettings.json under the
section "MessageQueues".

### Example controller

![MessageApiController](https://raw.githubusercontent.com/mikosken/RabbitMQ_FactoryDI/master/Swagger_UI.png)

## Build and continue development of this project

To compile or continue development try this:

Install **git**, **dotnet SDK** and **Visual Studio Code**, then open a git console:

```
cd .\suitable\project\folder
git clone <address_to_this_repo>
cd ".\RabbitMQ_FactoryDI"
code .
```

To build and run enter in console:

```
dotnet restore
dotnet build
dotnet watch run
```

Don't forget that you need a running instance of RabbitMQ to connect to.
If you have docker or Docker for Desktop installed, you can start a docker
container with RabbitMQ using the terminal command:

```
docker run -d --hostname my-rabbit --name some-rabbit -p 5672:5672 -p 8080:15672 -e RABBITMQ_DEFAULT_USER=user -e RABBITMQ_DEFAULT_PASS=password rabbitmq:3-management
```

This starts RabbitMQ with a management interface available in your browser on
`localhost:8080`. And the example configuration in `appsettings.json` should
be able to connect.

## References

Inspiration, code snippets, etc.

-   [RabbitMQ.Client.Core.DependencyInjection](https://github.com/AntonyVorontsov/RabbitMQ.Client.Core.DependencyInjection)
-   [Official RabbitMQ - Get Started](https://www.rabbitmq.com/getstarted.html)
-   [RabbitMQ Consumer Received Event with Docker in .NET](https://medium.com/swlh/rabbitmq-consumer-received-event-with-docker-in-net-cb2cde6a12f3)
