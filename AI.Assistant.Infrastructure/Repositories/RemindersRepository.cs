using AI.Assistant.Core.Interfaces;
using AI.Assistant.Core.Models;

namespace AI.Assistant.Infrastructure.Repositories;

public class RemindersRepository(Supabase.Client client) : IRemindersRepository
{
    public async Task SaveReminderAsync(ReminderModel reminder)
    {
        await client.From<ReminderModel>().Insert(reminder);
    }

    public async Task<IEnumerable<ReminderModel>> GetNeededRemindersAsync()
    {
        var response = await client.From<ReminderModel>()
            .Where(x => x.IsActive == true && x.NextRunAt <= DateTime.UtcNow)
            .Get();

        return response.ResponseMessage is { IsSuccessStatusCode: false } ? [] : response.Models;
    }

    public async Task UpdateReminderAsync(Guid reminderId, bool? isActive = null, DateTime? nextRunAt = null)
    {
        var query = client.From<ReminderModel>()
            .Where(x => x.Id == reminderId);

        if (isActive.HasValue)
            query = query.Set(x => x.IsActive, isActive.Value);

        if (nextRunAt.HasValue)
            query = query.Set(x => x.NextRunAt, nextRunAt.Value);

        await query.Update();
    }
}