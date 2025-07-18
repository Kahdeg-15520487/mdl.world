using System;
using System.Collections.Generic;

namespace mdl.worlddata.Events
{
    // Event hierarchy
    public abstract class BaseEvent
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public EventStatus Status { get; set; } = EventStatus.Historical;
        public List<string> ParticipantIds { get; set; } = new List<string>(); // Historic figures
        public List<string> AffectedPlaceIds { get; set; } = new List<string>();
        public Dictionary<string, string> Consequences { get; set; } = new Dictionary<string, string>();
    }

    public class WorldEvent : BaseEvent
    {
        public WorldEventType Type { get; set; }
        public int GlobalImpactLevel { get; set; } // 1-10 scale
    }

    public class RegionalEvent : BaseEvent
    {
        public RegionalEventType Type { get; set; }
        public string PrimaryPlaceId { get; set; } = string.Empty;
        public int RegionalImpactLevel { get; set; } // 1-10 scale
    }

    public class MinorEvent : BaseEvent
    {
        public MinorEventType Type { get; set; }
        public List<string> AffectedGroupIds { get; set; } = new List<string>();
        public int LocalImpactLevel { get; set; } // 1-5 scale
    }

    public enum EventStatus
    {
        Planned,
        Ongoing,
        Historical,
        Legendary,
        Mythical
    }

    public enum WorldEventType
    {
        Creation,
        Apocalypse,
        DivineIntervention,
        MagicalCatastrophe,
        TechnologicalSingularity,
        PlaneShift,
        TimeDistortion,
        Other
    }

    public enum RegionalEventType
    {
        War,
        Revolution,
        NaturalDisaster,
        Plague,
        Discovery,
        Conquest,
        Trade,
        Migration,
        Other
    }

    public enum MinorEventType
    {
        Festival,
        Crime,
        Birth,
        Death,
        Marriage,
        Duel,
        Theft,
        Rescue,
        Meeting,
        Other
    }
}
