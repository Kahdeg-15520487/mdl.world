using System;
using System.Collections.Generic;

namespace mdl.worlddata.Characters
{
    // Historic figures
    public class HistoricFigure
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Race { get; set; } = string.Empty;
        public string Class { get; set; } = string.Empty;
        public DateTime? BirthDate { get; set; }
        public DateTime? DeathDate { get; set; }
        public bool IsAlive { get; set; } = true;
        public string BirthPlaceId { get; set; } = string.Empty;
        public List<string> AssociatedPlaceIds { get; set; } = new List<string>();
        public List<string> Achievements { get; set; } = new List<string>();
        public List<string> RelatedEventIds { get; set; } = new List<string>();
        public Dictionary<string, int> Attributes { get; set; } = new Dictionary<string, int>();
        public List<string> Relationships { get; set; } = new List<string>();
    }
}
