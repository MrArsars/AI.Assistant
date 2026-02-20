using AI.Assistant.Application.Interfaces;
using AI.Assistant.Core.Interfaces;
using AI.Assistant.Core.Models;
using Polly;
using Polly.Registry;
using Supabase;

namespace AI.Assistant.Infrastructure.Repositories;

public class ContextRepository(Client client, IPolicyRegistry<string> policyRegistry)
    : IContextManager, IContextProvider
{
    private readonly IAsyncPolicy _retryPolicy = policyRegistry.Get<IAsyncPolicy>("DbRetryPolicy");

    public async Task SaveContextAsync(long chatId, string info)
    {
        var context = new ContextModel(chatId, info);
        await _retryPolicy.ExecuteAsync(async () => { await client.From<ContextModel>().Insert(context); });
    }

    public async Task<List<string>> GetContextByChatIdAsync(long chatId)
    {
        var rows = await client
            .From<ContextModel>()
            .Where(x => x.ChatId == chatId)
            .Get();
        return await _retryPolicy.ExecuteAsync(async () => { return rows.Models.Select(x => x.Text).ToList(); });
    }
}