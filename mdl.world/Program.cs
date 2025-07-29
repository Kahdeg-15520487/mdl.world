using mdl.world.Services;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace mdl.world
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            
            // Add CORS
            builder.Services.AddCors();
            
            // Add health checks
            builder.Services.AddHealthChecks()
                .AddCheck<LLMServiceHealthCheck>("llm_service");
            
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

            // Configure default files (serve index.html when accessing root)
            app.UseDefaultFiles();
            
            // Enable static files (for the frontend)
            app.UseStaticFiles();
            
            // Enable CORS for frontend access
            app.UseCors(policy => policy
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseAuthorization();

            // Add health check endpoints
            app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
            {
                ResponseWriter = async (context, report) =>
                {
                    context.Response.ContentType = "application/json";
                    
                    var response = new
                    {
                        status = report.Status.ToString(),
                        checks = report.Entries.Select(x => new
                        {
                            name = x.Key,
                            status = x.Value.Status.ToString(),
                            description = x.Value.Description,
                            data = x.Value.Data,
                            duration = x.Value.Duration.TotalMilliseconds
                        })
                    };
                    
                    await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response, new System.Text.Json.JsonSerializerOptions
                    {
                        WriteIndented = true
                    }));
                }
            });

            app.MapControllers();

            app.Run();
        }
    }
}
