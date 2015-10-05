using Autofac;
using Microsoft.WindowsAzure.ServiceRuntime;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using WoW.Crawler.Model.DTO;
using WoW.Crawler.Model.Message;
using WoW.Crawler.Service;
using WoW.Crawler.Service.Messaging;
using WoW.Crawler.Service.Service.Contract;

namespace WoW.Crawler.Realm.Worker
{
    public class WorkerRole : RoleEntryPoint
    {
        private IQueueClientWrapper<RealmDto> _realmQueueClient;
        private IQueueClientWrapper<ProcessRealmGuildsRequest> _guildsQueueClient;
        private IGuildService _guildService;

        public override bool OnStart()
        {
            // Autofac setup.
            var builder = new ContainerBuilder();
            IoC.RegisterServices(builder);
            var container = builder.Build();

            // Assign injections.
            this._realmQueueClient = container.Resolve<IQueueClientWrapper<RealmDto>>();
            this._guildsQueueClient = container.Resolve<IQueueClientWrapper<ProcessRealmGuildsRequest>>();
            this._guildService = container.Resolve<IGuildService>();

            Trace.TraceInformation("WoW.Crawler.Realm.Worker has been started");
            return base.OnStart();
        }

        public override void Run()
        {
            Trace.WriteLine("WoW.Crawler.Realm.Worker is running");
            this._realmQueueClient.ReceiveMessageAsync(async (jobId, request) =>
            {
                Trace.WriteLine(String.Format("Consumed realm {0} ({1})", request.Name, request.Region.ToString()));

                // Send processed data to next worker.
                try
                {
                    var guilds = await this._guildService.GetGuildsForRealmAsync(request.Name, request.Region);
                    var realmGuilds = new ProcessRealmGuildsRequest { Realm = request, Guilds = guilds };
                    Trace.WriteLine(String.Format("Fetched {0} guilds from realm {1} ({2})", guilds.Count(), request.Name, request.Region.ToString()));

                    // Put the message on the queue.
                    var id = Guid.NewGuid();
                    var body = JsonConvert.SerializeObject(realmGuilds, Formatting.None);
                    await this._guildsQueueClient.SendMessageAsync(id, body);
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(String.Format("Failed to retrieve guilds on realm {0} ({1})",
                            request.Name, request.Region.ToString()));

                    var error = new[] { "Unhandled Exception: ", ex.Message, ex.StackTrace };
                    Trace.WriteLine(String.Join(Environment.NewLine, error));
                }
            });
        }

        public override void OnStop()
        {
            Trace.TraceInformation("WoW.Crawler.Realm.Worker is stopping");
            this._realmQueueClient.CloseConnection();
            this._guildsQueueClient.CloseConnection();
            base.OnStop();
        }
    }
}