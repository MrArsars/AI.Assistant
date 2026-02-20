using AI.Assistant.Core;
using AI.Assistant.Core.Interfaces;
using AI.Assistant.Core.Models;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Polly;
using Polly.Registry;
using Supabase;

namespace AI.Assistant.Infrastructure.Repositories;

public class MessagesRepository(Client client, Settings settings, IPolicyRegistry<string> policyRegistry)
    : IMessagesRepository
{
    private readonly IAsyncPolicy _retryPolicy = policyRegistry.Get<IAsyncPolicy>("DbRetryPolicy");

    public async Task SaveMessageAsync(MessageModel message)
    {
        await _retryPolicy.ExecuteAsync(async () => { await client.From<MessageModel>().Insert(message); });
    }

    public async Task<ChatHistory> GetLatestHistoryByChatIdAsync(long chatId, bool useLimit = true)
    {
        return await _retryPolicy.ExecuteAsync(async () =>
        {
            var rows = await client.From<MessageModel>()
                .Where(x => x.ChatId == chatId)
                .Limit(useLimit ? settings.HistoryMinLimit : int.MaxValue)
                .Get();
            var messages = rows.Models.Select(m => new ChatMessageContent(new AuthorRole(m.Role), m.Text));
            return new ChatHistory(messages);
        });
    }
}