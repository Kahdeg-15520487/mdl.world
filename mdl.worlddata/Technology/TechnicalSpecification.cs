using System;
using System.Collections.Generic;

namespace mdl.worlddata.Technology
{
    // Technical specifications for sci-fi elements
    public class TechnicalSpecification
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TechSpecType Type { get; set; }
        public int TechLevel { get; set; }
        public string Manufacturer { get; set; } = string.Empty;
        public string ModelNumber { get; set; } = string.Empty;
        public Dictionary<string, string> Specifications { get; set; } = new Dictionary<string, string>();
        public List<string> Requirements { get; set; } = new List<string>();
        public List<string> Capabilities { get; set; } = new List<string>();
        public string PowerConsumption { get; set; } = string.Empty;
        public string MaintenanceSchedule { get; set; } = string.Empty;
        public bool IsClassified { get; set; } = false;
    }

    public enum TechSpecType
    {
        Weapon,
        Vehicle,
        Computer,
        Communication,
        Medical,
        Manufacturing,
        Defense,
        Exploration,
        Other
    }
}
