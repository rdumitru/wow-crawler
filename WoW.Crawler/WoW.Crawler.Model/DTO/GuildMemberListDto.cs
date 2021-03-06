﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoW.Crawler.Model.Enum;

namespace WoW.Crawler.Model.DTO
{
    public class GuildMemberListDto
    {
        #region Blizzard Fields

        public DateTime LastModified { get; set; }

        public string Name { get; set; }

        [JsonProperty("Realm")]
        public string RealmName { get; set; }

        public Faction Side { get; set; }

        public IEnumerable<GuildMemberDto> Members { get; set; }

        #endregion Blizzard Fields

        #region Post-Processing Fields

        public Region Region { get; set; }

        #endregion Post-Processing Fields
    }
}