namespace RabbitMQ_FactoryDI.MQFactory
{
    public class IQueueConfiguration
    {
        string Identifier { get; set; } = "default";
        bool PublishOnly { get; set; }
        bool ReceiveOnly { get; set; }
    }
}