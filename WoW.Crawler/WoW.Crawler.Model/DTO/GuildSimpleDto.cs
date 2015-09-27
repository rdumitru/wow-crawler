using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Crawler.Model.DTO
{
    public class GuildSimpleDto
    {
        public string Name { get; set; }

        [JsonProperty("Realm")]
        public string RealmName { get; set; }

        [JsonProperty("Members")]
        public int MemberCount { get; set; }
    }
}