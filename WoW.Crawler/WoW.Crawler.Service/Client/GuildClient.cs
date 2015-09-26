using RestSharp;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WoW.Crawler.Model.DTO;
using WoW.Crawler.Model.Enum;
using WoW.Crawler.Service.Client.Contract;

namespace WoW.Crawler.Service.Client
{
    public class GuildClient : BattleNetClientBase, IGuildClient
    {
        #region Constructors

        public GuildClient()
        { }

        #endregion Constructors

        #region Public Members

        public async Task<GuildMemberListDto> GetMemberList(string guild, string realm, Region region)
        {
            // Build relative URL.
            NameValueCollection nvc = new NameValueCollection();
            nvc.Add("fields", "members");

            var relativeUrl = this.BuildRelativeUrlWithQueryStr(
                String.Format("wow/guild/{0}/{1}", Uri.EscapeDataString(realm), Uri.EscapeDataString(guild)), nvc);

            // Make GET request.
            var response = await this.GetClient(region).GetAsync(relativeUrl);
            response.EnsureSuccessStatusCode();

            var responseContent = await this.DeserializeContentAsync<GuildMemberListDto>(response.Content);
            foreach (var member in responseContent.Members)
            {
                member.Character.SetFaction();
            }

            return responseContent;
        }

        #endregion Public Members
    }
}