using System.ComponentModel;

namespace AI.Assistant.Core.Models;

public enum MessageType
{
    [Description("Звичайне текстове повідомлення")]
    Text,

    [Description("Транскрибоване голосове повідомлення")]
    Voice
}