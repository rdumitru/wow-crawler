using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoW.Crawler.Model.DTO;
using WoW.Crawler.Model.Message;
using WoW.Crawler.Service.Client;
using WoW.Crawler.Service.Client.Contract;
using WoW.Crawler.Service.Messaging;
using WoW.Crawler.Service.Service;
using WoW.Crawler.Service.Service.Contract;

namespace WoW.Crawler.Service
{
    public class IoC
    {
        public static void RegisterServices(ContainerBuilder builder)
        {
            //=================================================================
            // CLIENTS
            //=================================================================

            // IAuctionClient.
            builder
                .RegisterType<AuctionClient>()
                .As<IAuctionClient>()
                .InstancePerLifetimeScope();

            // ICharacterClient.
            builder
                .RegisterType<CharacterClient>()
                .As<ICharacterClient>()
                .InstancePerLifetimeScope();

            // IGuildClient.
            builder
                .RegisterType<GuildClient>()
                .As<IGuildClient>()
                .InstancePerLifetimeScope();

            // IRealmClient.
            builder
                .RegisterType<RealmClient>()
                .As<IRealmClient>()
                .InstancePerLifetimeScope();

            //=================================================================
            // SERVICES
            //=================================================================

            // IAuctionClient.
            builder
                .RegisterType<GuildService>()
                .As<IGuildService>()
                .InstancePerLifetimeScope();

            // IRealmService.
            builder
                .RegisterType<RealmService>()
                .As<IRealmService>()
                .InstancePerLifetimeScope();

            // IQueueClientWrapper<RealmDto>
            builder
                .RegisterType<QueueClientWrapper<RealmDto>>()
                .WithParameter("queueName", "WoW.Crawler.RealmQueue")
                .As<IQueueClientWrapper<RealmDto>>()
                .InstancePerDependency();

            // IQueueClientWrapper<ProcessRealmGuildsRequest>
            builder
                .RegisterType<QueueClientWrapper<ProcessRealmGuildsRequest>>()
                .WithParameter("queueName", "WoW.Crawler.GuildsQueue")
                .As<IQueueClientWrapper<ProcessRealmGuildsRequest>>()
                .InstancePerDependency();
        }
    }
}