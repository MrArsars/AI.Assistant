using AI.Assistant.Core.Interfaces;
using AI.Assistant.Core.Models;
using Microsoft.SemanticKernel.ChatCompletion;

namespace AI.Assistant.Core.Services;

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