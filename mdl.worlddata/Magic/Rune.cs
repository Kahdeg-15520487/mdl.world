using System;
using System.Collections.Generic;

namespace mdl.worlddata.Magic
{
    // Runes of power
    public class RuneOfPower
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public string Symbol { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public RuneType Type { get; set; }
        public int PowerLevel { get; set; }
        public string Element { get; set; } = string.Empty;
        public List<string> Effects { get; set; } = new List<string>();
        public string ActivationCondition { get; set; } = string.Empty;
        public bool IsActive { get; set; } = false;
        public string Location { get; set; } = string.Empty;
        public string CreatorId { get; set; } = string.Empty;
    }

    public enum RuneType
    {
        Protection,
        Enhancement,
        Destruction,
        Binding,
        Summoning,
        Divination,
        Healing,
        Transformation,
        Other
    }
}
