using mdl.worlddata.Core;
using mdl.world.Controllers;

namespace mdl.world.Services
{
    public interface IWorldGenerationService
    {
        /// <summary>
        /// Generates a new fantasy world with sci-fi elements
        /// </summary>
        /// <param name="worldName">Name of the world to generate</param>
        /// <param name="theme">Primary theme (e.g., "Fantasy-SciFi", "Cyberpunk-Fantasy", "Space-Magic")</param>
        /// <param name="techLevel">Technology level (1-10, where 1 is primitive, 10 is advanced)</param>
        /// <param name="magicLevel">Magic level (1-10, where 1 is rare, 10 is abundant)</param>
        /// <returns>A fully generated world with fantasy and sci-fi elements</returns>
        Task<World> GenerateWorldAsync(string worldName, string theme = "Fantasy-SciFi", int techLevel = 5, int magicLevel = 7);

        /// <summary>
        /// Generates additional content for an existing world
        /// </summary>
        /// <param name="world">The existing world to enhance</param>
        /// <param name="contentType">Type of content to generate (Places, Characters, Events, etc.)</param>
        /// <returns>The enhanced world</returns>
        Task<World> EnhanceWorldAsync(World world, string contentType);

        /// <summary>
        /// Generates a world based on custom parameters
        /// </summary>
        /// <param name="parameters">Custom generation parameters</param>
        /// <returns>A generated world</returns>
        Task<World> GenerateCustomWorldAsync(WorldGenerationParameters parameters);

        /// <summary>
        /// Generates a complete world with detailed specification of place types, regions, and content amounts
        /// </summary>
        /// <param name="parameters">Basic generation parameters</param>
        /// <param name="request">Complete world request with detailed specifications</param>
        /// <returns>A fully generated world with specified content amounts</returns>
        Task<World> GenerateCompleteWorldAsync(WorldGenerationParameters parameters, CompleteWorldRequest request);
    }

    public class WorldGenerationParameters
    {
        public string WorldName { get; set; } = string.Empty;
        public string Theme { get; set; } = "Fantasy-SciFi";
        public int TechLevel { get; set; } = 5;
        public int MagicLevel { get; set; } = 7;
        public List<string> PreferredBiomes { get; set; } = new List<string>();
        public List<string> PreferredRaces { get; set; } = new List<string>();
        public List<string> PreferredTechnologies { get; set; } = new List<string>();
        public List<string> PreferredMagicSchools { get; set; } = new List<string>();
        public bool IncludeAncientRuins { get; set; } = true;
        public bool IncludeSpaceTravel { get; set; } = false;
        public bool IncludeMagicTech { get; set; } = true;
        public int WorldSize { get; set; } = 50; // Number of locations to generate
        public string DifficultyLevel { get; set; } = "Medium";
    }
}
