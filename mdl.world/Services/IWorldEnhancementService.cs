using mdl.worlddata.Core;
using System.Text.Json;

namespace mdl.world.Services
{
    public interface IWorldEnhancementService
    {
        /// <summary>
        /// Enhance a world by adding user comments and regenerating relevant parts
        /// </summary>
        /// <param name="world">The existing world to enhance</param>
        /// <param name="userComment">User's comment describing desired changes</param>
        /// <param name="targetSection">Specific section to enhance (optional)</param>
        /// <returns>Enhanced world with updated JSON and generated descriptions</returns>
        Task<WorldEnhancementResult> EnhanceWorldAsync(World world, string userComment, string? targetSection = null);

        /// <summary>
        /// Regenerate a specific part of the world based on user feedback
        /// </summary>
        /// <param name="world">The world to modify</param>
        /// <param name="sectionType">Type of section to regenerate (places, characters, events, etc.)</param>
        /// <param name="sectionId">ID of the specific item to regenerate</param>
        /// <param name="userComment">User's guidance for regeneration</param>
        /// <returns>Updated world with regenerated content</returns>
        Task<WorldEnhancementResult> RegenerateWorldSectionAsync(World world, string sectionType, string sectionId, string userComment);

        /// <summary>
        /// Add new content to world based on user description
        /// </summary>
        /// <param name="world">The world to enhance</param>
        /// <param name="contentType">Type of content to add</param>
        /// <param name="userDescription">User's description of what to add</param>
        /// <returns>World with new content added</returns>
        Task<WorldEnhancementResult> AddContentToWorldAsync(World world, string contentType, string userDescription);

        /// <summary>
        /// Generate a comprehensive narrative for the entire world
        /// </summary>
        /// <param name="world">The world to describe</param>
        /// <returns>Complete narrative description</returns>
        Task<string> GenerateWorldNarrativeAsync(World world);

        /// <summary>
        /// Update world properties based on LLM analysis of user comments
        /// </summary>
        /// <param name="world">The world to update</param>
        /// <param name="userComment">User's comment</param>
        /// <returns>Updated world with modified properties</returns>
        Task<World> UpdateWorldPropertiesAsync(World world, string userComment);
    }

    public class WorldEnhancementResult
    {
        public World UpdatedWorld { get; set; } = new();
        public string GeneratedNarrative { get; set; } = string.Empty;
        public List<string> ChangesApplied { get; set; } = new();
        public string UserComment { get; set; } = string.Empty;
        public DateTime UpdateTimestamp { get; set; } = DateTime.UtcNow;
        public Dictionary<string, string> SectionNarratives { get; set; } = new();
    }

    public class WorldUpdateInstruction
    {
        public string Action { get; set; } = string.Empty; // add, modify, remove
        public string Target { get; set; } = string.Empty; // places, characters, events, etc.
        public string Description { get; set; } = string.Empty;
        public Dictionary<string, object> Properties { get; set; } = new();
    }
}
