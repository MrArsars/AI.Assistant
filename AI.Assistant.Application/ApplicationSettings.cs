using System.ComponentModel.DataAnnotations;

namespace AI.Assistant.Application;

public class ApplicationSettings
{
    public const string SectionName = "Application";
    [Required] public int HistoryMaxLimit { get; set; }
    [Required] public int HistoryMinLimit { get; set; }
}