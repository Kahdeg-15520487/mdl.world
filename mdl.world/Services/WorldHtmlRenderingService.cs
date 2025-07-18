using System.Text;
using mdl.worlddata.Core;
using mdl.worlddata.Geography;
using mdl.worlddata.Characters;
using mdl.worlddata.Items;
using mdl.worlddata.Magic;
using mdl.worlddata.Events;

namespace mdl.world.Services
{
    public class WorldHtmlRenderingService : IWorldHtmlRenderingService
    {
        private readonly ILogger<WorldHtmlRenderingService> _logger;

        public WorldHtmlRenderingService(ILogger<WorldHtmlRenderingService> logger)
        {
            _logger = logger;
        }

        public Task<string> RenderWorldAsHtmlAsync(World world)
        {
            var html = new StringBuilder();
            
            // Wikipedia-style HTML structure
            html.AppendLine("<!DOCTYPE html>");
            html.AppendLine("<html>");
            html.AppendLine("<head>");
            html.AppendLine($"<title>{world.Name} - World Wiki</title>");
            html.AppendLine(GetWikiStyles());
            html.AppendLine("</head>");
            html.AppendLine("<body>");
            
            // Header
            html.AppendLine("<div class=\"wiki-header\">");
            html.AppendLine($"<h1 class=\"wiki-title\">{world.Name}</h1>");
            html.AppendLine("<div class=\"wiki-subtitle\">From the World Archives</div>");
            html.AppendLine("</div>");
            
            // Navigation
            html.AppendLine("<div class=\"wiki-nav\">");
            html.AppendLine("<ul>");
            html.AppendLine($"<li><a href=\"#overview\">Overview</a></li>");
            html.AppendLine($"<li><a href=\"#places\">Places</a></li>");
            html.AppendLine($"<li><a href=\"#characters\">Characters</a></li>");
            html.AppendLine($"<li><a href=\"#items\">Items & Artifacts</a></li>");
            html.AppendLine($"<li><a href=\"#magic\">Magic & Spells</a></li>");
            html.AppendLine($"<li><a href=\"#events\">Historical Events</a></li>");
            html.AppendLine("</ul>");
            html.AppendLine("</div>");
            
            // Main content
            html.AppendLine("<div class=\"wiki-content\">");
            
            // Overview section
            html.AppendLine("<div class=\"wiki-section\" id=\"overview\">");
            html.AppendLine("<h2>Overview</h2>");
            html.AppendLine("<div class=\"wiki-infobox\">");
            html.AppendLine("<h3>World Information</h3>");
            html.AppendLine($"<div><strong>Name:</strong> {world.Name}</div>");
            html.AppendLine($"<div><strong>Created:</strong> {world.CreationDate:yyyy-MM-dd}</div>");
            html.AppendLine($"<div><strong>Genre:</strong> {world.WorldInfo.Genre}</div>");
            html.AppendLine($"<div><strong>Era:</strong> {world.WorldInfo.TimeEra}</div>");
            html.AppendLine($"<div><strong>Magic Level:</strong> {world.WorldInfo.MagicLevel}</div>");
            html.AppendLine($"<div><strong>Tech Level:</strong> {world.WorldInfo.TechnologyLevel}</div>");
            html.AppendLine("</div>");
            html.AppendLine($"<p>{world.Description}</p>");
            html.AppendLine("</div>");
            
            // Places section
            html.AppendLine("<div class=\"wiki-section\" id=\"places\">");
            html.AppendLine("<h2>Places</h2>");
            if (world.Places.Any())
            {
                html.AppendLine("<div class=\"wiki-grid\">");
                foreach (var place in world.Places.Take(20)) // Show first 20 places
                {
                    html.AppendLine("<div class=\"wiki-card\">");
                    html.AppendLine($"<h4><a href=\"/world/{world.Id}/wiki/place/{place.Id}\">{place.Name}</a></h4>");
                    html.AppendLine($"<p><em>{place.Type}</em></p>");
                    html.AppendLine($"<p>{TruncateText(place.Description, 100)}</p>");
                    html.AppendLine("</div>");
                }
                html.AppendLine("</div>");
            }
            else
            {
                html.AppendLine("<p>No places have been discovered yet.</p>");
            }
            html.AppendLine("</div>");
            
            // Characters section
            html.AppendLine("<div class=\"wiki-section\" id=\"characters\">");
            html.AppendLine("<h2>Notable Characters</h2>");
            if (world.HistoricFigures.Any())
            {
                html.AppendLine("<div class=\"wiki-grid\">");
                foreach (var character in world.HistoricFigures.Take(15))
                {
                    html.AppendLine("<div class=\"wiki-card\">");
                    html.AppendLine($"<h4><a href=\"/world/{world.Id}/wiki/character/{character.Id}\">{character.Name}</a></h4>");
                    html.AppendLine($"<p><em>{character.Title}</em></p>");
                    html.AppendLine($"<p><strong>Race:</strong> {character.Race} | <strong>Class:</strong> {character.Class}</p>");
                    html.AppendLine($"<p>{TruncateText(character.Description, 100)}</p>");
                    html.AppendLine("</div>");
                }
                html.AppendLine("</div>");
            }
            else
            {
                html.AppendLine("<p>No notable characters have been recorded.</p>");
            }
            html.AppendLine("</div>");
            
            // Items section
            html.AppendLine("<div class=\"wiki-section\" id=\"items\">");
            html.AppendLine("<h2>Items & Artifacts</h2>");
            if (world.Equipment.Any())
            {
                html.AppendLine("<div class=\"wiki-grid\">");
                foreach (var item in world.Equipment.Take(12))
                {
                    html.AppendLine("<div class=\"wiki-card\">");
                    html.AppendLine($"<h4><a href=\"/world/{world.Id}/wiki/item/{item.Id}\">{item.Name}</a></h4>");
                    html.AppendLine($"<p><em>{item.Type} - {item.Rarity}</em></p>");
                    html.AppendLine($"<p>{TruncateText(item.Description, 100)}</p>");
                    html.AppendLine("</div>");
                }
                html.AppendLine("</div>");
            }
            else
            {
                html.AppendLine("<p>No notable items have been catalogued.</p>");
            }
            html.AppendLine("</div>");
            
            // Magic section
            html.AppendLine("<div class=\"wiki-section\" id=\"magic\">");
            html.AppendLine("<h2>Magic & Spells</h2>");
            if (world.SpellBooks.Any() || world.RunesOfPower.Any() || world.AlchemyRecipes.Any())
            {
                html.AppendLine("<div class=\"wiki-subsection\">");
                html.AppendLine("<h3>Spell Books</h3>");
                html.AppendLine("<ul>");
                foreach (var spellBook in world.SpellBooks.Take(10))
                {
                    html.AppendLine($"<li><a href=\"/world/{world.Id}/wiki/spell/{spellBook.Id}\">{spellBook.Name}</a> - {spellBook.Description}</li>");
                }
                html.AppendLine("</ul>");
                html.AppendLine("</div>");
                
                html.AppendLine("<div class=\"wiki-subsection\">");
                html.AppendLine("<h3>Runes of Power</h3>");
                html.AppendLine("<ul>");
                foreach (var rune in world.RunesOfPower.Take(10))
                {
                    html.AppendLine($"<li><strong>{rune.Name}</strong> - {rune.Description}</li>");
                }
                html.AppendLine("</ul>");
                html.AppendLine("</div>");
                
                html.AppendLine("<div class=\"wiki-subsection\">");
                html.AppendLine("<h3>Alchemy Recipes</h3>");
                html.AppendLine("<ul>");
                foreach (var recipe in world.AlchemyRecipes.Take(10))
                {
                    html.AppendLine($"<li><strong>{recipe.Name}</strong> - {recipe.Description}</li>");
                }
                html.AppendLine("</ul>");
                html.AppendLine("</div>");
            }
            else
            {
                html.AppendLine("<p>No magical knowledge has been documented.</p>");
            }
            html.AppendLine("</div>");
            
            // Events section
            html.AppendLine("<div class=\"wiki-section\" id=\"events\">");
            html.AppendLine("<h2>Historical Events</h2>");
            if (world.WorldEvents.Any())
            {
                html.AppendLine("<div class=\"wiki-timeline\">");
                foreach (var evt in world.WorldEvents.Take(10))
                {
                    html.AppendLine("<div class=\"wiki-event\">");
                    html.AppendLine($"<h4><a href=\"/world/{world.Id}/wiki/event/{evt.Id}\">{evt.Name}</a></h4>");
                    html.AppendLine($"<p class=\"wiki-event-date\">{evt.StartDate:yyyy}</p>");
                    html.AppendLine($"<p>{evt.Description}</p>");
                    html.AppendLine("</div>");
                }
                html.AppendLine("</div>");
            }
            else
            {
                html.AppendLine("<p>No historical events have been recorded.</p>");
            }
            html.AppendLine("</div>");
            
            html.AppendLine("</div>"); // End wiki-content
            
            // Footer
            html.AppendLine("<div class=\"wiki-footer\">");
            html.AppendLine($"<p>Generated on {DateTime.Now:yyyy-MM-dd HH:mm} | World ID: {world.Id}</p>");
            html.AppendLine("</div>");
            
            html.AppendLine("</body>");
            html.AppendLine("</html>");
            
            return Task.FromResult(html.ToString());
        }

