using mdl.worlddata.Core;

namespace mdl.world.Services
{
    public interface IWorldHtmlRenderingService
    {
        Task<string> RenderWorldAsHtmlAsync(World world);
        Task<string> RenderPlaceAsHtmlAsync(World world, string placeId);
        Task<string> RenderCharacterAsHtmlAsync(World world, string characterId);
        Task<string> RenderItemAsHtmlAsync(World world, string itemId);
        Task<string> RenderSpellAsHtmlAsync(World world, string spellId);
        Task<string> RenderEventAsHtmlAsync(World world, string eventId);
    }
}
