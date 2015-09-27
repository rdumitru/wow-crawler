using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Crawler.Model.DTO
{
    public class GuildMemberDto
    {
        public GuildMemberCharacterDto Character { get; set; }

        public int Rank { get; set; }
    }
}