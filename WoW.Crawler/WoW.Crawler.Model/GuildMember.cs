using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Crawler.Model
{
    public class GuildMember
    {
        public CharacterWithSpec Character { get; set; }

        public int Rank { get; set; }
    }
}