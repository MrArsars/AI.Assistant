using AI.Assistant.Bot.Models;
using AI.Assistant.Bot.Repositories.Interfaces;
using AI.Assistant.Bot.Services.Interfaces;
using Microsoft.SemanticKernel.ChatCompletion;

namespace AI.Assistant.Bot.Services;

public class MessagesService(IMessagesRepository messagesRepository) : IMessagesService
{
    public async Task SaveToRepositoryAsync(string text, long chatId, AuthorRole role)
    {
        var messageModel = new MessageModel(chatId, role.Label, text);
        await messagesRepository.SaveMessageAsync(messageModel);
    }
    
    public async Task<ChatHistory> GetLatestHistoryByChatIdAsync(long chatId, bool useLimit = true)
    {
        var result = await messagesRepository.GetLatestHistoryByChatIdAsync(chatId, useLimit);
        return result;
    }
}