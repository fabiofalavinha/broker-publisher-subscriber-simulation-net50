using System;
using System.Collections.Generic;

namespace Broker_Publisher_Subscriber_Simulation
{
    class Program
    {
        static void Main(string[] args)
        {
            var broker = new Broker();

            var topicSampleA = new Topic("SampleA");

            broker.RegisterTopic(topicSampleA);
            broker.SendMessage(new Message(topicSampleA.Name) { Content = "First Message to SampleA" });

            Console.ReadKey();

            var logConsumer = new LogConsumer(topicSampleA);

            broker.Subscribe(logConsumer);

            broker.SendMessage(new Message(topicSampleA.Name) { Content = "Second Message to SampleA" });

            Console.ReadKey();
        }

        private class LogConsumer : IConsumer
        {
            public string Name => $"LogConsumer-{DateTime.Now.Ticks}";
            public Topic Topic { get; private set; }

            public LogConsumer(Topic topic)
            {
                Topic = topic ?? throw new ArgumentNullException(nameof(topic));
            }

            public bool CanDeliverMessage(Message message)
            {
                return Topic.Name.Equals(message?.TopicName);
            }

            public void DeliverMessage(Message message)
            {
                Console.WriteLine($"Message received [{message}] from topic [{Topic.Name}]. Processing... OK, everything good!");
            }

            public override string ToString()
            {
                return Name;
            }

            public override bool Equals(object obj)
            {
                return obj is LogConsumer consumer &&
                       Name == consumer.Name &&
                       EqualityComparer<Topic>.Default.Equals(Topic, consumer.Topic);
            }

            public override int GetHashCode()
            {
                int hashCode = -2005826214;
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
                hashCode = hashCode * -1521134295 + EqualityComparer<Topic>.Default.GetHashCode(Topic);
                return hashCode;
            }
        }
    }
}
