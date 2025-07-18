using mdl.worlddata.Core;

namespace mdl.world.Services
{
    /// <summary>
    /// Interface for world storage operations
    /// </summary>
    public interface IWorldStorageService
    {
        /// <summary>
        /// Save a world to JSON storage
        /// </summary>
        /// <param name="world">The world to save</param>
        /// <returns>The saved world with updated metadata</returns>
        Task<World> SaveWorldAsync(World world);

        /// <summary>
        /// Load a world from JSON storage by ID
        /// </summary>
        /// <param name="worldId">The world ID to load</param>
        /// <returns>The loaded world or null if not found</returns>
        Task<World?> LoadWorldAsync(string worldId);

        /// <summary>
        /// Get all stored worlds (metadata only)
        /// </summary>
        /// <returns>List of world metadata</returns>
        Task<List<WorldMetadata>> GetAllWorldsAsync();

        /// <summary>
        /// Delete a world from storage
        /// </summary>
        /// <param name="worldId">The world ID to delete</param>
        /// <returns>True if deleted successfully</returns>
        Task<bool> DeleteWorldAsync(string worldId);

        /// <summary>
        /// Check if a world exists in storage
        /// </summary>
        /// <param name="worldId">The world ID to check</param>
        /// <returns>True if the world exists</returns>
        Task<bool> WorldExistsAsync(string worldId);
    }

    /// <summary>
    /// Metadata for stored worlds
    /// </summary>
    public class WorldMetadata
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreationDate { get; set; }
        public DateTime LastModified { get; set; }
        public string Genre { get; set; } = string.Empty;
        public string Theme { get; set; } = string.Empty;
        public int PlaceCount { get; set; }
        public int CharacterCount { get; set; }
        public int EventCount { get; set; }
        public int ItemCount { get; set; }
        public long FileSizeBytes { get; set; }
    }
}
