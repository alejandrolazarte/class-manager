# Deleting data

Two rules apply to anything a user can delete.

## 1. Every delete asks for confirmation

An action that deletes something never runs on the first tap. It opens `DeleteConfirmation` (`app/src/ui/DeleteConfirmation.tsx`): a warning banner with the question, **Cancelar** and **Sí, borrar**.

- The question names what is deleted and what happens after, for example "¿Borrar el cambio desde Diciembre 2026? Ese mes sigue con la cuota anterior."
- The delete button is an `IconButton` with `icon="delete"` and an accessibility label that names the item ("Borrar el cambio desde {month}").
- A test proves that nothing is deleted until the user confirms, and that **Cancelar** deletes nothing.

Removing something from a form that is not saved yet (a photo in a form, a student in a sale that is being built, an item in the cart) is not a delete: it changes the form and needs no confirmation.

## 2. Data with business meaning is soft deleted

Payments, fees, plans, classes and anything else a business would want to audit or recover is marked as deleted instead of removed:

- The entity implements `ISoftDeletable` from `src/Records`: `IsDeleted` (a stored `bit` column), `DeletedOn` (date and time it was deleted, `null` = not deleted) and `Delete(now)`, which sets both.
- A check constraint `CK_<Table>_IsDeleted_DeletedOn` keeps them in sync: `IsDeleted = 0` with no `DeletedOn`, or `IsDeleted = 1` with a `DeletedOn`. The database rejects any other combination, even from a manual `UPDATE`.
- `SoftDeleteSaveChangesInterceptor` (`src/Infrastructure/Persistence`) turns removing an `ISoftDeletable` entity into `Delete(now)`, so a repository `Remove` or a use case calling `Delete(now)` both end in a soft delete, and such a row can't be physically deleted by mistake.
- `ApplySoftDeleteQueryFilters` (`src/Infrastructure/Persistence`) adds a global query filter named `SoftDelete` (`[IsDeleted] = 0`) and the check constraint to every `ISoftDeletable` entity, so deleted rows never show up and repositories don't filter by hand. The tenant filter is named too (`Tenant`), and both apply together. To read deleted rows (an audit, a restore), use `IgnoreQueryFilters([SoftDeleteModelBuilderExtensions.SoftDeleteQueryFilter])` inside `Infrastructure`, which keeps the tenant filter. Queries that read across tenants use `IgnoreTenantFilter()`, which keeps hiding deleted rows; `IgnoreQueryFilters()` without names is forbidden (ARCH010) because it would show them.
- Every unique index of an `ISoftDeletable` entity is filtered on `[IsDeleted] = 0` (`SoftDeleteModelBuilderExtensions.NotDeletedFilter`), so the same key can be used again after a delete.
- Tests enforce it: every `ISoftDeletable` entity stores `IsDeleted` and has both filters, every unique index ignores deleted rows, the check constraint rejects an `IsDeleted` without `DeletedOn`, and a deleted row stays in the table but is not returned.

### Where global query filters don't protect

From the EF Core documentation (Global Query Filters) and how EF applies them:

- They apply to LINQ reads only. Writes (`SaveChanges`) are not filtered: `TenantStampingSaveChangesInterceptor` rejects changing another tenant's rows, and `SoftDeleteSaveChangesInterceptor` turns removals into soft deletes. Raw SQL (`ExecuteSql`) is not filtered either.
- `IgnoreQueryFilters()` without names turns off every filter. ARCH010 forbids it; use `IgnoreTenantFilter()` or name the filters.
- An unnamed `HasQueryFilter` replaces the previous one. Both filters here are named.
- A required navigation to an entity with a filter is loaded with an `INNER JOIN`, so when the related row is filtered out (deleted, another tenant) the parent row disappears too. No navigation points to a soft deletable entity today, and `AppDbContext` turns EF's warning for this case (`PossibleIncorrectRequiredNavigationWithQueryFilterInteractionWarning`) into an exception, so a new one fails the tests.
- Filters can only be defined on the root type of an inheritance hierarchy, and EF doesn't detect cycles between filters. Neither applies today.

### Why `IsDeleted` is stored

Measured on SQL Server 2022 with 2 million payments (5% deleted), the monthly query `WHERE TenantId = @t AND Month = @m AND <not deleted>`:

| Index | `DeletedOn IS NULL` | `IsDeleted = 0` |
| --- | --- | --- |
| The current one, not filtered | same plan and reads (1,853), same time | same plan and reads (1,853), same time |
| Filtered on the deleted column, covering, including that column | 6 reads, 367 µs CPU | 6 reads, 160 µs CPU |

With a filtered index, SQL Server drops the `IsDeleted = 0` predicate because the index already guarantees it, but keeps checking `DeletedOn IS NULL` row by row. So the `bit` is about twice as cheap there. Without a filtered index both cost the same. A computed column can't be used in an index filter, so `IsDeleted` is a real column and the check constraint keeps it honest.

When a large table needs a faster index, filter it on `[IsDeleted] = 0` and add `IsDeleted` to its `INCLUDE` columns. Without the column in the index, SQL Server did not use the filtered index in the measurement.

`ISoftDeletable` is not `IDeletedOn`: `IDeletedOn` marks a record that another one replaced (a subscription, an add-on), whose history is still read with `WhereCurrent()`. A soft deleted row was removed by a user and is hidden everywhere.

Rows are physically deleted only when they have no value once gone, for example web push subscriptions or rows the system recreates on its own.

Example: upcoming fee changes (`DefaultMonthlyFeeChange`, `ClientBillingPlanChange`). Only changes for months after the current one can be deleted. The row keeps `IsDeleted` and `DeletedOn`, and saving the same month again creates a new row.

## Hard deletes on purpose

| What | Confirmation | Why it is not soft deleted |
| --- | --- | --- |
| Private lesson | Yes | It can only be deleted while it has no attendance and was not deducted as a trial from a pack sale, so nothing depends on it; it is meant for a lesson loaded by mistake. A lesson that didn't happen is cancelled instead, which keeps it. |
| Web push subscriptions | No user action | The device subscribes again on its own. |

Everything else a user can delete is soft deleted: payments, class pack purchases ("Anular"), announcements, class comments, upcoming fee changes, team members and custom roles.
