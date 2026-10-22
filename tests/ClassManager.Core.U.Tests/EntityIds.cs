using System.Collections;

namespace ClassManager.Core.U.Tests;

internal static class EntityIds
{
    private const string IdPropertyName = "Id";
    private const string UserPropertyName = "User";
    private const string UserIdPropertyName = "UserId";

    public static TEntity WithId<TEntity>(this TEntity entity, long? id = null)
        where TEntity : class
    {
        entity.WithValue(IdPropertyName, id ?? TestIds.Next());
        if (entity is not User && entity.GetType().GetProperty(UserPropertyName)?.GetValue(entity) is User { Id: 0 } user)
        {
            user.WithId();
            entity.WithValue(UserIdPropertyName, user.Id);
        }

        return entity.WithChildIds();
    }

    public static TEntity WithChildIds<TEntity>(this TEntity entity)
        where TEntity : class
    {
        var parentId = IdOf(entity);
        var parentIdPropertyName = entity.GetType().Name + IdPropertyName;
        foreach (var property in entity.GetType().GetProperties())
        {
            if (property.PropertyType == typeof(string) || property.GetIndexParameters().Length > 0
                || property.GetValue(entity) is not IEnumerable children)
            {
                continue;
            }

            foreach (var child in children.OfType<object>().Where(child => IdOf(child) == 0))
            {
                child.WithId();
                if (child.GetType().GetProperty(parentIdPropertyName)?.PropertyType == typeof(long))
                {
                    child.WithValue(parentIdPropertyName, parentId);
                }
            }
        }

        return entity;
    }

    public static TEntity WithValue<TEntity>(this TEntity entity, string propertyName, object? value)
        where TEntity : class
    {
        entity.GetType().GetProperty(propertyName)!.SetValue(entity, value);
        return entity;
    }

    private static long? IdOf(object entity) =>
        entity.GetType().GetProperty(IdPropertyName)?.GetValue(entity) as long?;
}
