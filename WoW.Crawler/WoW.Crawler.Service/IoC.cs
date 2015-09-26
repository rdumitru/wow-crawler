using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoW.Crawler.Service.Client;
using WoW.Crawler.Service.Client.Contract;

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
        }
    }
}