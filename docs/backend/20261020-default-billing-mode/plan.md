# Default billing mode of a business

Follow-up to [fee history and class packs](../20260927-fee-history-and-class-packs/plan.md). Until now every family without a billing plan of its own paid the business's monthly fee, and Cobros told every new business "Todavía no definiste la cuota mensual", even a studio that only sells class packs.

## Decisions

- **A business chooses how it charges**: `MonthlyFee` (cuota mensual) or `ClassPacks` (packs de clases). It is chosen in Ajustes › Forma de cobro, the screen that used to be Ajustes › Cuota mensual.
- **The choice is effective-dated, like the fee.** It is stored on the same change (`DefaultMonthlyFeeChanges.BillingMode`), so one change says "from this month the business charges this way, with this fee". Earlier months keep how they were charged.
- **A family without a plan of its own follows the business in each month**: `BusinessFee` when the business charges a monthly fee, `ClassPacks` when it charges with packs. A plan chosen for a family (Cuota general, Cuota propia, Paga por clases) does not change when the business switches; the family's screen keeps its three options and only the one marked by default follows the business.
- **A new family starts with the business's choice written down.** Registering a family (by hand or with the students import) records its own plan from the current month, copied from how the business charges: `ClassPacks` when it charges with packs, `BusinessFee` when it charges a monthly fee. A change the business already scheduled for a later month is copied too. So a family that joined paying with packs keeps paying with packs when the business later moves to a monthly fee, and the switch only reaches families that join afterwards. A business that has not chosen yet records nothing.
- **Every family has its plan written down.** Nuevo alumno asks "¿Cómo cobrás?" while the business has not chosen how it charges: the answer becomes the business's billing mode from the current month (`billingMode` on `POST /api/clients`, ignored once the business has one) and the new family's plan. The business's first choice, from that form or from Ajustes › Forma de cobro, also records it for every family registered before without a plan of its own (a students import before choosing). Migration `RecordBillingPlanOfRegisteredClients` gave every family without a plan the business's timeline at the time (only the months where it switched), so nobody follows the business any more. Ajustes › Forma de cobro says the choice applies to the students added from now on.
- **The fee amount is kept** when the business moves to packs, so a family set to Cuota general still pays it.
- **Not chosen yet** = the business has no change at all. Cobros then asks "Todavía no elegiste cómo cobrás" instead of asking for the monthly fee. Businesses that existed before this change keep charging a monthly fee: the migration fills `BillingMode` with `MonthlyFee`.
- **Cobros with packs by default** lists the families paying per class first and the ones paying a fee under "Pagan cuota". In both modes "Deben" and "Todos" count the families paying per class too, and the summary says how many owe classes.

## Contract

```json
PUT /api/business/monthly-fee { "amount": 25000, "effectiveFrom": "2026-11", "billingMode": "ClassPacks" }

GET /api/business → { ..., "billingMode": "ClassPacks",
  "defaultMonthlyFeeChanges": [ { "effectiveFrom": "2026-11", "amount": 25000, "billingMode": "ClassPacks" } ] }

GET /api/fees?month=2026-11 → { ..., "billingMode": "ClassPacks" }
```

`billingMode` is optional in the request and defaults to `MonthlyFee`, so the previous app version keeps working during a deploy. `GET /api/business` returns `billingMode: null` while no change applies yet.

## Data

Migration `AddBusinessBillingMode` adds `DefaultMonthlyFeeChanges.BillingMode` (`nvarchar(16)`, default `MonthlyFee`). It is additive: the previous API version still inserts changes without the column and gets the default.
