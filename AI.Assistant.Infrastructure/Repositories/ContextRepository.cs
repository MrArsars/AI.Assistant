using AI.Assistant.Application.Interfaces;
using AI.Assistant.Core.Interfaces;
using AI.Assistant.Infrastructure.Persistence.Models;
using AutoMapper;
using Polly;
using Polly.Registry;
using Supabase;
using Context = AI.Assistant.Core.Models.Context;

namespace AI.Assistant.Infrastructure.Repositories;

public class ContextRepository(Client client, IPolicyRegistry<string> policyRegistry, IMapper mapper)
    : IContextManager, IContextProvider
{
    private readonly IAsyncPolicy _retryPolicy = policyRegistry.Get<IAsyncPolicy>("DbRetryPolicy");

    public async Task SaveContextAsync(long chatId, string info)
    {
        var context = new Context(chatId, info);
        var dto = mapper.Map<ContextDto>(context);
        await _retryPolicy.ExecuteAsync(async () => { await client.From<ContextDto>().Insert(dto); });
    }

    public async Task<List<string>> GetContextByChatIdAsync(long chatId)
    {
        var rows = await client
            .From<ContextDto>()
            .Where(x => x.ChatId == chatId)
            .Get();
        return await _retryPolicy.ExecuteAsync(() =>
        {
            return Task.FromResult(rows.Models.Select(x => x.Text).ToList());
        });
    }
}