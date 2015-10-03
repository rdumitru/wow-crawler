using Microsoft.Azure;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WoW.Crawler.Service.Messaging
{
    public interface IQueueClientWrapper<T>
    {
        Task SendMessageAsync(Guid messageId, string body);

        void ReceiveMessage(Action<Guid, T> action, TimeSpan autoRenewTimeout,
            int maxConcurrentCalls = 1, bool autoComplete = true);

        void ReceiveMessageAsync(Func<Guid, T, Task> func, TimeSpan autoRenewTimeout,
            int maxConcurrentCalls = 1, bool autoComplete = true);

        void CloseConnection();
    }

    public class QueueClientWrapper<T> : IQueueClientWrapper<T>
    {
        private readonly string _queueName;
        private readonly QueueClient _client;
        private readonly ManualResetEvent _completedEvent = new ManualResetEvent(false);

        public QueueClientWrapper(string queueName)
        {
            this._queueName = queueName;

            // Worker configuration.
            ServicePointManager.DefaultConnectionLimit = 12;

            var connectionString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString");
            var namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);
            if (!namespaceManager.QueueExists(this._queueName))
            {
                namespaceManager.CreateQueue(this._queueName);
            }

            this._client = QueueClient.CreateFromConnectionString(connectionString, this._queueName);
        }

        public async Task SendMessageAsync(Guid messageId, string body)
        {
            await this._client.SendAsync(new BrokeredMessage(body) { MessageId = messageId.ToString() });
        }

        public void ReceiveMessage(Action<Guid, T> action, TimeSpan autoRenewTimeout,
            int maxConcurrentCalls = 1, bool autoComplete = true)
        {
            try
            {
                // Initiates the message pump and callback is invoked for each message that is received, calling close on the client will stop the pump.
                this._client.OnMessage(msg =>
                {
                    var jobId = Guid.Parse(msg.MessageId);
                    var messageBody = msg.GetBody<string>();
                    var request = JsonConvert.DeserializeObject<T>(messageBody);

                    action(jobId, request);
                },
                new OnMessageOptions
                {
                    AutoComplete = autoComplete,
                    AutoRenewTimeout = autoRenewTimeout,
                    MaxConcurrentCalls = maxConcurrentCalls
                });

                this._completedEvent.WaitOne();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
        }

        public void ReceiveMessageAsync(Func<Guid, T, Task> func, TimeSpan autoRenewTimeout,
            int maxConcurrentCalls = 1, bool autoComplete = true)
        {
            try
            {
                // Initiates the message pump and callback is invoked for each message that is received, calling close on the client will stop the pump.
                this._client.OnMessageAsync(async msg =>
                {
                    var jobId = Guid.Parse(msg.MessageId);
                    var messageBody = msg.GetBody<string>();
                    var request = JsonConvert.DeserializeObject<T>(messageBody);

                    await func(jobId, request);
                },
                new OnMessageOptions
                {
                    AutoComplete = autoComplete,
                    AutoRenewTimeout = autoRenewTimeout,
                    MaxConcurrentCalls = maxConcurrentCalls
                });

                this._completedEvent.WaitOne();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
        }

        public void CloseConnection()
        {
            this._client.Close();
            this._completedEvent.Set();
        }
    }
}