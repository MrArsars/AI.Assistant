using AI.Assistant.Application.Interfaces;
using AI.Assistant.Core.Models;
using AI.Assistant.Infrastructure.Persistence.Models;
using AutoMapper;
using Polly;
using Polly.Registry;
using Supabase;

namespace AI.Assistant.Infrastructure.Repositories;

public class RemindersRepository(
    Client client,
    IPolicyRegistry<string> policyRegistry,
    IMapper mapper) : IRemindersRepository
{
    private readonly IAsyncPolicy _retryPolicy = policyRegistry.Get<IAsyncPolicy>("DbRetryPolicy");

    public async Task SaveReminderAsync(Reminder reminder)
    {
        var dto = mapper.Map<ReminderDto>(reminder);
        await _retryPolicy.ExecuteAsync(async () => { await client.From<ReminderDto>().Insert(dto); });
    }

    public async Task<IEnumerable<Reminder>> GetNeededRemindersAsync()
    {
        var localTime = DateTime.Now.ToLocalTime();
        return await _retryPolicy.ExecuteAsync(async () =>
        {
            var response = await client.From<ReminderDto>()
                .Where(x => x.IsActive == true && x.NextRunAt <= localTime)
                .Get();
            return response.ResponseMessage is { IsSuccessStatusCode: false }
                ? []
                : mapper.Map<List<Reminder>>(response.Models);
        });
    }

    public async Task UpdateReminderAsync(Guid reminderId, bool? isActive = null, DateTime? nextRunAt = null)
    {
        await _retryPolicy.ExecuteAsync(async () =>
        {
            var query = client.From<ReminderDto>()
                .Where(x => x.Id == reminderId);

            if (isActive.HasValue)
                query = query.Set(x => x.IsActive, isActive.Value);

            if (nextRunAt.HasValue)
                query = query.Set(x => x.NextRunAt, nextRunAt.Value);

            await query.Update();
        });
    }
}