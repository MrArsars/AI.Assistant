using AI.Assistant.Core.Models;

namespace AI.Assistant.Application.Interfaces;

public interface IJsonConverterService
{
    string RequestToJson(AiRequest request);
    AiRequest RequestFromJson(string json);
    AiResponse JsonToResponse(string response);
}