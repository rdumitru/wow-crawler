using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoW.Crawler.Model.Enum;

namespace WoW.Crawler.Model.DTO
{
    public class GuildSimpleDto
    {
        #region Blizzard Fields

        public string Name { get; set; }

        [JsonProperty("Realm")]
        public string RealmName { get; set; }

        [JsonProperty("Members")]
        public int MemberCount { get; set; }

        #endregion Blizzard Fields

        #region Post-Processing Fields

        public Region Region { get; set; }

        #endregion Post-Processing Fields

        #region Overridden methods.

        public override bool Equals(object obj)
        {
            GuildSimpleDto other = obj as GuildSimpleDto;
            if (obj == null || other == null) return false;

            return this.Name == other.Name
                && this.RealmName == other.RealmName
                && this.Region == other.Region;
        }

        public override int GetHashCode()
        {
            var result = String.Format("{0}{1}{2}", this.Name, this.RealmName, this.Region.ToString()).GetHashCode();
            return result;
        }

        #endregion Overridden methods.
    }
}