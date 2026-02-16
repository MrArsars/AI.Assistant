using System.Text;

namespace AI.Assistant.Core.Prompts;

public static class Prompts
{
    public static string SystemPrompt { get; } = LoadSystemInstruction();
    public static string Introduction { get; } = LoadIntroduction();

    private static string LoadSystemInstruction()
    {
        var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Prompts", "system_instruction.txt");

        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Промпт не знайдено за шляхом: {filePath}");

        return File.ReadAllText(filePath, Encoding.UTF8);
    }

    private static string LoadIntroduction()
    {
        var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Prompts", "introduction.txt");

        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Промпт не знайдено за шляхом: {filePath}");

        return File.ReadAllText(filePath, Encoding.UTF8);
    }
}