        public Task<string> RenderPlaceAsHtmlAsync(World world, string placeId)
        {
            var place = world.Places.FirstOrDefault(p => p.Id == placeId);
            if (place == null)
            {
                return Task.FromResult(GenerateNotFoundPage("Place", placeId));
            }

            var html = new StringBuilder();
            
            html.AppendLine("<!DOCTYPE html>");
            html.AppendLine("<html>");
            html.AppendLine("<head>");
            html.AppendLine($"<title>{place.Name} - {world.Name} Wiki</title>");
            html.AppendLine(GetWikiStyles());
            html.AppendLine("</head>");
            html.AppendLine("<body>");
            
            // Header with breadcrumb
            html.AppendLine("<div class=\"wiki-header\">");
            html.AppendLine($"<div class=\"wiki-breadcrumb\"><a href=\"/world/{world.Id}/wiki\">{world.Name}</a> > <a href=\"/world/{world.Id}/wiki#places\">Places</a> > {place.Name}</div>");
            html.AppendLine($"<h1 class=\"wiki-title\">{place.Name}</h1>");
            html.AppendLine($"<div class=\"wiki-subtitle\">{place.Type}</div>");
            html.AppendLine("</div>");
            
            html.AppendLine("<div class=\"wiki-content\">");
            
            // Place info box
            html.AppendLine("<div class=\"wiki-infobox\">");
            html.AppendLine($"<h3>{place.Name}</h3>");
            html.AppendLine($"<div><strong>Type:</strong> {place.Type}</div>");
            html.AppendLine($"<div><strong>Climate:</strong> {place.Geography.Climate}</div>");
            html.AppendLine($"<div><strong>Terrain:</strong> {place.Geography.Terrain}</div>");
            html.AppendLine($"<div><strong>Population:</strong> {place.Population.TotalCount:N0}</div>");
            if (place.Geography.Area > 0)
            {
                html.AppendLine($"<div><strong>Area:</strong> {place.Geography.Area:N0} km²</div>");
            }
            html.AppendLine("</div>");
            
            // Description
            html.AppendLine($"<p>{place.Description}</p>");
            
            // Notable Features
            if (place.NotableFeatures.Any())
            {
                html.AppendLine("<h3>Notable Features</h3>");
                html.AppendLine("<ul>");
                foreach (var feature in place.NotableFeatures)
                {
                    html.AppendLine($"<li>{feature}</li>");
                }
                html.AppendLine("</ul>");
            }
            
            // Related Characters
            var relatedCharacters = world.HistoricFigures.Where(c => 
                c.BirthPlaceId == place.Id || c.AssociatedPlaceIds.Contains(place.Id)).ToList();
            
            if (relatedCharacters.Any())
            {
                html.AppendLine("<h3>Notable People</h3>");
                html.AppendLine("<ul>");
                foreach (var character in relatedCharacters)
                {
                    html.AppendLine($"<li><a href=\"/world/{world.Id}/wiki/character/{character.Id}\">{character.Name}</a> - {character.Title}</li>");
                }
                html.AppendLine("</ul>");
            }
            
            // Child Places
            var childPlaces = world.Places.Where(p => p.ParentPlaceId == place.Id).ToList();
            if (childPlaces.Any())
            {
                html.AppendLine("<h3>Sub-locations</h3>");
                html.AppendLine("<ul>");
                foreach (var childPlace in childPlaces)
                {
                    html.AppendLine($"<li><a href=\"/world/{world.Id}/wiki/place/{childPlace.Id}\">{childPlace.Name}</a> ({childPlace.Type})</li>");
                }
                html.AppendLine("</ul>");
            }
            
            // Parent Place
            if (!string.IsNullOrEmpty(place.ParentPlaceId))
            {
                var parentPlace = world.Places.FirstOrDefault(p => p.Id == place.ParentPlaceId);
                if (parentPlace != null)
                {
                    html.AppendLine("<h3>Part of</h3>");
                    html.AppendLine($"<p><a href=\"/world/{world.Id}/wiki/place/{parentPlace.Id}\">{parentPlace.Name}</a></p>");
                }
            }
            
            html.AppendLine("</div>");
            
            // Footer
            html.AppendLine("<div class=\"wiki-footer\">");
            html.AppendLine($"<p><a href=\"/world/{world.Id}/wiki\">← Back to {world.Name}</a></p>");
            html.AppendLine("</div>");
            
            html.AppendLine("</body>");
            html.AppendLine("</html>");
            
            return Task.FromResult(html.ToString());
        }

