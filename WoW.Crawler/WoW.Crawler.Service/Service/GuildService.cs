using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
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

        public async Task<IEnumerable<GuildSimpleDto>> GetGuildsForRealm(string realm, Region region)
        {
            // Keep only one instance of each guild.
            var guilds = new HashSet<GuildSimpleDto>();

            // Get the list of all auctions on the server.
            var auctionList = await this._auctionClient.GetAuctionList(realm, region);

            // Get the guild from each auction owner.
            var idx = 0;
            var total = auctionList.Auctions.Count();

            // TODO: finish this.
            //var characterTasks = auctionList.Auctions.Select(auction => this._characterClient.GetCharacter(auction.Owner, auction.OwnerRealmName, auction.Region, true, false)).Take(5);
            //var characters = await Task.WhenAll(characterTasks);

            foreach (var auction in auctionList.Auctions)
            {
                try
                {
                    var character = await this._characterClient.GetCharacter(auction.Owner, auction.OwnerRealmName, auction.Region, true, false);
                    if (character != null && character.Guild != null)
                    {
                        guilds.Add(character.Guild);
                    }
                }
                catch (HttpRequestException)
                {
                    // This will happen for inactive players.
                }

                Console.WriteLine("{0} / {1}", idx, total);
                idx++;
            }

            return guilds;
        }

        public Task<GuildMemberListDto> GetGuildMemberList(string guild, string realm, Region region)
        {
            return this._guildClient.GetMemberList(guild, realm, region);
        }

        public async Task<IEnumerable<CharacterDto>> GetGuildDetailedCharacters(string guild, string realm, Region region)
        {
            // The list of all the characters in this guild.
            var characters = new List<CharacterDto>();

            // Get the simple list of members.
            var memberList = await this._guildClient.GetMemberList(guild, realm, region);

            foreach (var member in memberList.Members)
            {
                // WARN: This should never happen!
                if (member.Character.Region != region) throw new ApplicationException();

                try
                {
                    var character = await this._characterClient.GetCharacter(member.Character.Name, member.Character.RealmName, member.Character.Region, true, true);
                    if (character != null) characters.Add(character);
                }
                catch (HttpRequestException)
                {
                    // This will happen for inactive players.
                }
            }

            return characters;
        }

        #endregion Public Members
    }
}