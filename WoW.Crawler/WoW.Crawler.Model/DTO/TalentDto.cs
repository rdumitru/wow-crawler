using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Crawler.Model.DTO
{
    public class TalentDto
    {
        public int Tier { get; set; }

        public int Column { get; set; }

        public SpellDto Spell { get; set; }
    }
}