        public Task<string> RenderCharacterAsHtmlAsync(World world, string characterId)
        {
            var character = world.HistoricFigures.FirstOrDefault(c => c.Id == characterId);
            if (character == null)
            {
                return Task.FromResult(GenerateNotFoundPage("Character", characterId));
            }

            var html = new StringBuilder();
            
            html.AppendLine("<!DOCTYPE html>");
            html.AppendLine("<html>");
            html.AppendLine("<head>");
            html.AppendLine($"<title>{character.Name} - {world.Name} Wiki</title>");
            html.AppendLine(GetWikiStyles());
            html.AppendLine("</head>");
            html.AppendLine("<body>");
            
            // Header with breadcrumb
            html.AppendLine("<div class=\"wiki-header\">");
            html.AppendLine($"<div class=\"wiki-breadcrumb\"><a href=\"/world/{world.Id}/wiki\">{world.Name}</a> > <a href=\"/world/{world.Id}/wiki#characters\">Characters</a> > {character.Name}</div>");
            html.AppendLine($"<h1 class=\"wiki-title\">{character.Name}</h1>");
            html.AppendLine($"<div class=\"wiki-subtitle\">{character.Title}</div>");
            html.AppendLine("</div>");
            
            html.AppendLine("<div class=\"wiki-content\">");
            
            // Character info box
            html.AppendLine("<div class=\"wiki-infobox\">");
            html.AppendLine($"<h3>{character.Name}</h3>");
            html.AppendLine($"<div><strong>Title:</strong> {character.Title}</div>");
            html.AppendLine($"<div><strong>Race:</strong> {character.Race}</div>");
            html.AppendLine($"<div><strong>Class:</strong> {character.Class}</div>");
            html.AppendLine($"<div><strong>Status:</strong> {(character.IsAlive ? "Alive" : "Deceased")}</div>");
            
            if (character.BirthDate.HasValue)
            {
                html.AppendLine($"<div><strong>Born:</strong> {character.BirthDate.Value:yyyy}</div>");
            }
            if (character.DeathDate.HasValue)
            {
                html.AppendLine($"<div><strong>Died:</strong> {character.DeathDate.Value:yyyy}</div>");
            }
            
            // Birth place
            if (!string.IsNullOrEmpty(character.BirthPlaceId))
            {
                var birthPlace = world.Places.FirstOrDefault(p => p.Id == character.BirthPlaceId);
                if (birthPlace != null)
                {
                    html.AppendLine($"<div><strong>Birthplace:</strong> <a href=\"/world/{world.Id}/wiki/place/{birthPlace.Id}\">{birthPlace.Name}</a></div>");
                }
            }
            
            html.AppendLine("</div>");
            
            // Description
            html.AppendLine($"<p>{character.Description}</p>");
            
            // Achievements
            if (character.Achievements.Any())
            {
                html.AppendLine("<h3>Achievements</h3>");
                html.AppendLine("<ul>");
                foreach (var achievement in character.Achievements)
                {
                    html.AppendLine($"<li>{achievement}</li>");
                }
                html.AppendLine("</ul>");
            }
            
            // Associated Places
            if (character.AssociatedPlaceIds.Any())
            {
                html.AppendLine("<h3>Associated Places</h3>");
                html.AppendLine("<ul>");
                foreach (var placeId in character.AssociatedPlaceIds)
                {
                    var place = world.Places.FirstOrDefault(p => p.Id == placeId);
                    if (place != null)
                    {
                        html.AppendLine($"<li><a href=\"/world/{world.Id}/wiki/place/{place.Id}\">{place.Name}</a> ({place.Type})</li>");
                    }
                }
                html.AppendLine("</ul>");
            }
            
            // Attributes
            if (character.Attributes.Any())
            {
                html.AppendLine("<h3>Attributes</h3>");
                html.AppendLine("<div class=\"wiki-attributes\">");
                foreach (var attr in character.Attributes)
                {
                    html.AppendLine($"<div class=\"attribute\"><strong>{attr.Key}:</strong> {attr.Value}</div>");
                }
                html.AppendLine("</div>");
            }
            
            // Equipment owned
            var ownedEquipment = world.Equipment.Where(e => e.CurrentOwnerId == character.Id).ToList();
            if (ownedEquipment.Any())
            {
                html.AppendLine("<h3>Equipment</h3>");
                html.AppendLine("<ul>");
                foreach (var item in ownedEquipment)
                {
                    html.AppendLine($"<li><a href=\"/world/{world.Id}/wiki/item/{item.Id}\">{item.Name}</a> ({item.Type})</li>");
                }
                html.AppendLine("</ul>");
            }
            
            html.AppendLine("</div>");
            
            // Footer
            html.AppendLine("<div class=\"wiki-footer\">");
            html.AppendLine($"<p><a href=\"/world/{world.Id}/wiki\">← Back to {world.Name}</a></p>");
            html.AppendLine("</div>");
            
            html.AppendLine("</body>");
            html.AppendLine("</html>");
            
            return Task.FromResult(html.ToString());
        }

