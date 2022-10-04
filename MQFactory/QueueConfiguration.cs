namespace RabbitMQ_FactoryDI.MQFactory
{
    public class QueueConfiguration : IQueueConfiguration
    {
        // For locally identifying this specific configuration/connection.
        // Has no effect on connection or server side configuration.
        // Must match value to MessageQueueFactory.GetQueue()
        public string Identifier { get; set; } = "QueueIdentifier";
        // Only allow publishing messages.
        public bool PublishOnly { get; set; } = false;
        // Only allow receiving messages.
        public bool ReceiveOnly { get; set; } = false;
        public string Hostname { get; set; } = "localhost";
        public int Port { get; set; } = 5672;
        // If the queue should be passively declared.
        // If true, Durable/Exclusive/AutoDelete has no effect.
        // If true, queue must already exist.
        public bool QueueDeclarePassive { get; set; } = false;
        // What the queue is named on the server.
        public string Queue { get; set; } = "TestQueue";
        public bool Durable { get; set; } = false;
        public bool Exclusive { get; set; } = false;
        public bool AutoDelete { get; set; } = false;
        // Must be set to publish messages.
        public string? Exchange { get; set; } = null;
        //Must be set to publish messages.
        public string? RoutingKey { get; set; } = null;
        public string VirtualHost { get; set; } = "/";
        public string Username { get; set; } = "guest";
        public string Password { get; set; } = "guest";
    }
}