using System;
using System.Collections.Generic;

namespace mdl.worlddata.Core
{
    // Core world data structure
    public class World
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreationDate { get; set; } = DateTime.UtcNow;
        public WorldInfo WorldInfo { get; set; } = new WorldInfo();
        public List<Geography.Place> Places { get; set; } = new List<Geography.Place>();
        public List<Characters.HistoricFigure> HistoricFigures { get; set; } = new List<Characters.HistoricFigure>();
        public List<Events.WorldEvent> WorldEvents { get; set; } = new List<Events.WorldEvent>();
        public List<Items.Equipment> Equipment { get; set; } = new List<Items.Equipment>();
        public List<Magic.SpellBook> SpellBooks { get; set; } = new List<Magic.SpellBook>();
        public List<Magic.RuneOfPower> RunesOfPower { get; set; } = new List<Magic.RuneOfPower>();
        public List<Magic.AlchemyRecipe> AlchemyRecipes { get; set; } = new List<Magic.AlchemyRecipe>();
        public List<Technology.TechnicalSpecification> TechnicalSpecs { get; set; } = new List<Technology.TechnicalSpecification>();
    }

    // World metadata and settings
    public class WorldInfo
    {
        public string Genre { get; set; } = string.Empty; // Fantasy, SciFi, Modern, etc.
        public string TimeEra { get; set; } = string.Empty; // Medieval, Future, Present
        public string MagicLevel { get; set; } = string.Empty; // None, Low, High
        public string TechnologyLevel { get; set; } = string.Empty; // Stone Age, Industrial, Space Age
        public Dictionary<string, string> CustomSettings { get; set; } = new Dictionary<string, string>();
        public List<string> ActiveThemes { get; set; } = new List<string>();
        public WorldLaws Laws { get; set; } = new WorldLaws();
    }

    public class WorldLaws
    {
        public bool MagicExists { get; set; }
        public bool DeathIsPermanent { get; set; } = true;
        public bool TimeTravel { get; set; } = false;
        public bool Multiverse { get; set; } = false;
        public Dictionary<string, object> CustomLaws { get; set; } = new Dictionary<string, object>();
    }
}