        public Task<string> RenderItemAsHtmlAsync(World world, string itemId)
        {
            var item = world.Equipment.FirstOrDefault(e => e.Id == itemId);
            if (item == null)
            {
                return Task.FromResult(GenerateNotFoundPage("Item", itemId));
            }

            var html = new StringBuilder();
            
            html.AppendLine("<!DOCTYPE html>");
            html.AppendLine("<html>");
            html.AppendLine("<head>");
            html.AppendLine($"<title>{item.Name} - {world.Name} Wiki</title>");
            html.AppendLine(GetWikiStyles());
            html.AppendLine("</head>");
            html.AppendLine("<body>");
            
            // Header with breadcrumb
            html.AppendLine("<div class=\"wiki-header\">");
            html.AppendLine($"<div class=\"wiki-breadcrumb\"><a href=\"/world/{world.Id}/wiki\">{world.Name}</a> > <a href=\"/world/{world.Id}/wiki#items\">Items</a> > {item.Name}</div>");
            html.AppendLine($"<h1 class=\"wiki-title\">{item.Name}</h1>");
            html.AppendLine($"<div class=\"wiki-subtitle\">{item.Type}</div>");
            html.AppendLine("</div>");
            
            html.AppendLine("<div class=\"wiki-content\">");
            
            // Item info box
            html.AppendLine("<div class=\"wiki-infobox\">");
            html.AppendLine($"<h3>{item.Name}</h3>");
            html.AppendLine($"<div><strong>Type:</strong> {item.Type}</div>");
            html.AppendLine($"<div><strong>Rarity:</strong> {item.Rarity}</div>");
            html.AppendLine($"<div><strong>Material:</strong> {item.Material}</div>");
            html.AppendLine($"<div><strong>Condition:</strong> {item.Condition}</div>");
            html.AppendLine($"<div><strong>Weight:</strong> {item.Weight:F2} kg</div>");
            html.AppendLine($"<div><strong>Value:</strong> {item.Value:C}</div>");
            
            // Creator
            if (!string.IsNullOrEmpty(item.CreatorId))
            {
                var creator = world.HistoricFigures.FirstOrDefault(c => c.Id == item.CreatorId);
                if (creator != null)
                {
                    html.AppendLine($"<div><strong>Creator:</strong> <a href=\"/world/{world.Id}/wiki/character/{creator.Id}\">{creator.Name}</a></div>");
                }
            }
            
            // Current Owner
            if (!string.IsNullOrEmpty(item.CurrentOwnerId))
            {
                var owner = world.HistoricFigures.FirstOrDefault(c => c.Id == item.CurrentOwnerId);
                if (owner != null)
                {
                    html.AppendLine($"<div><strong>Current Owner:</strong> <a href=\"/world/{world.Id}/wiki/character/{owner.Id}\">{owner.Name}</a></div>");
                }
            }
            
            html.AppendLine("</div>");
            
            // Description
            html.AppendLine($"<p>{item.Description}</p>");
            
            // Properties
            if (item.Properties.Any())
            {
                html.AppendLine("<h3>Properties</h3>");
                html.AppendLine("<div class=\"wiki-properties\">");
                foreach (var prop in item.Properties)
                {
                    html.AppendLine($"<div class=\"property\"><strong>{prop.Key}:</strong> {prop.Value}</div>");
                }
                html.AppendLine("</div>");
            }
            
            // Weapon-specific details
            if (item is Weapon weapon)
            {
                html.AppendLine("<h3>Combat Statistics</h3>");
                html.AppendLine($"<div><strong>Damage:</strong> {weapon.Damage}</div>");
                html.AppendLine($"<div><strong>Range:</strong> {weapon.Range}</div>");
                html.AppendLine($"<div><strong>Damage Type:</strong> {weapon.DamageType}</div>");
                html.AppendLine($"<div><strong>Magical:</strong> {(weapon.IsMagical ? "Yes" : "No")}</div>");
            }
            
            // History
            if (item.History.Any())
            {
                html.AppendLine("<h3>History</h3>");
                html.AppendLine("<ul>");
                foreach (var historyEntry in item.History)
                {
                    html.AppendLine($"<li>{historyEntry}</li>");
                }
                html.AppendLine("</ul>");
            }
            
            html.AppendLine("</div>");
            
            // Footer
            html.AppendLine("<div class=\"wiki-footer\">");
            html.AppendLine($"<p><a href=\"/world/{world.Id}/wiki\">← Back to {world.Name}</a></p>");
            html.AppendLine("</div>");
            
            html.AppendLine("</body>");
            html.AppendLine("</html>");
            
            return Task.FromResult(html.ToString());
        }

