using AI.Assistant.Bot.Models;
using AI.Assistant.Bot.Repositories.Interfaces;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace AI.Assistant.Bot.Repositories;

public class MessagesRepository(Supabase.Client client, Settings settings) : IMessagesRepository
{
    public async Task SaveMessageAsync(MessageModel message)
    {
        await client.From<MessageModel>().Insert(message);
    }

    public async Task<ChatHistory> GetLatestHistoryByChatIdAsync(long chatId, bool useLimit = true)
    {
        var rows = await client.From<MessageModel>()
            .Where(x => x.ChatId == chatId)
            .Limit(useLimit ? settings.HistoryMinLimit : int.MaxValue)
            .Get();
        var messages = rows.Models.Select(m => new ChatMessageContent(new AuthorRole(m.Role), m.Text));
        return new ChatHistory(messages);
    }
}