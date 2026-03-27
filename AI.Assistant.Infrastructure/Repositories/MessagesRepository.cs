using AI.Assistant.Application;
using AI.Assistant.Application.Interfaces;
using AI.Assistant.Core.Models;
using AI.Assistant.Infrastructure.Persistence.Models;
using AutoMapper;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Polly;
using Polly.Registry;
using Supabase;
using static Supabase.Postgrest.Constants;

namespace AI.Assistant.Infrastructure.Repositories;

public class MessagesRepository(
    Client client,
    IPolicyRegistry<string> policyRegistry,
    IMapper mapper,
    IOptions<ApplicationSettings> settings)
    : IMessagesRepository
{
    private readonly IAsyncPolicy _retryPolicy = policyRegistry.Get<IAsyncPolicy>("DbRetryPolicy");

    public async Task SaveMessageAsync(Message message)
    {
        var dto = mapper.Map<MessageDto>(message);
        await _retryPolicy.ExecuteAsync(async () => { await client.From<MessageDto>().Insert(dto); });
    }

    public async Task<ChatHistory> GetLatestHistoryByChatIdAsync(long chatId, bool useLimit = true)
    {
        return await _retryPolicy.ExecuteAsync(async () =>
        {
            var rows = await client.From<MessageDto>()
                .Order(x => x.CreatedAt, Ordering.Descending)
                .Where(x => x.ChatId == chatId)
                .Limit(useLimit ? settings.Value.HistoryMinLimit : int.MaxValue)
                .Get();

            var messages = rows.Models.AsEnumerable()
                .Reverse()
                .Select(m => new ChatMessageContent(new AuthorRole(m.Role), m.Text))
                .ToList();

            return new ChatHistory(messages);
        });
    }
}