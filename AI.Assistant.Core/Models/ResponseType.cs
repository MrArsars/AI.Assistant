using System.ComponentModel;

namespace AI.Assistant.Core.Models;

public enum ResponseType
{
    [Description("Звичайна відповідь на запит користувача")]
    Default,

    [Description("Відкладене за проханням користувача повідомлення")]
    Proactive
}