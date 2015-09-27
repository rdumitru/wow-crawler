using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoW.Crawler.Model.DTO;
using WoW.Crawler.Model.Enum;

namespace WoW.Crawler.Service.Service.Contract
{
    public interface IGuildService
    {
        Task<IEnumerable<GuildSimpleDto>> GetGuildListForRealm(string realm, Region region);
    }
}