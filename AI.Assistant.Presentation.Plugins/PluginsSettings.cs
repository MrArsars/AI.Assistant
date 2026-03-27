using System.ComponentModel.DataAnnotations;

namespace AI.Assistant.Presentation.Plugins;

public class PluginsSettings
{
    public const string SectionName = "Plugins";
    [Required] public required string GeminiApiToken { get; set; }
    [Required] public required string GeminiModel { get; set; }
    [Required] public required string EmbeddingModel { get; set; }
}