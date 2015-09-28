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

        private readonly IAuctionClient _auctionClient;
        private readonly ICharacterClient _characterClient;

        #endregion Private Fields

        #region Constructors

        public GuildService(IAuctionClient auctionClient, ICharacterClient characterClient)
        {
            this._auctionClient = auctionClient;
            this._characterClient = characterClient;
        }

        #endregion Constructors

        #region Public Members

        public async Task<IEnumerable<GuildSimpleDto>> GetGuildListForRealm(string realm, Region region)
        {
            // Keep only one instance of each guild.
            var guilds = new HashSet<GuildSimpleDto>();

            // Get the list of all auctions on the server.
            var auctionList = await this._auctionClient.GetAuctionList(realm, region);

            // Get the guild from each auction owner.
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
            }

            return guilds;
        }

        #endregion Public Members
    }
}