using Autofac;
using Microsoft.WindowsAzure.ServiceRuntime;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using WoW.Crawler.Model.DTO;
using WoW.Crawler.Model.Message;
using WoW.Crawler.Service;
using WoW.Crawler.Service.Messaging;

namespace WoW.Crawler.Realm.Worker
{
    public class WorkerRole : RoleEntryPoint
    {
        private IQueueClientWrapper<RealmDto> _realmQueueClient;
        private IQueueClientWrapper<ProcessRealmGuildsRequest> _guildsQueueClient;

        public override bool OnStart()
        {
            // Autofac setup.
            var builder = new ContainerBuilder();
            IoC.RegisterServices(builder);
            var container = builder.Build();

            // Assign injections.
            this._realmQueueClient = container.Resolve<IQueueClientWrapper<RealmDto>>();
            this._guildsQueueClient = container.Resolve<IQueueClientWrapper<ProcessRealmGuildsRequest>>();

            Trace.TraceInformation("WoW.Crawler.Realm.Worker has been started");
            return base.OnStart();
        }

        public override void Run()
        {
            Trace.WriteLine("WoW.Crawler.Realm.Worker is running");
            this._realmQueueClient.ReceiveMessage(async (jobId, request) =>
            {
                Trace.WriteLine(String.Format("WoW.Crawler.Realm.Worker consumed message with Id = {0}", jobId));

                // Send processed data to next worker.
                var realmGuilds = new ProcessRealmGuildsRequest();
                realmGuilds.Realm = request;

                // Test data.
                realmGuilds.Guilds = new List<GuildSimpleDto>
                {
                    new GuildSimpleDto
                    {
                        Name = "Test Guild Name",
                        MemberCount = 1,
                        RealmName = request.Name,
                        Region = request.Region
                    }
                };

                var id = Guid.NewGuid();
                var body = JsonConvert.SerializeObject(realmGuilds, Formatting.None);
                await this._guildsQueueClient.SendMessageAsync(id, body);
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