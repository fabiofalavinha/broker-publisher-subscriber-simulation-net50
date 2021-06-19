namespace Broker_Publisher_Subscriber_Simulation
{
    public interface IConsumer
    {
        string Name { get; }
        Topic Topic { get; }
        bool CanDeliverMessage(Message message);
        void DeliverMessage(Message message);
    }
}