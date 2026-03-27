using System.ComponentModel.DataAnnotations;

namespace AI.Assistant.Infrastructure;

public class InfrastructureSettings
{
    public const string SectionName = "Infrastructure";
    [Required] public required string GeminiApiToken { get; set; }
    [Required] public required string GeminiModel { get; set; }
    [Required] public required string SupabaseUrl { get; set; }
    [Required] public required string SupabaseApiToken { get; set; }
    [Required] public required string TavilyApiKey { get; set; }
    [Required] public required string TavilySearchUrl { get; set; }
    [Required] public required string AssemblyAiApiKey { get; set; }
    [Required] public required string AssemblyAiApiUrl { get; set; }
    [Required] public required string AutoMapperLicenseKey { get; set; }
}