        public Task<string> RenderSpellAsHtmlAsync(World world, string spellId)
        {
            var spell = world.SpellBooks.FirstOrDefault(s => s.Id == spellId);
            if (spell == null)
            {
                return Task.FromResult(GenerateNotFoundPage("Spell", spellId));
            }

            var html = new StringBuilder();
            
            html.AppendLine("<!DOCTYPE html>");
            html.AppendLine("<html>");
            html.AppendLine("<head>");
            html.AppendLine($"<title>{spell.Name} - {world.Name} Wiki</title>");
            html.AppendLine(GetWikiStyles());
            html.AppendLine("</head>");
            html.AppendLine("<body>");
            
            // Header with breadcrumb
            html.AppendLine("<div class=\"wiki-header\">");
            html.AppendLine($"<div class=\"wiki-breadcrumb\"><a href=\"/world/{world.Id}/wiki\">{world.Name}</a> > <a href=\"/world/{world.Id}/wiki#magic\">Magic</a> > {spell.Name}</div>");
            html.AppendLine($"<h1 class=\"wiki-title\">{spell.Name}</h1>");
            html.AppendLine($"<div class=\"wiki-subtitle\">Spell Book</div>");
            html.AppendLine("</div>");
            
            html.AppendLine("<div class=\"wiki-content\">");
            
            // Spell info box
            html.AppendLine("<div class=\"wiki-infobox\">");
            html.AppendLine($"<h3>{spell.Name}</h3>");
            html.AppendLine($"<div><strong>Type:</strong> Spell Book</div>");
            html.AppendLine($"<div><strong>School:</strong> {spell.MagicSchool}</div>");
            html.AppendLine($"<div><strong>Required Level:</strong> {spell.RequiredLevel}</div>");
            html.AppendLine($"<div><strong>Spell Count:</strong> {spell.Spells.Count}</div>");
            html.AppendLine($"<div><strong>Complete:</strong> {(spell.IsComplete ? "Yes" : "No")}</div>");
            html.AppendLine("</div>");
            
            // Description
            html.AppendLine($"<p>{spell.Description}</p>");
            
            // Spells contained
            if (spell.Spells.Any())
            {
                html.AppendLine("<h3>Contained Spells</h3>");
                html.AppendLine("<ul>");
                foreach (var spellEntry in spell.Spells)
                {
                    html.AppendLine($"<li>{spellEntry.Name} - {spellEntry.Description}</li>");
                }
                html.AppendLine("</ul>");
            }
            
            html.AppendLine("</div>");
            
            // Footer
            html.AppendLine("<div class=\"wiki-footer\">");
            html.AppendLine($"<p><a href=\"/world/{world.Id}/wiki\">← Back to {world.Name}</a></p>");
            html.AppendLine("</div>");
            
            html.AppendLine("</body>");
            html.AppendLine("</html>");
            
            return Task.FromResult(html.ToString());
        }

