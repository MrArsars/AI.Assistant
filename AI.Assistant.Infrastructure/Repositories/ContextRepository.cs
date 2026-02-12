using AI.Assistant.Application.Interfaces;
using AI.Assistant.Core.Interfaces;
using AI.Assistant.Core.Models;

namespace AI.Assistant.Infrastructure.Repositories;

public class ContextRepository(Supabase.Client client) : IContextManager, IContextProvider
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