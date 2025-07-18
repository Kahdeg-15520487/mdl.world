using mdl.worlddata.Core;
using mdl.worlddata.Geography;
using mdl.worlddata.Characters;
using mdl.worlddata.Events;
using mdl.worlddata.Items;
using mdl.worlddata.Magic;
using mdl.worlddata.Technology;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace mdl.world.Services
{
    public class WorldEnhancementService : IWorldEnhancementService
    {
        private readonly ILLMTextGenerationService _llmService;
        private readonly IWorldGenerationService _worldGenerationService;
        private readonly ILogger<WorldEnhancementService> _logger;

        public WorldEnhancementService(
            ILLMTextGenerationService llmService,
            IWorldGenerationService worldGenerationService,
            ILogger<WorldEnhancementService> logger)
        {
            _llmService = llmService;
            _worldGenerationService = worldGenerationService;
            _logger = logger;
        }

        public async Task<WorldEnhancementResult> EnhanceWorldAsync(World world, string userComment, string? targetSection = null)
        {
            _logger.LogInformation("Enhancing world '{WorldName}' with user comment: {Comment}", world.Name, userComment);

            var result = new WorldEnhancementResult
            {
                UpdatedWorld = world,
                UserComment = userComment
            };

            try
            {
                // Step 1: Analyze user comment to understand what changes are needed
                var instructions = await AnalyzeUserCommentAsync(world, userComment);
                
                // Step 2: Apply changes to the world JSON structure
                await ApplyInstructionsToWorldAsync(result.UpdatedWorld, instructions);
                
                // Step 3: Generate new content if needed
                await GenerateNewContentAsync(result.UpdatedWorld, instructions);
                
                // Step 4: Generate narratives for changed sections
                await GenerateSectionNarrativesAsync(result, targetSection);
                
                // Step 5: Generate overall world narrative
                result.GeneratedNarrative = await GenerateWorldNarrativeAsync(result.UpdatedWorld);
                
                // Step 6: Document changes applied
                result.ChangesApplied = instructions.Select(i => $"{i.Action} {i.Target}: {i.Description}").ToList();

                _logger.LogInformation("Successfully enhanced world '{WorldName}' with {ChangeCount} changes", 
                    world.Name, result.ChangesApplied.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enhancing world '{WorldName}'", world.Name);
                throw;
            }

            return result;
        }

        public async Task<WorldEnhancementResult> RegenerateWorldSectionAsync(World world, string sectionType, string sectionId, string userComment)
        {
            _logger.LogInformation("Regenerating {SectionType} section '{SectionId}' in world '{WorldName}'", 
                sectionType, sectionId, world.Name);

            var result = new WorldEnhancementResult
            {
                UpdatedWorld = world,
                UserComment = userComment
            };

            try
            {
                // Find and regenerate the specific section
                switch (sectionType.ToLower())
                {
                    case "places":
                        await RegeneratePlaceAsync(result.UpdatedWorld, sectionId, userComment);
                        break;
                    case "characters":
                        await RegenerateCharacterAsync(result.UpdatedWorld, sectionId, userComment);
                        break;
                    case "events":
                        await RegenerateEventAsync(result.UpdatedWorld, sectionId, userComment);
                        break;
                    case "technology":
                        await RegenerateTechnologyAsync(result.UpdatedWorld, sectionId, userComment);
                        break;
                    case "magic":
                        await RegenerateMagicAsync(result.UpdatedWorld, sectionId, userComment);
                        break;
                    default:
                        throw new ArgumentException($"Unknown section type: {sectionType}");
                }

                // Generate narrative for the regenerated section
                await GenerateSectionNarrativesAsync(result, sectionType);
                
                result.ChangesApplied.Add($"Regenerated {sectionType} section '{sectionId}' based on user feedback");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error regenerating {SectionType} section '{SectionId}'", sectionType, sectionId);
                throw;
            }

            return result;
        }

        public async Task<WorldEnhancementResult> AddContentToWorldAsync(World world, string contentType, string userDescription)
        {
            _logger.LogInformation("Adding {ContentType} content to world '{WorldName}': {Description}", 
                contentType, world.Name, userDescription);

            var result = new WorldEnhancementResult
            {
                UpdatedWorld = world,
                UserComment = userDescription
            };

            try
            {
                // Generate new content based on user description
                await GenerateNewContentByTypeAsync(result.UpdatedWorld, contentType, userDescription);
                
                // Generate narratives for new content
                await GenerateSectionNarrativesAsync(result, contentType);
                
                result.ChangesApplied.Add($"Added new {contentType} content: {userDescription}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding {ContentType} content to world '{WorldName}'", contentType, world.Name);
                throw;
            }

            return result;
        }

        public async Task<string> GenerateWorldNarrativeAsync(World world)
        {
            return await _llmService.GenerateWorldNarrativeAsync(world);
        }

        public async Task<World> UpdateWorldPropertiesAsync(World world, string userComment)
        {
            _logger.LogInformation("Updating properties for world '{WorldName}' based on comment: {Comment}", 
                world.Name, userComment);

            try
            {
                // Use LLM to analyze comment and suggest property changes
                var analysisPrompt = $@"
                Analyze this user comment about a fantasy/sci-fi world and suggest specific property changes:
                
                World: {world.Name}
                Current Description: {world.Description}
                Current Genre: {world.WorldInfo.Genre}
                Current Tech Level: {world.WorldInfo.TechnologyLevel}
                Current Magic Level: {world.WorldInfo.MagicLevel}
                
                User Comment: {userComment}
                
                Please provide a JSON response with suggested property updates in this format:
                {{
                    ""description"": ""updated description"",
                    ""genre"": ""updated genre"",
                    ""technologyLevel"": ""updated tech level"",
                    ""magicLevel"": ""updated magic level"",
                    ""customSettings"": {{
                        ""key"": ""value""
                    }},
                    ""activeThemes"": [""theme1"", ""theme2""]
                }}";

                var analysisResult = await _llmService.GenerateTextFromJsonAsync(world, analysisPrompt);
                
                // Parse the LLM response and apply changes
                await ApplyPropertyChangesAsync(world, analysisResult, userComment);
                
                return world;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating world properties");
                return world;
            }
        }

        private async Task<List<WorldUpdateInstruction>> AnalyzeUserCommentAsync(World world, string userComment)
        {
            var analysisPrompt = $@"
            Analyze this user comment about a fantasy/sci-fi world and break it down into specific update instructions.
            
            World: {world.Name}
            Current state: {JsonSerializer.Serialize(world, new JsonSerializerOptions { WriteIndented = true })}
            
            User Comment: {userComment}
            
            Please provide a JSON array of update instructions in this format:
            [
                {{
                    ""action"": ""add|modify|remove"",
                    ""target"": ""places|characters|events|technology|magic|worldinfo"",
                    ""description"": ""specific description of what to change"",
                    ""properties"": {{
                        ""key"": ""value""
                    }}
                }}
            ]";

            var analysisResult = await _llmService.GenerateTextFromJsonAsync(world, analysisPrompt);
            
            // Parse the LLM response to extract instructions
            return ParseUpdateInstructions(analysisResult);
        }

        private List<WorldUpdateInstruction> ParseUpdateInstructions(string llmResponse)
        {
            var instructions = new List<WorldUpdateInstruction>();
            
            try
            {
                // Extract JSON from LLM response
                var jsonMatch = Regex.Match(llmResponse, @"\[.*\]", RegexOptions.Singleline);
                if (jsonMatch.Success)
                {
                    var jsonStr = jsonMatch.Value;
                    instructions = JsonSerializer.Deserialize<List<WorldUpdateInstruction>>(jsonStr) ?? new List<WorldUpdateInstruction>();
                }
                else
                {
                    // Fallback: create a general instruction
                    instructions.Add(new WorldUpdateInstruction
                    {
                        Action = "modify",
                        Target = "worldinfo",
                        Description = llmResponse
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not parse LLM instructions, using fallback");
                instructions.Add(new WorldUpdateInstruction
                {
                    Action = "enhance",
                    Target = "general",
                    Description = llmResponse
                });
            }

            return instructions;
        }

        private async Task ApplyInstructionsToWorldAsync(World world, List<WorldUpdateInstruction> instructions)
        {
            foreach (var instruction in instructions)
            {
                switch (instruction.Target.ToLower())
                {
                    case "places":
                        await ApplyPlaceInstructionAsync(world, instruction);
                        break;
                    case "characters":
                        await ApplyCharacterInstructionAsync(world, instruction);
                        break;
                    case "events":
                        await ApplyEventInstructionAsync(world, instruction);
                        break;
                    case "technology":
                        await ApplyTechnologyInstructionAsync(world, instruction);
                        break;
                    case "magic":
                        await ApplyMagicInstructionAsync(world, instruction);
                        break;
                    case "worldinfo":
                        await ApplyWorldInfoInstructionAsync(world, instruction);
                        break;
                }
            }
        }

        private async Task ApplyPlaceInstructionAsync(World world, WorldUpdateInstruction instruction)
        {
            if (instruction.Action == "add")
            {
                // Generate new place based on instruction
                var newPlace = await GenerateNewPlaceAsync(world, instruction.Description);
                world.Places.Add(newPlace);
            }
            else if (instruction.Action == "modify" && instruction.Properties.ContainsKey("name"))
            {
                var placeName = instruction.Properties["name"].ToString();
                var existingPlace = world.Places.FirstOrDefault(p => p.Name == placeName);
                if (existingPlace != null)
                {
                    // Update place properties
                    await UpdatePlacePropertiesAsync(existingPlace, instruction);
                }
            }
        }

        private async Task ApplyCharacterInstructionAsync(World world, WorldUpdateInstruction instruction)
        {
            if (instruction.Action == "add")
            {
                var newCharacter = await GenerateNewCharacterAsync(world, instruction.Description);
                world.HistoricFigures.Add(newCharacter);
            }
            // Similar pattern for modify/remove
        }

        private async Task ApplyEventInstructionAsync(World world, WorldUpdateInstruction instruction)
        {
            if (instruction.Action == "add")
            {
                var newEvent = await GenerateNewEventAsync(world, instruction.Description);
                world.WorldEvents.Add(newEvent);
            }
            // Similar pattern for modify/remove
        }

        private async Task ApplyTechnologyInstructionAsync(World world, WorldUpdateInstruction instruction)
        {
            if (instruction.Action == "add")
            {
                var newTech = await GenerateNewTechnologyAsync(world, instruction.Description);
                world.TechnicalSpecs.Add(newTech);
            }
            // Similar pattern for modify/remove
        }

        private async Task ApplyMagicInstructionAsync(World world, WorldUpdateInstruction instruction)
        {
            if (instruction.Action == "add")
            {
                // Could be spell, rune, or alchemy - determine from description
                if (instruction.Description.ToLower().Contains("spell"))
                {
                    var newSpellBook = await GenerateNewSpellBookAsync(world, instruction.Description);
                    world.SpellBooks.Add(newSpellBook);
                }
                else if (instruction.Description.ToLower().Contains("rune"))
                {
                    var newRune = await GenerateNewRuneAsync(world, instruction.Description);
                    world.RunesOfPower.Add(newRune);
                }
                else if (instruction.Description.ToLower().Contains("alchemy"))
                {
                    var newAlchemy = await GenerateNewAlchemyAsync(world, instruction.Description);
                    world.AlchemyRecipes.Add(newAlchemy);
                }
            }
        }

        private async Task ApplyWorldInfoInstructionAsync(World world, WorldUpdateInstruction instruction)
        {
            // Apply world info changes based on instruction
            if (instruction.Properties.ContainsKey("description"))
            {
                world.Description = instruction.Properties["description"].ToString() ?? world.Description;
            }
            if (instruction.Properties.ContainsKey("genre"))
            {
                world.WorldInfo.Genre = instruction.Properties["genre"].ToString() ?? world.WorldInfo.Genre;
            }
            if (instruction.Properties.ContainsKey("technologyLevel"))
            {
                world.WorldInfo.TechnologyLevel = instruction.Properties["technologyLevel"].ToString() ?? world.WorldInfo.TechnologyLevel;
            }
            if (instruction.Properties.ContainsKey("magicLevel"))
            {
                world.WorldInfo.MagicLevel = instruction.Properties["magicLevel"].ToString() ?? world.WorldInfo.MagicLevel;
            }
            
            // If there are theme-related changes, use LLM to generate new themes
            if (instruction.Description.ToLower().Contains("theme"))
            {
                var themePrompt = $@"Based on this instruction: '{instruction.Description}', 
                suggest 3-5 active themes for the world '{world.Name}' that match the current genre '{world.WorldInfo.Genre}'.
                Return as a JSON array: [""theme1"", ""theme2"", ""theme3""]";
                
                var themeResponse = await _llmService.GenerateTextFromJsonAsync(world, themePrompt);
                
                try
                {
                    var jsonMatch = Regex.Match(themeResponse, @"\[.*\]", RegexOptions.Singleline);
                    if (jsonMatch.Success)
                    {
                        var themes = JsonSerializer.Deserialize<List<string>>(jsonMatch.Value);
                        if (themes != null)
                        {
                            world.WorldInfo.ActiveThemes = themes;
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Could not parse theme generation response");
                }
            }
        }

        private async Task GenerateNewContentAsync(World world, List<WorldUpdateInstruction> instructions)
        {
            // Handle complex content generation scenarios based on multiple instructions
            var addInstructions = instructions.Where(i => i.Action == "add").ToList();
            
            if (addInstructions.Any())
            {
                _logger.LogInformation("Processing {Count} content generation instructions", addInstructions.Count);
                
                // Group instructions by target type for batch processing
                var groupedInstructions = addInstructions.GroupBy(i => i.Target.ToLower());
                
                foreach (var group in groupedInstructions)
                {
                    switch (group.Key)
                    {
                        case "places":
                            await GenerateMultiplePlacesAsync(world, group.ToList());
                            break;
                        case "characters":
                            await GenerateMultipleCharactersAsync(world, group.ToList());
                            break;
                        case "events":
                            await GenerateMultipleEventsAsync(world, group.ToList());
                            break;
                        case "technology":
                            await GenerateMultipleTechnologiesAsync(world, group.ToList());
                            break;
                        case "magic":
                            await GenerateMultipleMagicItemsAsync(world, group.ToList());
                            break;
                    }
                }
            }
        }
        
        private async Task GenerateMultiplePlacesAsync(World world, List<WorldUpdateInstruction> instructions)
        {
            foreach (var instruction in instructions)
            {
                var place = await GenerateNewPlaceAsync(world, instruction.Description);
                world.Places.Add(place);
            }
        }
        
        private async Task GenerateMultipleCharactersAsync(World world, List<WorldUpdateInstruction> instructions)
        {
            foreach (var instruction in instructions)
            {
                var character = await GenerateNewCharacterAsync(world, instruction.Description);
                world.HistoricFigures.Add(character);
            }
        }
        
        private async Task GenerateMultipleEventsAsync(World world, List<WorldUpdateInstruction> instructions)
        {
            foreach (var instruction in instructions)
            {
                var worldEvent = await GenerateNewEventAsync(world, instruction.Description);
                world.WorldEvents.Add(worldEvent);
            }
        }
        
        private async Task GenerateMultipleTechnologiesAsync(World world, List<WorldUpdateInstruction> instructions)
        {
            foreach (var instruction in instructions)
            {
                var tech = await GenerateNewTechnologyAsync(world, instruction.Description);
                world.TechnicalSpecs.Add(tech);
            }
        }
        
        private async Task GenerateMultipleMagicItemsAsync(World world, List<WorldUpdateInstruction> instructions)
        {
            foreach (var instruction in instructions)
            {
                if (instruction.Description.ToLower().Contains("rune"))
                {
                    var rune = await GenerateNewRuneAsync(world, instruction.Description);
                    world.RunesOfPower.Add(rune);
                }
                else if (instruction.Description.ToLower().Contains("spell"))
                {
                    var spellBook = await GenerateNewSpellBookAsync(world, instruction.Description);
                    world.SpellBooks.Add(spellBook);
                }
                else if (instruction.Description.ToLower().Contains("alchemy"))
                {
                    var recipe = await GenerateNewAlchemyAsync(world, instruction.Description);
                    world.AlchemyRecipes.Add(recipe);
                }
            }
        }

        private async Task GenerateNewContentByTypeAsync(World world, string contentType, string description)
        {
            switch (contentType.ToLower())
            {
                case "place":
                case "places":
                    var newPlace = await GenerateNewPlaceAsync(world, description);
                    world.Places.Add(newPlace);
                    break;
                case "character":
                case "characters":
                    var newCharacter = await GenerateNewCharacterAsync(world, description);
                    world.HistoricFigures.Add(newCharacter);
                    break;
                case "event":
                case "events":
                    var newEvent = await GenerateNewEventAsync(world, description);
                    world.WorldEvents.Add(newEvent);
                    break;
                case "technology":
                    var newTech = await GenerateNewTechnologyAsync(world, description);
                    world.TechnicalSpecs.Add(newTech);
                    break;
                case "magic":
                    var newRune = await GenerateNewRuneAsync(world, description);
                    world.RunesOfPower.Add(newRune);
                    break;
            }
        }

        private async Task GenerateSectionNarrativesAsync(WorldEnhancementResult result, string? targetSection = null)
        {
            if (targetSection == null)
            {
                // Generate narratives for all sections
                result.SectionNarratives["places"] = await _llmService.GenerateLocationDescriptionAsync(result.UpdatedWorld.Places);
                result.SectionNarratives["characters"] = await _llmService.GenerateCharacterDescriptionAsync(result.UpdatedWorld.HistoricFigures);
                result.SectionNarratives["events"] = await _llmService.GenerateEventNarrativeAsync(result.UpdatedWorld.WorldEvents);
            }
            else
            {
                // Generate narrative for specific section
                switch (targetSection.ToLower())
                {
                    case "places":
                        result.SectionNarratives["places"] = await _llmService.GenerateLocationDescriptionAsync(result.UpdatedWorld.Places);
                        break;
                    case "characters":
                        result.SectionNarratives["characters"] = await _llmService.GenerateCharacterDescriptionAsync(result.UpdatedWorld.HistoricFigures);
                        break;
                    case "events":
                        result.SectionNarratives["events"] = await _llmService.GenerateEventNarrativeAsync(result.UpdatedWorld.WorldEvents);
                        break;
                }
            }
        }

        // Helper methods for generating new content
        private async Task<Place> GenerateNewPlaceAsync(World world, string description)
        {
            // Use world generation service to create new place
            var enhancedWorld = await _worldGenerationService.EnhanceWorldAsync(world, "places");
            return enhancedWorld.Places.LastOrDefault() ?? new Place { Name = "New Place", Description = description };
        }

        private async Task<HistoricFigure> GenerateNewCharacterAsync(World world, string description)
        {
            var enhancedWorld = await _worldGenerationService.EnhanceWorldAsync(world, "characters");
            return enhancedWorld.HistoricFigures.LastOrDefault() ?? new HistoricFigure { Name = "New Character", Description = description };
        }

        private async Task<WorldEvent> GenerateNewEventAsync(World world, string description)
        {
            // Use LLM to generate a detailed event based on the world context
            var prompt = $@"Create a detailed world event for the world '{world.Name}' based on this description: {description}
            
            World Context:
            - Genre: {world.WorldInfo.Genre}
            - Tech Level: {world.WorldInfo.TechnologyLevel}
            - Magic Level: {world.WorldInfo.MagicLevel}
            - Existing Places: {string.Join(", ", world.Places.Take(3).Select(p => p.Name))}
            
            Generate a JSON object with these fields:
            {{
                ""name"": ""Event Name"",
                ""description"": ""Detailed description of what happened"",
                ""startDate"": ""2023-01-01T00:00:00Z"",
                ""endDate"": ""2023-01-02T00:00:00Z"",
                ""consequences"": {{
                    ""political"": ""Political impact"",
                    ""social"": ""Social impact"",
                    ""economic"": ""Economic impact""
                }}
            }}";

            var llmResponse = await _llmService.GenerateTextFromJsonAsync(world, prompt);
            
            // Try to parse the LLM response, fallback to basic event if parsing fails
            try
            {
                var jsonMatch = Regex.Match(llmResponse, @"\{.*\}", RegexOptions.Singleline);
                if (jsonMatch.Success)
                {
                    var eventData = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonMatch.Value);
                    return new WorldEvent 
                    { 
                        Name = eventData?.GetValueOrDefault("name")?.ToString() ?? "New Event",
                        Description = eventData?.GetValueOrDefault("description")?.ToString() ?? description,
                        StartDate = DateTime.TryParse(eventData?.GetValueOrDefault("startDate")?.ToString(), out var startDate) ? startDate : DateTime.Now,
                        EndDate = DateTime.TryParse(eventData?.GetValueOrDefault("endDate")?.ToString(), out var endDate) ? endDate : DateTime.Now.AddDays(1)
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not parse LLM event generation response");
            }
            
            return new WorldEvent { Name = "New Event", Description = description };
        }

        private async Task<TechnicalSpecification> GenerateNewTechnologyAsync(World world, string description)
        {
            // Use LLM to generate detailed technology specification
            var prompt = $@"Create a detailed technology specification for the world '{world.Name}' based on this description: {description}
            
            World Context:
            - Genre: {world.WorldInfo.Genre}
            - Tech Level: {world.WorldInfo.TechnologyLevel}
            - Magic Level: {world.WorldInfo.MagicLevel}
            
            Generate a JSON object with these fields:
            {{
                ""name"": ""Technology Name"",
                ""description"": ""Detailed technical description"",
                ""category"": ""Transportation|Communication|Weapons|Medical|Energy|Other"",
                ""complexity"": ""Simple|Moderate|Complex|Advanced"",
                ""requirements"": [""requirement1"", ""requirement2""],
                ""applications"": [""application1"", ""application2""]
            }}";

            var llmResponse = await _llmService.GenerateTextFromJsonAsync(world, prompt);
            
            try
            {
                var jsonMatch = Regex.Match(llmResponse, @"\{.*\}", RegexOptions.Singleline);
                if (jsonMatch.Success)
                {
                    var techData = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonMatch.Value);
                    var techSpec = new TechnicalSpecification 
                    { 
                        Name = techData?.GetValueOrDefault("name")?.ToString() ?? "New Technology",
                        Description = techData?.GetValueOrDefault("description")?.ToString() ?? description,
                        Manufacturer = "Unknown",
                        PowerConsumption = "Standard"
                    };
                    
                    // Try to parse the category into TechSpecType enum
                    if (techData?.ContainsKey("category") == true)
                    {
                        var categoryStr = techData["category"].ToString();
                        if (Enum.TryParse<TechSpecType>(categoryStr, true, out var techType))
                        {
                            techSpec.Type = techType;
                        }
                    }
                    
                    return techSpec;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not parse LLM technology generation response");
            }
            
            return new TechnicalSpecification { Name = "New Technology", Description = description };
        }

        private async Task<RuneOfPower> GenerateNewRuneAsync(World world, string description)
        {
            // Use LLM to generate detailed rune specification
            var prompt = $@"Create a detailed magical rune for the world '{world.Name}' based on this description: {description}
            
            World Context:
            - Genre: {world.WorldInfo.Genre}
            - Magic Level: {world.WorldInfo.MagicLevel}
            - Tech Level: {world.WorldInfo.TechnologyLevel}
            
            Generate a JSON object with these fields:
            {{
                ""name"": ""Rune Name"",
                ""description"": ""Detailed description of the rune's appearance and power"",
                ""element"": ""Fire|Water|Earth|Air|Shadow|Light|Arcane"",
                ""powerLevel"": ""1-10"",
                ""activationMethod"": ""How to activate the rune"",
                ""effects"": [""effect1"", ""effect2""],
                ""restrictions"": [""restriction1"", ""restriction2""]
            }}";

            var llmResponse = await _llmService.GenerateTextFromJsonAsync(world, prompt);
            
            try
            {
                var jsonMatch = Regex.Match(llmResponse, @"\{.*\}", RegexOptions.Singleline);
                if (jsonMatch.Success)
                {
                    var runeData = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonMatch.Value);
                    var rune = new RuneOfPower 
                    { 
                        Name = runeData?.GetValueOrDefault("name")?.ToString() ?? "New Rune",
                        Description = runeData?.GetValueOrDefault("description")?.ToString() ?? description,
                        Element = runeData?.GetValueOrDefault("element")?.ToString() ?? "Arcane",
                        ActivationCondition = runeData?.GetValueOrDefault("activationMethod")?.ToString() ?? "Touch and speak command word",
                        Symbol = "◈"
                    };
                    
                    // Try to parse power level
                    if (runeData?.ContainsKey("powerLevel") == true && 
                        int.TryParse(runeData["powerLevel"].ToString(), out var powerLevel))
                    {
                        rune.PowerLevel = Math.Clamp(powerLevel, 1, 10);
                    }
                    
                    return rune;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not parse LLM rune generation response");
            }
            
            return new RuneOfPower { Name = "New Rune", Description = description };
        }

        private async Task<AlchemyRecipe> GenerateNewAlchemyAsync(World world, string description)
        {
            // Use LLM to generate detailed alchemy recipe
            var prompt = $@"Create a detailed alchemy recipe for the world '{world.Name}' based on this description: {description}
            
            World Context:
            - Genre: {world.WorldInfo.Genre}
            - Magic Level: {world.WorldInfo.MagicLevel}
            - Tech Level: {world.WorldInfo.TechnologyLevel}
            
            Generate a JSON object with these fields:
            {{
                ""name"": ""Potion/Recipe Name"",
                ""description"": ""Detailed description of what this creates and its effects"",
                ""difficulty"": ""Novice|Apprentice|Journeyman|Expert|Master"",
                ""ingredients"": [""ingredient1"", ""ingredient2"", ""ingredient3""],
                ""instructions"": ""Step by step brewing/crafting instructions"",
                ""effects"": [""effect1"", ""effect2""],
                ""sideEffects"": [""side effect1"", ""side effect2""]
            }}";

            var llmResponse = await _llmService.GenerateTextFromJsonAsync(world, prompt);
            
            try
            {
                var jsonMatch = Regex.Match(llmResponse, @"\{.*\}", RegexOptions.Singleline);
                if (jsonMatch.Success)
                {
                    var alchemyData = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonMatch.Value);
                    var recipe = new AlchemyRecipe 
                    { 
                        Name = alchemyData?.GetValueOrDefault("name")?.ToString() ?? "New Alchemy Recipe",
                        Description = alchemyData?.GetValueOrDefault("description")?.ToString() ?? description,
                        PreparationTime = TimeSpan.FromHours(1)
                    };
                    
                    // Try to parse difficulty as integer
                    if (alchemyData?.ContainsKey("difficulty") == true)
                    {
                        var difficultyStr = alchemyData["difficulty"].ToString();
                        recipe.Difficulty = difficultyStr?.ToLower() switch
                        {
                            "novice" => 1,
                            "apprentice" => 3,
                            "journeyman" => 5,
                            "expert" => 7,
                            "master" => 10,
                            _ => 3
                        };
                    }
                    
                    // Try to parse ingredients array
                    if (alchemyData?.ContainsKey("ingredients") == true)
                    {
                        var ingredientsElement = alchemyData["ingredients"] as JsonElement?;
                        if (ingredientsElement?.ValueKind == JsonValueKind.Array)
                        {
                            recipe.Ingredients = ingredientsElement.Value.EnumerateArray()
                                .Select(item => new Ingredient 
                                { 
                                    Name = item.GetString() ?? "Unknown ingredient",
                                    Quantity = 1,
                                    Unit = "piece",
                                    Rarity = IngredientRarity.Common
                                })
                                .ToList();
                        }
                    }
                    
                    // Try to parse steps from instructions
                    if (alchemyData?.ContainsKey("instructions") == true)
                    {
                        var instructions = alchemyData["instructions"].ToString();
                        recipe.Steps = instructions?.Split('.', StringSplitOptions.RemoveEmptyEntries)
                            .Select(step => step.Trim())
                            .Where(step => !string.IsNullOrEmpty(step))
                            .ToList() ?? new List<string> { "Follow standard procedure" };
                    }
                    
                    return recipe;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not parse LLM alchemy generation response");
            }
            
            return new AlchemyRecipe { Name = "New Alchemy Recipe", Description = description };
        }

        private async Task<SpellBook> GenerateNewSpellBookAsync(World world, string description)
        {
            // Use LLM to generate detailed spell book
            var prompt = $@"Create a detailed spell book for the world '{world.Name}' based on this description: {description}
            
            World Context:
            - Genre: {world.WorldInfo.Genre}
            - Magic Level: {world.WorldInfo.MagicLevel}
            - Tech Level: {world.WorldInfo.TechnologyLevel}
            
            Generate a JSON object with these fields:
            {{
                ""name"": ""Spell Book Name"",
                ""description"": ""Detailed description of the spell book's appearance and origin"",
                ""author"": ""Author Name"",
                ""language"": ""Language the book is written in"",
                ""requiredLevel"": ""1-20"",
                ""spells"": [
                    {{
                        ""name"": ""Spell Name"",
                        ""description"": ""What the spell does"",
                        ""level"": ""1-9"",
                        ""components"": ""V, S, M (material components)"",
                        ""castingTime"": ""1 action"",
                        ""range"": ""30 feet"",
                        ""duration"": ""1 hour""
                    }}
                ]
            }}";

            var llmResponse = await _llmService.GenerateTextFromJsonAsync(world, prompt);
            
            try
            {
                var jsonMatch = Regex.Match(llmResponse, @"\{.*\}", RegexOptions.Singleline);
                if (jsonMatch.Success)
                {
                    var spellBookData = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonMatch.Value);
                    var spellBook = new SpellBook 
                    { 
                        Name = spellBookData?.GetValueOrDefault("name")?.ToString() ?? "New Spell Book",
                        Description = spellBookData?.GetValueOrDefault("description")?.ToString() ?? description,
                        Language = spellBookData?.GetValueOrDefault("language")?.ToString() ?? "Common"
                    };
                    
                    // Try to parse required level
                    if (spellBookData?.ContainsKey("requiredLevel") == true && 
                        int.TryParse(spellBookData["requiredLevel"].ToString(), out var requiredLevel))
                    {
                        spellBook.RequiredLevel = Math.Clamp(requiredLevel, 1, 20);
                    }
                    
                    return spellBook;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not parse LLM spell book generation response");
            }
            
            return new SpellBook { Name = "New Spell Book", Description = description };
        }

        // Additional helper methods for regeneration and property updates...
        private async Task RegeneratePlaceAsync(World world, string placeId, string userComment)
        {
            var place = world.Places.FirstOrDefault(p => p.Id == placeId);
            if (place != null)
            {
                var newDescription = await _llmService.GenerateLocationDescriptionAsync(new { place, userComment });
                place.Description = newDescription;
            }
        }

        private async Task RegenerateCharacterAsync(World world, string characterId, string userComment)
        {
            var character = world.HistoricFigures.FirstOrDefault(c => c.Id == characterId);
            if (character != null)
            {
                var newDescription = await _llmService.GenerateCharacterDescriptionAsync(new { character, userComment });
                character.Description = newDescription;
            }
        }

        private async Task RegenerateEventAsync(World world, string eventId, string userComment)
        {
            var worldEvent = world.WorldEvents.FirstOrDefault(e => e.Id == eventId);
            if (worldEvent != null)
            {
                var newDescription = await _llmService.GenerateEventNarrativeAsync(new { worldEvent, userComment });
                worldEvent.Description = newDescription;
            }
        }

        private async Task RegenerateTechnologyAsync(World world, string techId, string userComment)
        {
            var tech = world.TechnicalSpecs.FirstOrDefault(t => t.Id == techId);
            if (tech != null)
            {
                var newDescription = await _llmService.GenerateTextFromJsonAsync(new { tech, userComment }, 
                    "Generate a detailed technical description based on the user comment");
                tech.Description = newDescription;
            }
        }

        private async Task RegenerateMagicAsync(World world, string magicId, string userComment)
        {
            var rune = world.RunesOfPower.FirstOrDefault(r => r.Id == magicId);
            if (rune != null)
            {
                var newDescription = await _llmService.GenerateTextFromJsonAsync(new { rune, userComment }, 
                    "Generate a detailed magical description based on the user comment");
                rune.Description = newDescription;
            }
        }

        private async Task UpdatePlacePropertiesAsync(Place place, WorldUpdateInstruction instruction)
        {
            // Generate an enhanced description using LLM if description is being updated
            if (instruction.Properties.ContainsKey("description"))
            {
                var currentDescription = place.Description;
                var newDescription = instruction.Properties["description"].ToString();
                
                var prompt = $"Enhanced the description of the place '{place.Name}' (Type: {place.Type}). " +
                           $"Current description: '{currentDescription}'. " +
                           $"User wants to update it to: '{newDescription}'. " +
                           $"Please provide a rich, detailed description that incorporates the user's request while maintaining consistency with the place's type and existing characteristics.";
                
                var enhancedDescription = await _llmService.GenerateLocationDescriptionAsync(place);
                place.Description = enhancedDescription;
            }
            
            // Apply other property changes
            foreach (var prop in instruction.Properties)
            {
                switch (prop.Key.ToLower())
                {
                    case "description":
                        // Already handled above with LLM enhancement
                        break;
                    case "type":
                        if (Enum.TryParse<PlaceType>(prop.Value.ToString(), out var placeType))
                            place.Type = placeType;
                        break;
                    case "population":
                        if (int.TryParse(prop.Value.ToString(), out int pop))
                            place.Population.TotalCount = pop;
                        break;
                }
            }
        }

        private async Task ApplyPropertyChangesAsync(World world, string llmResponse, string userComment)
        {
            try
            {
                // Extract JSON from LLM response
                var jsonMatch = Regex.Match(llmResponse, @"\{.*\}", RegexOptions.Singleline);
                if (jsonMatch.Success)
                {
                    var jsonStr = jsonMatch.Value;
                    var changes = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonStr);
                    
                    if (changes != null)
                    {
                        foreach (var change in changes)
                        {
                            switch (change.Key.ToLower())
                            {
                                case "description":
                                    // Use LLM to enhance the description based on user comment
                                    var enhancedDescription = await _llmService.GenerateTextFromJsonAsync(
                                        world, 
                                        $"Update the world description based on this user comment: '{userComment}'. " +
                                        $"Current description: '{world.Description}'. " +
                                        $"Suggested change: '{change.Value}'. " +
                                        $"Provide a rich, detailed description that incorporates the user's feedback.");
                                    world.Description = enhancedDescription;
                                    break;
                                case "genre":
                                    world.WorldInfo.Genre = change.Value.ToString() ?? world.WorldInfo.Genre;
                                    break;
                                case "technologylevel":
                                    world.WorldInfo.TechnologyLevel = change.Value.ToString() ?? world.WorldInfo.TechnologyLevel;
                                    break;
                                case "magiclevel":
                                    world.WorldInfo.MagicLevel = change.Value.ToString() ?? world.WorldInfo.MagicLevel;
                                    break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not parse property changes from LLM response");
            }
        }
    }
}
