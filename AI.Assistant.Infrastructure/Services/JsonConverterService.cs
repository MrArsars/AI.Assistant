using System.Text.Json;
using AI.Assistant.Application.Interfaces;
using AI.Assistant.Core.Models;

namespace AI.Assistant.Infrastructure.Services;

public class JsonConverterService : IJsonConverterService
{
    public string RequestToJson(AiRequest request)
    {
        var result = JsonSerializer.Serialize(request);
        return result;
    }

    public AiRequest RequestFromJson(string json)
    {
        var options = new JsonSerializerOptions
        {
            IncludeFields = true
        };
        var result = JsonSerializer.Deserialize<AiRequest>(json, options);
        return result ?? throw new Exception();
    }

    public AiResponse JsonToResponse(string response)
    {
        var options = new JsonSerializerOptions
        {
            IncludeFields = true
        };
        var result = JsonSerializer.Deserialize<AiResponse>(response);
        return result ?? throw new Exception();
    }
}