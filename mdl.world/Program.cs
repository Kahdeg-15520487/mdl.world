using mdl.world.Services;

namespace mdl.world
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            
            // Register HTTP client for LLM service
            builder.Services.AddHttpClient<ILLMTextGenerationService, LLMTextGenerationService>();
            
            // Register world generation service
            builder.Services.AddScoped<IWorldGenerationService, WorldGenerationService>();
            
            // Register world storage service
            builder.Services.AddScoped<IWorldStorageService, JsonWorldStorageService>();
            
            // Register LLM text generation service
            builder.Services.AddScoped<ILLMTextGenerationService, LLMTextGenerationService>();
            
            // Register world enhancement service
            builder.Services.AddScoped<IWorldEnhancementService, WorldEnhancementService>();
            
            // Register HTML rendering service
            builder.Services.AddScoped<IWorldHtmlRenderingService, WorldHtmlRenderingService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
