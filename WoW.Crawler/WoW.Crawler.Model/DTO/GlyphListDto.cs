using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Crawler.Model.DTO
{
    public class GlyphListDto
    {
        public IEnumerable<GlyphDto> Major { get; set; }

        public IEnumerable<GlyphDto> Minor { get; set; }
    }
}