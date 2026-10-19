using Microsoft.AspNetCore.Http;

namespace ClassManager.Subscriptions.AspNetCore.Endpoints;

public static class PlanLimitResponses
{
    private const string PlanLimitItemKey = "ClassManager.Subscriptions.PlanLimit";

    public static void Mark(HttpContext httpContext) => httpContext.Items[PlanLimitItemKey] = true;

    public static bool IsMarked(HttpContext httpContext) => httpContext.Items.ContainsKey(PlanLimitItemKey);
}
