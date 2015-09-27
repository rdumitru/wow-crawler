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
    public class CharacterClient : BattleNetClientBase, ICharacterClient
    {
        #region Constructors

        public CharacterClient()
        { }

        #endregion Constructors

        #region Public Members

        public async Task<CharacterDto> GetCharacter(string character, string realm, Region region)
        {
            // Build relative URL.
            NameValueCollection nvc = new NameValueCollection();
            nvc.Add("fields", "guild, talents");

            var relativeUrl = this.BuildRelativeUrlWithQueryStr(
                String.Format("wow/character/{0}/{1}", Uri.EscapeDataString(realm), Uri.EscapeDataString(character)), nvc);

            // Make GET request.
            var response = await this.GetClient(region).GetAsync(relativeUrl);
            response.EnsureSuccessStatusCode();

            var responseContent = await this.DeserializeContentAsync<CharacterDto>(response.Content);
            responseContent.SetFaction();

            return responseContent;
        }

        #endregion Public Members
    }
}