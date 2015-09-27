using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoW.Crawler.Model.Enum;

namespace WoW.Crawler.Model.DTO
{
    public class RealmDto
    {
        #region Blizzard Fields

        public string Type { get; set; }

        public string Population { get; set; }

        public string Name { get; set; }

        public string Slug { get; set; }

        public string Locale { get; set; }

        public string TimeZone { get; set; }

        #endregion Blizzard Fields

        #region Post-Processing Fields

        public Region Region { get; set; }

        #endregion Post-Processing Fields
    }
}