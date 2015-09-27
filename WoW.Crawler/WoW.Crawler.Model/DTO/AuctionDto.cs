using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoW.Crawler.Model.Enum;

namespace WoW.Crawler.Model.DTO
{
    public class AuctionDto
    {
        #region Blizzard Fields

        public string Owner { get; set; }

        [JsonProperty("OwnerRealm")]
        public string OwnerRealmName { get; set; }

        #endregion Blizzard Fields

        #region Post-Processing Fields

        public Region Region { get; set; }

        #endregion Post-Processing Fields
    }
}