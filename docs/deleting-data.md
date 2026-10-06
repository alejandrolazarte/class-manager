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
- `ApplySoftDeleteQueryFilters` (`src/Infrastructure/Persistence`) adds a global query filter named `SoftDelete` (`[IsDeleted] = 0`) and the check constraint to every `ISoftDeletable` entity, so deleted rows never show up and repositories don't filter by hand. The tenant filter is named too (`Tenant`), and both apply together. To read deleted rows (an audit, a restore), use `IgnoreQueryFilters([SoftDeleteModelBuilderExtensions.SoftDeleteQueryFilter])` inside `Infrastructure`, which keeps the tenant filter.
- Every unique index of an `ISoftDeletable` entity is filtered on `[IsDeleted] = 0` (`SoftDeleteModelBuilderExtensions.NotDeletedFilter`), so the same key can be used again after a delete.
- Tests enforce it: every `ISoftDeletable` entity stores `IsDeleted` and has both filters, every unique index ignores deleted rows, the check constraint rejects an `IsDeleted` without `DeletedOn`, and a deleted row stays in the table but is not returned.

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

## Deletes that don't follow these rules yet

| What | Confirmation | Storage |
| --- | --- | --- |
| Private lesson | Yes | Hard delete |
| Custom role | Yes | Hard delete |
| Team member | Yes | Hard delete |

Payments, class pack purchases ("Anular"), announcements, class comments and upcoming fee changes already follow both rules. Each of the rows above should move to them in its own change.
