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
using Timer = System.Timers.Timer;

namespace WoW.Crawler.Service.Messaging
{
    public interface IQueueClientWrapper<T>
    {
        Task SendMessageAsync(Guid messageId, string body);

        void ReceiveMessage(Action<Guid, T> action, int maxConcurrentCalls = 1);

        void ReceiveMessageAsync(Func<Guid, T, Task> func, int maxConcurrentCalls = 1);

        void CloseConnection();
    }

    public class QueueClientWrapper<T> : IQueueClientWrapper<T>
    {
        private readonly string _queueName;
        private readonly QueueClient _client;
        private readonly ManualResetEvent _completedEvent = new ManualResetEvent(false);

        // Keep-alive heartbeat interval.
        private static readonly double FifteenSeconds = TimeSpan.FromSeconds(15).TotalMilliseconds;

        // WARN: must be greater than the heartbeat interval.
        private static readonly TimeSpan AutoRenewTimeout = TimeSpan.FromMinutes(5);

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

        public void ReceiveMessage(Action<Guid, T> action, int maxConcurrentCalls = 1)
        {
            this._client.OnMessage(msg =>
            {
                var jobId = Guid.Parse(msg.MessageId);
                var messageBody = msg.GetBody<string>();
                var request = JsonConvert.DeserializeObject<T>(messageBody);

                using (var timer = new Timer(FifteenSeconds))
                {
                    try
                    {
                        // Renew lock periodically to keep message alive.
                        timer.Elapsed += (sender, args) =>
                        {
                            msg.RenewLock();
                        };

                        timer.Start();
                        action(jobId, request);
                        timer.Stop();
                    }
                    catch (Exception ex)
                    {
                        var error = new[] { "Unhandled Exception: ", ex.Message, ex.StackTrace };
                        Trace.WriteLine(String.Join(Environment.NewLine, error));
                    }
                    finally
                    {
                        timer.Stop();
                        msg.Complete();
                    }
                }
            },
            new OnMessageOptions
            {
                MaxConcurrentCalls = maxConcurrentCalls,
                AutoRenewTimeout = AutoRenewTimeout
            });

            this._completedEvent.WaitOne();
        }

        public void ReceiveMessageAsync(Func<Guid, T, Task> func, int maxConcurrentCalls = 1)
        {
            this._client.OnMessageAsync(async msg =>
            {
                var jobId = Guid.Parse(msg.MessageId);
                var messageBody = msg.GetBody<string>();
                var request = JsonConvert.DeserializeObject<T>(messageBody);

                using (var timer = new Timer(FifteenSeconds))
                {
                    try
                    {
                        // Renew lock periodically to keep message alive.
                        timer.Elapsed += (sender, args) =>
                        {
                            msg.RenewLock();
                        };

                        timer.Start();
                        await func(jobId, request);
                        timer.Stop();
                    }
                    catch (Exception ex)
                    {
                        var error = new[] { "Unhandled Exception: ", ex.Message, ex.StackTrace };
                        Trace.WriteLine(String.Join(Environment.NewLine, error));
                    }
                    finally
                    {
                        timer.Stop();
                        msg.Complete();
                    }
                }
            },
            new OnMessageOptions
            {
                MaxConcurrentCalls = maxConcurrentCalls,
                AutoRenewTimeout = AutoRenewTimeout
            });

            this._completedEvent.WaitOne();
        }

        public void CloseConnection()
        {
            this._client.Close();
            this._completedEvent.Set();
        }
    }
}