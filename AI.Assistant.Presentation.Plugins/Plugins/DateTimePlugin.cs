using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace AI.Assistant.Presentation.Plugins.Plugins;

public class DateTimePlugin
{
    [KernelFunction("get_datetime")]
    [Description("Returns the current date and time in dd.MM.yyyy HH:mm:ss format.")]
    public string GetDateTimeAsync()
    {
        return DateTime.Now.ToLocalTime().ToString("dd.MM.yyyy HH:mm:ss");
    }
}