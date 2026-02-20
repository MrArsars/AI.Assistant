using System.Net.Sockets;
using Polly;

namespace AI.Assistant.Infrastructure.Resilience;

public class ResiliencePolicyFactory
{
    public static IAsyncPolicy GetDbRetryPolicy()
    {
        return Policy
            .Handle<HttpRequestException>()
            .Or<IOException>()
            .OrInner<SocketException>()
            .WaitAndRetryAsync(6, retryAttempt =>
                TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
    }
}