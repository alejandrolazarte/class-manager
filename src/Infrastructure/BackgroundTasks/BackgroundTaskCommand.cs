using System.Text.Json.Serialization;

namespace ClassManager.Infrastructure.BackgroundTasks;

public abstract record BackgroundTaskCommand([property: JsonIgnore] Guid? TenantId);
