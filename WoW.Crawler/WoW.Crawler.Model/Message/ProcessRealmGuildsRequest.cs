using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoW.Crawler.Model.DTO;

namespace WoW.Crawler.Model.Message
{
    public class ProcessRealmGuildsRequest
    {
        public RealmDto Realm { get; set; }

        public IEnumerable<GuildSimpleDto> Guilds { get; set; }
    }
}