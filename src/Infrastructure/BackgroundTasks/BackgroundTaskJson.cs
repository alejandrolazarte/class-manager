using System.Text.Json;

namespace ClassManager.Infrastructure.BackgroundTasks;

internal static class BackgroundTaskJson
{
    public static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web);
}
