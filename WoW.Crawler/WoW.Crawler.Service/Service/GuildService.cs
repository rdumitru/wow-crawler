using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WoW.Crawler.Model.DTO;
using WoW.Crawler.Model.Enum;
using WoW.Crawler.Service.Client.Contract;
using WoW.Crawler.Service.Service.Contract;

namespace WoW.Crawler.Service.Service
{
    public class GuildService : IGuildService
    {
        #region Private Fields

        private readonly IGuildClient _guildClient;
        private readonly IAuctionClient _auctionClient;
        private readonly ICharacterClient _characterClient;

        #endregion Private Fields

        #region Constructors

        public GuildService(IGuildClient guildClient, IAuctionClient auctionClient, ICharacterClient characterClient)
        {
            this._guildClient = guildClient;
            this._auctionClient = auctionClient;
            this._characterClient = characterClient;
        }

        #endregion Constructors

        #region Public Members

        public async Task<IEnumerable<GuildSimpleDto>> GetGuildsForRealmAsync(string realm, Region region)
        {
            // Get the list of all auctions on the server.
            var auctionList = await this._auctionClient.GetAuctionListAsync(realm, region);

            // Return null for tasks which throw an exception and throttle.
            // TODO: choose semaphore limit.
            SemaphoreSlim throttler = new SemaphoreSlim(64);
            Func<Task<CharacterDto>, Task<CharacterDto>> taskWrapper = async (task) =>
            {
                try
                {
                    var result = await task;
                    return result;
                }
                catch (HttpRequestException) { return null; }
                finally { throttler.Release(); }
            };

            // Create a task for each character request.
            var characterTasks = auctionList.Auctions.Select(auction => this._characterClient.GetCharacterAsync(auction.Owner, auction.OwnerRealmName, auction.Region, true, false));
            var wrappedCharacterTasks = characterTasks.Select(task =>
            {
                throttler.Wait();
                return taskWrapper(task);
            });

            var unfilteredGuilds = (await Task.WhenAll(wrappedCharacterTasks))
                .Where(character => character != null && character.Guild != null)
                .Select(character => character.Guild)
                .ToList();

            // Remove duplicate guilds.
            var guilds = new HashSet<GuildSimpleDto>(unfilteredGuilds);

            return guilds;
        }

        public Task<GuildMemberListDto> GetGuildMemberListAsync(string guild, string realm, Region region)
        {
            return this._guildClient.GetMemberListAsync(guild, realm, region);
        }

        public async Task<IEnumerable<CharacterDto>> GetGuildDetailedCharactersAsync(string guild, string realm, Region region)
        {
            // Get the simple list of members.
            var memberList = await this._guildClient.GetMemberListAsync(guild, realm, region);

            // Return null for tasks which throw an exception.
            Func<Task<CharacterDto>, Task<CharacterDto>> taskWrapper = async (task) =>
            {
                try { return await task; }
                catch (HttpRequestException) { return null; }
            };

            // Create a task for each character request.
            var characterTasks = memberList.Members.Select(member => this._characterClient.GetCharacterAsync(member.Character.Name, member.Character.RealmName, member.Character.Region, true, true));
            var wrappedCharacterTasks = characterTasks.Select(task => taskWrapper(task));

            var characters = (await Task.WhenAll(wrappedCharacterTasks))
                .Where(character => character != null)
                .ToList();

            return characters;
        }

        #endregion Public Members
    }
}