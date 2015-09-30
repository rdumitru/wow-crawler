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

        public async Task<GuildMemberListDto> GetMemberListAsync(string guild, string realm, Region region)
        {
            // Build relative URL.
            NameValueCollection nvc = new NameValueCollection();
            nvc.Add("fields", "members");

            var relativeUrl = String.Format("wow/guild/{0}/{1}", Uri.EscapeDataString(realm), Uri.EscapeDataString(guild));

            // Make GET request.
            var response = await this.GetAsync(region, relativeUrl, nvc);

            // Deserialize content.
            var responseContent = await this.DeserializeContentAsync<GuildMemberListDto>(response.Content);

            // Set post-processing fields.
            foreach (var member in responseContent.Members)
            {
                member.Character.SetFaction();
                member.Character.Region = region;
            }
            responseContent.Region = region;

            return responseContent;
        }

        #endregion Public Members
    }
}