using mdl.worlddata.Core;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace mdl.world.Services
{
    /// <summary>
    /// JSON file-based world storage service
    /// </summary>
    public class JsonWorldStorageService : IWorldStorageService
    {
        private readonly ILogger<JsonWorldStorageService> _logger;
        private readonly string _storageDirectory;
        private readonly JsonSerializerOptions _jsonOptions;

        public JsonWorldStorageService(ILogger<JsonWorldStorageService> logger, IConfiguration configuration)
        {
            _logger = logger;
            
            // Get storage directory from configuration or use default
            _storageDirectory = configuration.GetValue<string>("WorldStorage:Directory") ?? 
                               Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "MDL_Worlds");
            
            // Ensure directory exists
            Directory.CreateDirectory(_storageDirectory);
            
            // Configure JSON serialization options
            _jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                Converters =
                {
                    new JsonStringEnumConverter()
                }
            };
            
            _logger.LogInformation("World storage initialized at: {Directory}", _storageDirectory);
        }

        public async Task<World> SaveWorldAsync(World world)
        {
            try
            {
                // Ensure world has an ID
                if (string.IsNullOrEmpty(world.Id))
                {
                    world.Id = Guid.NewGuid().ToString();
                }

                // Update creation date if not set
                if (world.CreationDate == default)
                {
                    world.CreationDate = DateTime.UtcNow;
                }

                var filePath = GetWorldFilePath(world.Id);
                var json = JsonSerializer.Serialize(world, _jsonOptions);
                
                await File.WriteAllTextAsync(filePath, json);
                
                _logger.LogInformation("World saved successfully: {WorldId} - {WorldName}", world.Id, world.Name);
                
                return world;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving world: {WorldId} - {WorldName}", world.Id, world.Name);
                throw;
            }
        }

        public async Task<World?> LoadWorldAsync(string worldId)
        {
            try
            {
                var filePath = GetWorldFilePath(worldId);
                
                if (!File.Exists(filePath))
                {
                    _logger.LogWarning("World file not found: {WorldId}", worldId);
                    return null;
                }

                var json = await File.ReadAllTextAsync(filePath);
                var world = JsonSerializer.Deserialize<World>(json, _jsonOptions);
                
                if (world != null)
                {
                    _logger.LogInformation("World loaded successfully: {WorldId} - {WorldName}", world.Id, world.Name);
                }
                
                return world;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading world: {WorldId}", worldId);
                throw;
            }
        }

        public async Task<List<WorldMetadata>> GetAllWorldsAsync()
        {
            try
            {
                var worlds = new List<WorldMetadata>();
                var files = Directory.GetFiles(_storageDirectory, "*.json");
                
                foreach (var file in files)
                {
                    try
                    {
                        var json = await File.ReadAllTextAsync(file);
                        var world = JsonSerializer.Deserialize<World>(json, _jsonOptions);
                        
                        if (world != null)
                        {
                            var fileInfo = new FileInfo(file);
                            worlds.Add(new WorldMetadata
                            {
                                Id = world.Id,
                                Name = world.Name,
                                Description = world.Description,
                                CreationDate = world.CreationDate,
                                LastModified = fileInfo.LastWriteTime,
                                Genre = world.WorldInfo.Genre,
                                Theme = world.WorldInfo.ActiveThemes.FirstOrDefault() ?? "",
                                PlaceCount = world.Places.Count,
                                CharacterCount = world.HistoricFigures.Count,
                                EventCount = world.WorldEvents.Count,
                                ItemCount = world.Equipment.Count + world.SpellBooks.Count + world.RunesOfPower.Count + world.AlchemyRecipes.Count + world.TechnicalSpecs.Count,
                                FileSizeBytes = fileInfo.Length
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Error reading world file: {FilePath}", file);
                    }
                }
                
                _logger.LogInformation("Retrieved {Count} worlds from storage", worlds.Count);
                return worlds.OrderByDescending(w => w.LastModified).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving worlds from storage");
                throw;
            }
        }

        public Task<bool> DeleteWorldAsync(string worldId)
        {
            try
            {
                var filePath = GetWorldFilePath(worldId);
                
                if (!File.Exists(filePath))
                {
                    _logger.LogWarning("World file not found for deletion: {WorldId}", worldId);
                    return Task.FromResult(false);
                }

                File.Delete(filePath);
                _logger.LogInformation("World deleted successfully: {WorldId}", worldId);
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting world: {WorldId}", worldId);
                throw;
            }
        }

        public Task<bool> WorldExistsAsync(string worldId)
        {
            try
            {
                var filePath = GetWorldFilePath(worldId);
                return Task.FromResult(File.Exists(filePath));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking world existence: {WorldId}", worldId);
                return Task.FromResult(false);
            }
        }

        private string GetWorldFilePath(string worldId)
        {
            // Sanitize world ID to be safe for file names
            var safeId = string.Join("_", worldId.Split(Path.GetInvalidFileNameChars()));
            return Path.Combine(_storageDirectory, $"{safeId}.json");
        }
    }
}
