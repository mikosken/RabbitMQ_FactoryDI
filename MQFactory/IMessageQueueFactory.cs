namespace RabbitMQ_FactoryDI.MQFactory
{
    public interface IMessageQueueFactory
    {
        MessageQueue GetQueue(string queueIdentifier);
        void InstantiateQueues();
    }
}