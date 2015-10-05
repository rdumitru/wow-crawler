using Autofac;
using Microsoft.WindowsAzure.ServiceRuntime;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using WoW.Crawler.Model.Message;
using WoW.Crawler.Service;
using WoW.Crawler.Service.Messaging;
using WoW.Crawler.Service.Service.Contract;

namespace WoW.Crawler.Guild.Worker
{
    public class WorkerRole : RoleEntryPoint
    {
        private IQueueClientWrapper<ProcessRealmGuildsRequest> _guildsQueueClient;
        private IGuildService _guildService;

        public override bool OnStart()
        {
            // Autofac setup.
            var builder = new ContainerBuilder();
            IoC.RegisterServices(builder);
            var container = builder.Build();

            // Assign injections.
            this._guildsQueueClient = container.Resolve<IQueueClientWrapper<ProcessRealmGuildsRequest>>();
            this._guildService = container.Resolve<IGuildService>();

            Trace.TraceInformation("WoW.Crawler.Guild.Worker has been started");
            return base.OnStart();
        }

        public override void Run()
        {
            Trace.WriteLine("WoW.Crawler.Guild.Worker is running");
            this._guildsQueueClient.ReceiveMessageAsync(async (jobId, request) =>
            {
                Trace.WriteLine(String.Format("Consumed realm {0} ({1}) with {2} guilds", request.Realm.Name, request.Realm.Region.ToString(), request.Guilds.Count()));

                foreach (var guild in request.Guilds)
                {
                    try
                    {
                        var characters = await this._guildService.GetGuildDetailedCharactersAsync(guild.Name, guild.RealmName, guild.Region);
                        Trace.WriteLine(String.Format("Fetched {0} / {1} characters from guild {2} on realm {3} ({4})", characters.Count(), guild.MemberCount, guild.Name, guild.RealmName, guild.Region.ToString()));

                        // TODO: do something with the characters.
                        //foreach (var character in characters)
                        //{
                        //    Trace.WriteLine(String.Format("{0} - {1} - {2}",
                        //        character.Name, character.Class, character.RealmName));
                        //}
                    }
                    catch (Exception ex)
                    {
                        // Should happen for inexistent guilds.
                        Trace.WriteLine(String.Format("Failed to retrieve guild characters for guild {0} on realm {1} ({2})",
                            guild.Name, guild.RealmName, guild.Region.ToString()));

                        var error = new[] { "Unhandled Exception: ", ex.Message, ex.StackTrace };
                        Trace.WriteLine(String.Join(Environment.NewLine, error));
                    }
                }
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