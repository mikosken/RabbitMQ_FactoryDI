using RabbitMQ.Client.Events;

namespace RabbitMQ_FactoryDI.MQFactory
{
    public interface IMessageQueue
    {
        string Identifier { get; }
        bool CanPublish { get; }
        bool CanReceive { get; }
        bool PublishOnly { get; }
        bool ReceiveOnly { get; }

        T? Get<T>();
        string? Get();
        void Publish<T>(T item);
        void Publish(string message);
        void RegisterConsumer(EventHandler<BasicDeliverEventArgs> OnNewMessageReceived, bool autoAck = true);
    }
}