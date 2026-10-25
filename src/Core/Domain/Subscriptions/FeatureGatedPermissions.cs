using ClassManager.Core.Domain.Authorization;

namespace ClassManager.Core.Domain.Subscriptions;

public static class FeatureGatedPermissions
{
    public static IReadOnlyDictionary<string, IReadOnlyList<string>> AnyFeatureByPermission { get; } =
        new Dictionary<string, IReadOnlyList<string>>(StringComparer.Ordinal)
        {
            [Permissions.ImportExport.Run] = [Features.ImportExport],
            [Permissions.Roles.Manage] = [Features.CustomRoles],
            [Permissions.ClassPacks.Manage] = [Features.ClassPacks],
            [Permissions.ClassPacks.Sell] = [Features.ClassPacks],
            [Permissions.PrivateLessons.ManageOwn] = [Features.ClassPacks],
            [Permissions.PrivateLessons.ManageAll] = [Features.ClassPacks],
            [Permissions.Products.Manage] = [Features.Shop],
            [Permissions.Orders.Manage] = [Features.Shop, Features.ClassPacks],
        };
}
