using AI.Assistant.Application.Interfaces;
using AI.Assistant.Core.Models;
using Microsoft.SemanticKernel.ChatCompletion;

namespace AI.Assistant.Application.Services;

public class MessagesService(IMessagesRepository messagesRepository) : IMessagesService
{
    public async Task SaveToRepositoryAsync(string text, long chatId, AuthorRole role, float[]? embedding)
    {
        var messageModel = new Message(chatId, role.Label, "", text, embedding);
        await messagesRepository.SaveMessageAsync(messageModel);
    }

    public async Task<ChatHistory> GetLatestHistoryByChatIdAsync(long chatId, bool useLimit = true)
    {
        var result = await messagesRepository.GetLatestHistoryByChatIdAsync(chatId, useLimit);
        return result;
    }
}