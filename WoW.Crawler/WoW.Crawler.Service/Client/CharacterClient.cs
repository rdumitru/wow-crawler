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

        public async Task<CharacterDto> GetCharacter(string character, string realm, Region region,
            bool includeGuild = false, bool includeTalents = false)
        {
            // Set values for the "fields" query string field.
            var fieldsValues = new List<string>();
            if (includeGuild) fieldsValues.Add("guild");
            if (includeTalents) fieldsValues.Add("talents");

            NameValueCollection nvc = new NameValueCollection();
            if (fieldsValues.Count > 0) nvc.Add("fields", String.Join(", ", fieldsValues));

            // Build relative URL.
            var relativeUrl = this.BuildRelativeUrlWithQueryStr(
                String.Format("wow/character/{0}/{1}", Uri.EscapeDataString(realm), Uri.EscapeDataString(character)), nvc);

            // Make GET request.
            var response = await this.GetClient(region).GetAsync(relativeUrl);
            response.EnsureSuccessStatusCode();

            // Deserialize content.
            var responseContent = await this.DeserializeContentAsync<CharacterDto>(response.Content);

            // Set post-processing fields.
            responseContent.SetFactionFromRace();
            responseContent.Region = region;
            if (responseContent.Guild != null)
            {
                responseContent.Guild.Region = region;
            }

            return responseContent;
        }

        #endregion Public Members
    }
}