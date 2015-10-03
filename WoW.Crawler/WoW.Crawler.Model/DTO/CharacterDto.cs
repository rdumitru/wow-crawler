using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoW.Crawler.Model.Enum;

namespace WoW.Crawler.Model.DTO
{
    public class CharacterDto
    {
        #region Blizzard Fields

        public DateTime LastModified { get; set; }

        public string Name { get; set; }

        [JsonProperty("Realm")]
        public string RealmName { get; set; }

        public Class Class { get; set; }

        public Race Race { get; set; }

        public Gender Gender { get; set; }

        public int Level { get; set; }

        public int TotalHonorableKills { get; set; }

        public GuildSimpleDto Guild { get; set; }

        public IEnumerable<TalentSpecDto> Talents { get; set; }

        #endregion Blizzard Fields

        #region Post-Processing Fields

        public Faction Faction { get; set; }

        public Region Region { get; set; }

        #endregion Post-Processing Fields

        #region Helpers

        public void SetFactionFromRace()
        {
            switch (this.Race)
            {
                case Race.Human:
                case Race.Dwarf:
                case Race.NightElf:
                case Race.Gnome:
                case Race.Draenei:
                case Race.Worgen:
                case Race.PandarenAlliance:
                    this.Faction = Faction.Alliance;
                    return;

                case Race.Orc:
                case Race.Undead:
                case Race.Tauren:
                case Race.Troll:
                case Race.Goblin:
                case Race.BloodElf:
                case Race.PandarenHorde:
                    this.Faction = Faction.Horde;
                    return;

                case Race.PandarenNeutral:
                    this.Faction = Faction.Neutral;
                    return;

                default:
                    this.Faction = Faction.Unknown;
                    return;
            }
        }

        #endregion Helpers
    }
}