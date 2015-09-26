using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Crawler.Model.Enum
{
    #region Region

    public enum Region
    {
        [Description("eu")]
        EU = 0,

        [Description("us")]
        US = 1
    }

    public enum Faction
    {
        [Description("alliance")]
        Alliance = 0,

        [Description("horde")]
        Horde = 1,

        [Description("neutral")]
        Neutral = 2,
    }

    public enum Gender
    {
        [Description("male")]
        Male = 0,

        [Description("female")]
        Female = 1
    }

    public enum Class
    {
        [Description("warrior")]
        Warrior = 1,

        [Description("paladin")]
        Paladin = 2,

        [Description("hunter")]
        Hunter = 3,

        [Description("rogue")]
        Rogue = 4,

        [Description("priest")]
        Priest = 5,

        [Description("deathknight")]
        DeathKnight = 6,

        [Description("shaman")]
        Shaman = 7,

        [Description("mage")]
        Mage = 8,

        [Description("warlock")]
        Warlock = 9,

        [Description("monk")]
        Monk = 10,

        [Description("druid")]
        Druid = 11
    }

    public enum Race
    {
        [Description("human")]
        Human = 1,

        [Description("orc")]
        Orc = 2,

        [Description("dwarf")]
        Dwarf = 3,

        [Description("nightelf")]
        NightElf = 4,

        [Description("undead")]
        Undead = 5,

        [Description("tauren")]
        Tauren = 6,

        [Description("gnome")]
        Gnome = 7,

        [Description("troll")]
        Troll = 8,

        [Description("goblin")]
        Goblin = 9,

        [Description("bloodelf")]
        BloodElf = 10,

        [Description("draenei")]
        Draenei = 11,

        [Description("worgen")]
        Worgen = 22,

        [Description("pandarenneutral")]
        PandarenNeutral = 24,

        [Description("pandarenalliance")]
        PandarenAlliance = 25,

        [Description("pandarenhorde")]
        PandarenHorde = 26
    }

    #endregion Region
}