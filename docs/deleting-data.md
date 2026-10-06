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

- The entity implements `ISoftDeletable` from `src/Records`: `DeletedOn` (date and time it was deleted, `null` = not deleted), `IsDeleted` (computed from `DeletedOn`, not stored) and `Delete(now)`.
- `ApplySoftDeleteQueryFilters` (`src/Infrastructure/Persistence`) adds a global query filter named `SoftDelete` to every `ISoftDeletable` entity, so deleted rows never show up and repositories don't filter by hand. The tenant filter is named too (`Tenant`), and both apply together. To read deleted rows (an audit, a restore), use `IgnoreQueryFilters([SoftDeleteModelBuilderExtensions.SoftDeleteQueryFilter])` inside `Infrastructure`, which keeps the tenant filter.
- Every unique index of an `ISoftDeletable` entity is filtered on `[DeletedOn] IS NULL`, so the same key can be used again after a delete.
- Tests enforce it: every `ISoftDeletable` entity has both filters, every unique index ignores deleted rows, and a deleted row stays in the table but is not returned.

`ISoftDeletable` is not `IDeletedOn`: `IDeletedOn` marks a record that another one replaced (a subscription, an add-on), whose history is still read with `WhereCurrent()`. A soft deleted row was removed by a user and is hidden everywhere.

Rows are physically deleted only when they have no value once gone, for example web push subscriptions or rows the system recreates on its own.

Example: upcoming fee changes (`DefaultMonthlyFeeChange`, `ClientBillingPlanChange`). Only changes for months after the current one can be deleted. The row keeps its `DeletedOn`, and saving the same month again creates a new row.

## Deletes that don't follow these rules yet

| What | Confirmation | Storage |
| --- | --- | --- |
| Payment (client card → Cobro) | No | Hard delete |
| Class pack purchase ("Anular") | No | Hard delete |
| Announcement | No | Hard delete |
| Class comment (feedback) | No | Hard delete |
| Private lesson | Yes | Hard delete |
| Custom role | Yes | Hard delete |
| Team member | Yes | Hard delete |

Each of these should move to the rules above in its own change.
