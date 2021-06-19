using System;
using System.Collections.Generic;
using System.Linq;

namespace Broker_Publisher_Subscriber_Simulation
{
    public class Broker
    {
        private readonly HashSet<Topic> topics;
        private readonly HashSet<IConsumer> consumers;

        public Broker()
        {
            topics = new HashSet<Topic>();
            consumers = new HashSet<IConsumer>();
        }

        public void RegisterTopic(Topic topic)
        {
            if (topic == null)
            {
                throw new ArgumentNullException(nameof(topic));
            }
            Console.WriteLine($"Registering topic [{topic.Name}]...");
            topic.MessageArrived += TopicFound_MessageArrived;
            topics.Add(topic);
        }

        public void SendMessage(Message message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            var topicFound = topics.SingleOrDefault(t => t.Name.Equals(message.TopicName));
            if (topicFound == null)
            {
                topicFound = new Topic(message.TopicName);
                topicFound.MessageArrived += TopicFound_MessageArrived;
                topics.Add(topicFound);
            }

            Console.WriteLine($"Sending message to topic [{topicFound.Name}]");
            topicFound.Publish(message);
        }

        private void TopicFound_MessageArrived(object sender, MessageArrivedEventArgs e)
        {
            var message = e.Message;
            var subscribers = consumers.Where(c => c.CanDeliverMessage(message)).ToArray();
            if (subscribers.Any())
            {
                Console.WriteLine($"Delivering message to [{subscribers.Length}] consumers...");
                var subscriber = subscribers.First();
                subscriber.DeliverMessage(message);
            }
            else
            {
                var topic = (Topic)sender;
                Console.WriteLine($"Retain message on topic [{topic.Name}] because no subscriber was found to deliver the message");
                topic.RetainMessage(message);
            }
        }

        public void Subscribe(IConsumer consumer)
        {
            if (consumer == null)
            {
                throw new ArgumentNullException(nameof(consumer));
            }

            Console.WriteLine($"Registering subscriber [{consumer}]");
            consumers.Add(consumer);

            var topic = consumer.Topic;

            var topicFound = topics.SingleOrDefault(t => t.Equals(topic));
            if (topicFound == null)
            {
                Console.WriteLine($"Registering topic [{topic.Name}] for new subscriber [{consumer.Name}]...");
                topic.MessageArrived += TopicFound_MessageArrived;
                topics.Add(topic);
                topicFound = topic;
            }

            Console.WriteLine($"Acknowledge new subscriber to topic [{topicFound.Name}]");
            topicFound.AcknowledgeSubscriber();
        }

        public void RemoveSubcriber(IConsumer consumer)
        {
            consumers.Remove(consumer);
        }

        public void RemoveTopic(Topic topic)
        {
            var topicFound = topics.SingleOrDefault(t => t.Equals(topic));
            if (topicFound != null)
            {
                topicFound.MessageArrived -= TopicFound_MessageArrived;
                topics.Remove(topicFound);
            }
        }
    }
}