        public Task<string> RenderEventAsHtmlAsync(World world, string eventId)
        {
            var evt = world.WorldEvents.FirstOrDefault(e => e.Id == eventId);
            if (evt == null)
            {
                return Task.FromResult(GenerateNotFoundPage("Event", eventId));
            }

            var html = new StringBuilder();
            
            html.AppendLine("<!DOCTYPE html>");
            html.AppendLine("<html>");
            html.AppendLine("<head>");
            html.AppendLine($"<title>{evt.Name} - {world.Name} Wiki</title>");
            html.AppendLine(GetWikiStyles());
            html.AppendLine("</head>");
            html.AppendLine("<body>");
            
            // Header with breadcrumb
            html.AppendLine("<div class=\"wiki-header\">");
            html.AppendLine($"<div class=\"wiki-breadcrumb\"><a href=\"/world/{world.Id}/wiki\">{world.Name}</a> > <a href=\"/world/{world.Id}/wiki#events\">Events</a> > {evt.Name}</div>");
            html.AppendLine($"<h1 class=\"wiki-title\">{evt.Name}</h1>");
            html.AppendLine($"<div class=\"wiki-subtitle\">Historical Event</div>");
            html.AppendLine("</div>");
            
            html.AppendLine("<div class=\"wiki-content\">");
            
            // Event info box
            html.AppendLine("<div class=\"wiki-infobox\">");
            html.AppendLine($"<h3>{evt.Name}</h3>");
            html.AppendLine($"<div><strong>Start Date:</strong> {evt.StartDate:yyyy-MM-dd}</div>");
            if (evt.EndDate.HasValue)
            {
                html.AppendLine($"<div><strong>End Date:</strong> {evt.EndDate.Value:yyyy-MM-dd}</div>");
            }
            html.AppendLine($"<div><strong>Type:</strong> {evt.Type}</div>");
            html.AppendLine($"<div><strong>Status:</strong> {evt.Status}</div>");
            html.AppendLine($"<div><strong>Global Impact:</strong> {evt.GlobalImpactLevel}/10</div>");
            html.AppendLine("</div>");
            
            // Description
            html.AppendLine($"<p>{evt.Description}</p>");
            
            // Participants
            if (evt.ParticipantIds.Any())
            {
                html.AppendLine("<h3>Participants</h3>");
                html.AppendLine("<ul>");
                foreach (var participantId in evt.ParticipantIds)
                {
                    var participant = world.HistoricFigures.FirstOrDefault(c => c.Id == participantId);
                    if (participant != null)
                    {
                        html.AppendLine($"<li><a href=\"/world/{world.Id}/wiki/character/{participant.Id}\">{participant.Name}</a></li>");
                    }
                }
                html.AppendLine("</ul>");
            }
            
            // Consequences
            if (evt.Consequences.Any())
            {
                html.AppendLine("<h3>Consequences</h3>");
                html.AppendLine("<ul>");
                foreach (var consequence in evt.Consequences)
                {
                    html.AppendLine($"<li><strong>{consequence.Key}:</strong> {consequence.Value}</li>");
                }
                html.AppendLine("</ul>");
            }
            
            html.AppendLine("</div>");
            
            // Footer
            html.AppendLine("<div class=\"wiki-footer\">");
            html.AppendLine($"<p><a href=\"/world/{world.Id}/wiki\">← Back to {world.Name}</a></p>");
            html.AppendLine("</div>");
            
            html.AppendLine("</body>");
            html.AppendLine("</html>");
            
            return Task.FromResult(html.ToString());
        }

