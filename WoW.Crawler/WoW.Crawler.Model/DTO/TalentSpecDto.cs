using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Crawler.Model.DTO
{
    public class TalentSpecDto
    {
        public bool Selected { get; set; }

        public IEnumerable<TalentDto> Talents { get; set; }

        public GlyphListDto Glyphs { get; set; }

        public SpecDto Spec { get; set; }
    }
}