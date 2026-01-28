using AI.Assistant.Bot.Models;
using AI.Assistant.Bot.Repositories.Interfaces;
using Supabase.Postgrest;

namespace AI.Assistant.Bot.Repositories;

public class RemindersRepository(Supabase.Client client) : IRemindersRepository
{
    public async Task SaveReminderAsync(ReminderModelDto reminder)
    {
        await client.From<ReminderModelDto>().Insert(reminder);
    }

    public async Task<IEnumerable<ReminderModel>> GetNeededRemindersAsync()
    {
        var response = await client.From<ReminderModelDto>()
            .Where(x => x.IsActive == true && x.NextRunAt <= DateTime.UtcNow)
            .Get();

        if (response.ResponseMessage is { IsSuccessStatusCode: false })
            return [];

        return response.Models.Select(dto => new ReminderModel(dto));
    }

    public async Task UpdateReminder(Guid reminderId, bool? isActive = null, DateTime? nextRunAt = null)
    {
        var query = client.From<ReminderModelDto>()
            .Where(x => x.Id == reminderId);

        if (isActive.HasValue)
            query = query.Set(x => x.IsActive, isActive.Value);

        if (nextRunAt.HasValue)
            query = query.Set(x => x.NextRunAt, nextRunAt.Value);

        await query.Update();
    }
}