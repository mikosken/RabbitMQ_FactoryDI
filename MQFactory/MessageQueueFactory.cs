namespace RabbitMQ_FactoryDI.MQFactory
{
    public class MessageQueueFactory : IMessageQueueFactory
    {
        private readonly IConfiguration _config;
        private readonly List<QueueConfiguration> _queueConfigs;
        private List<MessageQueue> Queues = new List<MessageQueue>();

        public MessageQueueFactory(IConfiguration config)
        {
            _config = config;
            _queueConfigs = _config.GetSection("MessageQueues").Get<List<QueueConfiguration>>();
        }

        public void Dispose()
        {
            foreach (var queue in Queues)
                queue.Dispose();
            Queues.Clear();
        }

        /// <summary>
        /// Attempts to instantiate all configured queues.
        /// If instantiation fails for a queue an exception is thrown.
        /// Useful for testing the configuration of all queues.
        /// </summary>
        public void InstantiateQueues()
        {
            foreach (var qc in _queueConfigs)
            {
                _ = GetQueue(qc.Identifier);
            }
        }

        /// <summary>
        /// Returns an instance of the named queue.
        /// If the queue is not already instantiated it is instantiated and stored for future use.
        /// 
        /// Throws an exception if there is no configuration for requested queue available.
        /// </summary>
        /// <param name="queueIdentifier"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public MessageQueue GetQueue(string queueIdentifier)
        {
            var q = Queues.FirstOrDefault(i => i.Identifier == queueIdentifier);
            if (q != null)
                return q;

            var c = _queueConfigs.FirstOrDefault(i => i.Identifier == queueIdentifier);
            if (c == null)
                throw new Exception($"No message queue configuration with identifier '{queueIdentifier}' found, unable to create queue.");

            q = new MessageQueue(c);
            Queues.Add(q);

            return q;
        }
    }
}