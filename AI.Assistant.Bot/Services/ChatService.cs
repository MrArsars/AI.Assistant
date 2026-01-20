using System.Text;
using AI.Assistant.Bot.Models;
using Microsoft.SemanticKernel.ChatCompletion;
using Telegram.Bot.Types;

namespace AI.Assistant.Bot.Services;

public class ChatService(Supabase.Client supabase, int historyLimit) : IChatService
{
    //TODO:Review method
    public void TrimHistory(ChatHistory history)
    {
        // while (history.Count > historyLimit)
        // {
        //     if (history.Count > 1) history.RemoveRange(1, Math.Min(2, history.Count - 1));
        //     else break;
        // }
    }

    public async Task SaveMessageAsync(long chatId, AuthorRole role, string text)
    {
        var model = new MessageModel
        {
            ChatId = chatId,
            Role = role.ToString(),
            Text = text,
            CreatedAt = DateTime.Now
        };
        await supabase.From<MessageModel>().Insert(model);
    }

    public static string LoadSystemInstruction()
    {
        var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Prompts", "system_instruction.txt");

        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Промпт не знайдено за шляхом: {filePath}");

        return File.ReadAllText(filePath, Encoding.UTF8);
    }

    public async Task<ChatHistory> LoadHistoryAsync(long chatId)
    {
        var loadedHistory = new ChatHistory();
        var rows = await supabase.From<MessageModel>().Where(x => x.ChatId == chatId).Limit(historyLimit).Get();
        foreach (var msg in rows.Models)
        {
            if (msg.Role == AuthorRole.Assistant.Label)
            {
                loadedHistory.AddAssistantMessage(msg.Text);
                continue;
            }

            if (msg.Role == AuthorRole.User.Label)
                loadedHistory.AddUserMessage(msg.Text);
        }

        return loadedHistory;
    }

    public async Task SavePermanentAsync(long chatId, string info)
    {

        var memory = new PermanentMemoryModel()
        {
            ChatId = chatId,
            CreatedAt = DateTime.Now,
            Text = info,
        };
        
        await supabase.From<PermanentMemoryModel>().Insert(memory);
    }
}