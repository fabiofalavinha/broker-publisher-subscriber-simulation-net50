using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Broker_Publisher_Subscriber_Simulation
{
    public class MessageArrivedEventArgs : EventArgs
    {
        public Message Message { get; private set; }

        public MessageArrivedEventArgs(Message message)
        {
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }
    }

    public class Topic
    {
        private readonly List<Message> messages;

        public string Name { get; private set; }

        public event EventHandler<MessageArrivedEventArgs> MessageArrived;

        public Topic(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }
            Name = name;
            messages = new List<Message>();
        }

        private void OnMessageArrived(Message message)
        {
            Task.Run(() => MessageArrived?.Invoke(this, new MessageArrivedEventArgs(message))); 
        }

        public void Publish(Message message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            OnMessageArrived(message);
        }

        internal void RetainMessage(Message message)
        {
            messages.Add(message);
        }

        internal void AcknowledgeSubscriber()
        {
            foreach (var message in messages.ToArray())
            {
                if (messages.Remove(message))
                {
                    OnMessageArrived(message);
                }
            }
        }

        public override bool Equals(object obj)
        {
            return obj is Topic topic &&
                   Name == topic.Name;
        }

        public override int GetHashCode()
        {
            return 539060726 + EqualityComparer<string>.Default.GetHashCode(Name);
        }
    }
}