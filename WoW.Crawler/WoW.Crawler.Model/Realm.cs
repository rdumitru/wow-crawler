using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoW.Crawler.Model.Enum;

namespace WoW.Crawler.Model
{
    public class Realm
    {
        #region Blizzard Fields

        public string Name { get; set; }

        public string Slug { get; set; }

        public string Locale { get; set; }

        #endregion Blizzard Fields

        #region Post-Processing Fields

        public Region Region { get; set; }

        #endregion Post-Processing Fields
    }
}