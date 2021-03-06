﻿using Autofac;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WoW.Crawler.Model.Enum;
using WoW.Crawler.Service;
using WoW.Crawler.Service.Client;
using WoW.Crawler.Service.Client.Contract;
using WoW.Crawler.Service.Service.Contract;

namespace WoW.Crawler.Console
{
    public class Program
    {
        public static IContainer container;

        public static void Main(string[] args)
        {
            // Autofac setup.
            var builder = new ContainerBuilder();
            IoC.RegisterServices(builder);
            container = builder.Build();

            Test().Wait();
        }

        public static async Task Test()
        {
            //var realmClient = container.Resolve<IRealmClient>();
            //var realmList = await realmClient.GetRealmList(Region.EU);

            //var charClient = container.Resolve<ICharacterClient>();
            //var character = await charClient.GetCharacterAsync("Dipi", "Korgath", Region.US, false, false);

            //var guildClient = container.Resolve<IGuildClient>();
            //var guild = await guildClient.GetMemberList("ii kagen ni shiro", "Korgath", Region.US);

            var guildService = container.Resolve<IGuildService>();
            var guilds = await guildService.GetGuildsForRealmAsync("Medivh", Region.EU);

            //var guildService = container.Resolve<IGuildService>();
            //var guildMembers = await guildService.GetGuildDetailedCharactersAsync("Reroll Horde", "Medivh", Region.EU);
            //var guildMembers2 = await guildService.GetGuildMemberListAsync("Reroll Horde", "Medivh", Region.EU);

            //var auctionClient = container.Resolve<IAuctionClient>();
            //var auctions = await auctionClient.GetAuctionListAsync("Medivh", Region.EU);
        }
    }
}