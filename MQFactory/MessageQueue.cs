using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMQ_FactoryDI.MQFactory
{
    public class MessageQueue : IMessageQueue, IDisposable
    {
        public string Identifier { get => _configuration.Identifier; }
        public bool CanPublish { get =>
                (!ReceiveOnly) &&
                (_configuration.Exchange != null
                && _configuration.RoutingKey != null); }
        public bool CanReceive { get => !PublishOnly; }
        public bool PublishOnly { get => _configuration.PublishOnly; }
        public bool ReceiveOnly { get => _configuration.ReceiveOnly; }

        public readonly QueueConfiguration _configuration;
        private ConnectionFactory connectionFactory;
        private IConnection connection;
        private IModel channel;
        private EventingBasicConsumer? eventingConsumer;
        // We need to store the assigned handlers so they can be deregistered.
        private EventHandler<BasicDeliverEventArgs>? receivedHandler;
        // Might be a good idea to implement a shutdown handler in case remote end shuts down during use.
        //private EventHandler<ShutdownEventArgs>? shutdownHandler;

        public MessageQueue(QueueConfiguration configuration)
        {
            _configuration = configuration;

            connectionFactory = new ConnectionFactory() { 
                HostName = _configuration.Hostname,
                Port = _configuration.Port,
                VirtualHost = _configuration.VirtualHost,
                UserName = _configuration.Username,
                Password = _configuration.Password };
            
            connection = connectionFactory.CreateConnection();
            channel = connection.CreateModel();

            if (_configuration.QueueDeclarePassive)
            {
                // Passive declare,
                // Just check if queue exists and use it if it does.
                // Throws exception if queue doesn't exist.
                channel.QueueDeclarePassive(_configuration.Queue);

            } else
            {
                // Non-passive declare,
                // Create the queue if it doesn't exist.
                channel.QueueDeclare(queue: _configuration.Queue,
                                     durable: _configuration.Durable,
                                     exclusive: _configuration.Exclusive,
                                     autoDelete: _configuration.AutoDelete,
                                     arguments: null);
            }
        }

        /// <summary>
        /// Converts a message item to json and publishes it to the configured message queue.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        public void Publish<T>(T item)
        {
            string jsonString = JsonSerializer.Serialize(item);
            Publish(jsonString);
        }

        /// <summary>
        /// Publishes a string message to the configured message queue.
        /// </summary>
        /// <param name="message"></param>
        public void Publish(string message)
        {
            if (!CanPublish)
                throw new Exception($"Configuration '{_configuration.Identifier}'. Unable to publish. Have you specified 'ReceiveOnly = true', or are you missing Exchange and RoutingKey?");

            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: _configuration.Exchange,
                                 routingKey: _configuration.RoutingKey,
                                 basicProperties: null,
                                 body: body);
        }


        /// <summary>
        /// Retrieves a single message from the configured queue and deserializes it to specified type.
        /// If no message available returns default value for T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public T? Get<T>()
        {
            if (!CanReceive)
                throw new Exception($"Configuration '{_configuration.Identifier}'. Not configured for receiving messages. Have you set 'PublishOnly = true'?");

            var getResult = channel.BasicGet(_configuration.Queue, false);
            if (getResult == null)
                return default(T);

            var body = getResult.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            var item =  JsonSerializer.Deserialize<T>(message);

            // If we get this far with deserialization all is well,
            // acknowledge message received.
            channel.BasicAck(getResult.DeliveryTag, false);
            return item;
        }

        /// <summary>
        /// Retrieves a single message from the configured queue and returns it as string.
        /// If no message available returns null.
        /// </summary>
        public string? Get()
        {
            if (!CanReceive)
                throw new Exception($"Configuration '{_configuration.Identifier}'. Not configured for receiving messages. Have you set 'PublishOnly = true'?");

            var getResult = channel.BasicGet(_configuration.Queue, false);
            if (getResult == null)
                return null;

            var body = getResult.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            // If we get this far with deserialization all is well,
            // acknowledge message received.
            channel.BasicAck(getResult.DeliveryTag, false);
            return message;
        }

        /// <summary>
        /// Registers a single event handler for incoming messages.
        /// </summary>
        /// <param name="OnNewMessageReceived">An event handler for taking care of received messages.
        /// Should have a signature like:
        /// <c>private static void OnNewMessageReceived(object sender, BasicDeliverEventArgs e)</c>
        /// </param>
        /// <param name="autoAck"> Should received messages be auto acknowledged?
        /// If false you must handle acks manually in your callback.
        /// To manually acknowledge, get <c>Deliverytag</c> from event args and call <c>channel.BasicAck()</c>.
        /// </param>
        public void RegisterConsumer(EventHandler<BasicDeliverEventArgs> OnNewMessageReceived, bool autoAck = true)
        {
            if (eventingConsumer == null)
                eventingConsumer = new EventingBasicConsumer(channel);
            if (receivedHandler != null)
            {
                eventingConsumer.Received -= receivedHandler;
                receivedHandler = null;
            }

            // Add callback to consumer and store callback reference.
            receivedHandler = OnNewMessageReceived;
            eventingConsumer.Received += receivedHandler;

            // Start consuming using handler.
            channel.BasicConsume(
                    queue: _configuration.Queue,
                    autoAck: autoAck,
                    consumer: eventingConsumer);
        }

        public void Dispose()
        {
            // Unsubscribe event handlers on dispose/shutdown.
            if (eventingConsumer != null)
            {
                if (receivedHandler != null)
                    eventingConsumer.Received -= receivedHandler;
            }

            channel.Close();
            channel.Dispose();
            connection.Close();
            connection.Dispose();
        }
    }
}