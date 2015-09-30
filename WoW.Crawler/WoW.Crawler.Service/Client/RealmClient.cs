using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoW.Crawler.Model.DTO;
using WoW.Crawler.Model.Enum;
using WoW.Crawler.Service.Client.Contract;

namespace WoW.Crawler.Service.Client
{
    public class RealmClient : BattleNetClientBase, IRealmClient
    {
        #region Constructors

        public RealmClient()
        { }

        #endregion Constructors

        #region Public Members

        public async Task<RealmListDto> GetRealmListAsync(Region region)
        {
            // Build relative URL.
            var relativeUrl = this.BuildRelativeUrlWithQueryStr("wow/realm/status");

            // Make GET request.
            var response = await this.GetClient(region).GetAsync(relativeUrl);
            response.EnsureSuccessStatusCode();

            // Add the appropriate region to each realm.
            var responseContent = await this.DeserializeContentAsync<RealmListDto>(response.Content);
            foreach (var realm in responseContent.Realms)
            {
                realm.Region = region;
            }

            return responseContent;
        }

        #endregion Public Members
    }
}