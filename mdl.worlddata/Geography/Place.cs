using System;
using System.Collections.Generic;

namespace mdl.worlddata.Geography
{
    // Geographic hierarchy
    public class Place
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public PlaceType Type { get; set; }
        public string? ParentPlaceId { get; set; } // For hierarchy
        public List<string> ChildPlaceIds { get; set; } = new List<string>();
        public GeographicInfo Geography { get; set; } = new GeographicInfo();
        public Population Population { get; set; } = new Population();
        public List<Events.RegionalEvent> RegionalEvents { get; set; } = new List<Events.RegionalEvent>();
        public List<string> NotableFeatures { get; set; } = new List<string>();
        public Dictionary<string, string> CustomProperties { get; set; } = new Dictionary<string, string>();
    }

    public enum PlaceType
    {
        World,
        Continent,
        Country,
        Region,
        Province,
        City,
        Town,
        Village,
        District,
        Building,
        Room,
        NaturalFeature,
        Dungeon,
        Other
    }

    public class GeographicInfo
    {
        public string Climate { get; set; } = string.Empty;
        public string Terrain { get; set; } = string.Empty;
        public List<string> NaturalResources { get; set; } = new List<string>();
        public Coordinates Coordinates { get; set; } = new Coordinates();
        public double Area { get; set; } // in square kilometers
        public string[] Borders { get; set; } = Array.Empty<string>();
    }

    public class Coordinates
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Elevation { get; set; } // in meters
    }

    public class Population
    {
        public int TotalCount { get; set; }
        public Dictionary<string, int> RaceDistribution { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> ClassDistribution { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> AgeDistribution { get; set; } = new Dictionary<string, int>();
        public string GovernmentType { get; set; } = string.Empty;
        public List<string> Languages { get; set; } = new List<string>();
        public List<string> Religions { get; set; } = new List<string>();
    }
}
