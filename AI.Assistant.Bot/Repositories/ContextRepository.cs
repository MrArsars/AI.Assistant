using AI.Assistant.Bot.Models;
using AI.Assistant.Bot.Repositories.Interfaces;

namespace AI.Assistant.Bot.Repositories;

public class ContextRepository(Supabase.Client client) : IContextRepository
{
    public async Task SaveContextAsync(long chatId, string info)
    {
        var context = new ContextModel(chatId, info);
        await client.From<ContextModel>().Insert(context);
    }
    public async Task<List<string>> GetContextByChatIdAsync(long chatId)
    {
        var rows = await client
            .From<ContextModel>()
            .Where(x => x.ChatId == chatId)
            .Get();
        return rows.Models.Select(x => x.Text).ToList();
    }
}