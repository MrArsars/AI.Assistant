namespace AI.Assistant.Core.Providers;

public static class DateTimeProvider
{
    public static string DateTimeNow => $"Поточний час: {DateTime.Now.ToLocalTime()}";
}