namespace AI.Assistant.Application.Interfaces;

public interface IWebService
{
    Task<string> SearchWeb(string query);
}