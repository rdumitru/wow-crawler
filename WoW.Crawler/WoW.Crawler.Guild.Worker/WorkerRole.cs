using Autofac;
using Microsoft.WindowsAzure.ServiceRuntime;
using System;
using System.Diagnostics;
using WoW.Crawler.Model.Message;
using WoW.Crawler.Service;
using WoW.Crawler.Service.Messaging;

namespace WoW.Crawler.Guild.Worker
{
    public class WorkerRole : RoleEntryPoint
    {
        private IQueueClientWrapper<ProcessRealmGuildsRequest> _guildsQueueClient;

        public override bool OnStart()
        {
            // Autofac setup.
            var builder = new ContainerBuilder();
            IoC.RegisterServices(builder);
            var container = builder.Build();

            // Assign injections.
            this._guildsQueueClient = container.Resolve<IQueueClientWrapper<ProcessRealmGuildsRequest>>();

            Trace.TraceInformation("WoW.Crawler.Guild.Worker has been started");
            return base.OnStart();
        }

        public override void Run()
        {
            Trace.WriteLine("WoW.Crawler.Guild.Worker is running");
            this._guildsQueueClient.ReceiveMessage((jobId, request) =>
            {
                Trace.WriteLine(String.Format("WoW.Crawler.Guild.Worker consumed message with Id = {0}", jobId));
                // TODO: do work here.
            });
        }

        public override void OnStop()
        {
            Trace.TraceInformation("WoW.Crawler.Guild.Worker is stopping");
            this._guildsQueueClient.CloseConnection();
            base.OnStop();
        }
    }
}