        private string GetWikiStyles()
        {
            return @"
<style>
    body {
        font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
        line-height: 1.6;
        margin: 0;
        padding: 0;
        background-color: #f8f9fa;
    }
    
    .wiki-header {
        background-color: #fff;
        border-bottom: 1px solid #a2a9b1;
        padding: 20px;
        margin-bottom: 20px;
    }
    
    .wiki-breadcrumb {
        font-size: 0.9em;
        color: #666;
        margin-bottom: 10px;
    }
    
    .wiki-breadcrumb a {
        color: #0645ad;
        text-decoration: none;
    }
    
    .wiki-breadcrumb a:hover {
        text-decoration: underline;
    }
    
    .wiki-title {
        margin: 0;
        font-size: 2.5em;
        color: #000;
        border-bottom: 3px solid #a2a9b1;
        padding-bottom: 5px;
    }
    
    .wiki-subtitle {
        font-size: 1.2em;
        color: #666;
        margin-top: 5px;
    }
    
    .wiki-nav {
        background-color: #fff;
        border: 1px solid #a2a9b1;
        margin: 0 20px 20px 20px;
        padding: 10px;
    }
    
    .wiki-nav ul {
        list-style: none;
        margin: 0;
        padding: 0;
        display: flex;
        flex-wrap: wrap;
    }
    
    .wiki-nav li {
        margin-right: 20px;
    }
    
    .wiki-nav a {
        color: #0645ad;
        text-decoration: none;
        font-weight: bold;
    }
    
    .wiki-nav a:hover {
        text-decoration: underline;
    }
    
    .wiki-content {
        max-width: 1200px;
        margin: 0 20px;
        background-color: #fff;
        padding: 20px;
        border: 1px solid #a2a9b1;
    }
    
    .wiki-section {
        margin-bottom: 40px;
    }
    
    .wiki-section h2 {
        border-bottom: 2px solid #a2a9b1;
        padding-bottom: 5px;
        color: #000;
    }
    
    .wiki-section h3 {
        color: #000;
        margin-top: 30px;
    }
    
    .wiki-infobox {
        float: right;
        width: 300px;
        border: 1px solid #a2a9b1;
        background-color: #f8f9fa;
        padding: 15px;
        margin: 0 0 20px 20px;
        font-size: 0.9em;
    }
    
    .wiki-infobox h3 {
        margin-top: 0;
        text-align: center;
        border-bottom: 1px solid #a2a9b1;
        padding-bottom: 5px;
    }
    
    .wiki-infobox div {
        margin-bottom: 8px;
    }
    
    .wiki-grid {
        display: grid;
        grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
        gap: 20px;
        margin-top: 20px;
    }
    
    .wiki-card {
        border: 1px solid #a2a9b1;
        padding: 15px;
        background-color: #f8f9fa;
    }
    
    .wiki-card h4 {
        margin-top: 0;
        color: #000;
    }
    
    .wiki-card h4 a {
        color: #0645ad;
        text-decoration: none;
    }
    
    .wiki-card h4 a:hover {
        text-decoration: underline;
    }
    
    .wiki-timeline {
        border-left: 3px solid #a2a9b1;
        padding-left: 20px;
    }
    
    .wiki-event {
        margin-bottom: 30px;
        position: relative;
    }
    
    .wiki-event::before {
        content: '●';
        position: absolute;
        left: -28px;
        color: #a2a9b1;
        font-size: 1.5em;
    }
    
    .wiki-event-date {
        font-weight: bold;
        color: #666;
        margin-bottom: 5px;
    }
    
    .wiki-attributes {
        display: grid;
        grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
        gap: 10px;
        margin-top: 15px;
    }
    
    .attribute {
        background-color: #f8f9fa;
        padding: 10px;
        border: 1px solid #a2a9b1;
    }
    
    .wiki-properties {
        display: grid;
        grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
        gap: 10px;
        margin-top: 15px;
    }
    
    .property {
        background-color: #f8f9fa;
        padding: 10px;
        border: 1px solid #a2a9b1;
    }
    
    .wiki-footer {
        background-color: #f8f9fa;
        border-top: 1px solid #a2a9b1;
        padding: 20px;
        margin-top: 40px;
        text-align: center;
        color: #666;
    }
    
    .wiki-footer a {
        color: #0645ad;
        text-decoration: none;
    }
    
    .wiki-footer a:hover {
        text-decoration: underline;
    }
    
    a {
        color: #0645ad;
        text-decoration: none;
    }
    
    a:hover {
        text-decoration: underline;
    }
    
    .wiki-subsection {
        margin-top: 30px;
    }
    
    .wiki-subsection h3 {
        border-bottom: 1px solid #a2a9b1;
        padding-bottom: 5px;
    }
    
    @media (max-width: 768px) {
        .wiki-content {
            margin: 0 10px;
        }
        
        .wiki-infobox {
            float: none;
            width: 100%;
            margin: 0 0 20px 0;
        }
        
        .wiki-nav ul {
            flex-direction: column;
        }
        
        .wiki-nav li {
            margin-right: 0;
            margin-bottom: 5px;
        }
    }
</style>";
        }

        private string GenerateNotFoundPage(string entityType, string entityId)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <title>{entityType} Not Found</title>
    {GetWikiStyles()}
</head>
<body>
    <div class=""wiki-header"">
        <h1 class=""wiki-title"">404 - {entityType} Not Found</h1>
    </div>
    <div class=""wiki-content"">
        <p>The {entityType.ToLower()} with ID '{entityId}' could not be found.</p>
        <p><a href=""javascript:history.back()"">← Go Back</a></p>
    </div>
</body>
</html>";
        }

        private string TruncateText(string text, int maxLength)
        {
            if (string.IsNullOrEmpty(text) || text.Length <= maxLength)
                return text;
            
            return text.Substring(0, maxLength) + "...";
        }
    }
}
