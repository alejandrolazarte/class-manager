# Backend plan — Fee history and class packs (M6)

Follow-up to [monthly fees](../20260926-monthly-fees/plan.md), asked by the pilot instructor:

1. Raising the fee rewrote every past month: a family that paid 25.000 in January showed "paid 25.000 of 30.000" after a March raise.
2. Instructors also sell classes: a single class for 25 €, 4 classes for 80 €, or 8 Saturdays paid up front for 160 €.

## Decisions

- **Fees are effective-dated.** A change applies from a month on (`effectiveFrom`, default the current month). Each month uses the fee in force that month; earlier months keep theirs. Setting a change for a month that already has one replaces it.
- **Each family has a billing plan, also effective-dated**: `BusinessFee` (the business's default, the default when no plan was set), `CustomFee` (its own monthly amount) or `ClassPacks` (pays per class). A family on class packs is not in the month's fee list; it shows its class balance instead.
- **Class packs are a catalog** kept by the business: name, number of classes, price and an optional validity in months ("8 classes, 2 months"). A single class is a pack of 1.
- **Selling a pack records the money** (it is the payment): price (defaults to the catalog's, can be lowered for a discount), date, method, notes. The purchase copies name, classes and validity, so editing the catalog never changes what was sold.
- **A class is used only when the student attended** ("Vino"). Absences and cancelled sessions don't use classes.
- **Expiry is optional per pack.** With a validity, classes left after `expiresOn` (purchase date + validity − 1 day) are lost; without one, they stay until used.
- **Which pack pays an attendance**: attendances in months when the family was on `ClassPacks`, oldest first, each one from the pack valid on that date that expires first (packs without expiry last), then the oldest purchase. An attendance with no pack left is an **unpaid class**; the next pack sold covers it.
- The balance is **computed, not stored**: purchases + attendances → classes used, left, expired and unpaid. No counters to keep in sync when attendance is corrected.

## Scope

| Operation | Endpoint | Use case |
|---|---|---|
| Change the default fee from a month | `PUT /api/business/monthly-fee` `{ amount, effectiveFrom }` | `SetDefaultMonthlyFeeUseCase` |
| Change a family's billing plan from a month | `PUT /api/clients/{clientId}/billing-plan` `{ kind, customFee, effectiveFrom }` | `SetClientBillingPlanUseCase` (replaces `PUT .../monthly-fee`) |
| Pack catalog | `GET/POST /api/class-packs`, `PUT /api/class-packs/{id}`, `PUT /api/class-packs/{id}/active` | `ListClassPacksUseCase`, `CreateClassPackUseCase`, `UpdateClassPackUseCase`, `SetClassPackActiveUseCase` |
| Sell a pack to a family | `POST /api/clients/{clientId}/class-pack-purchases` | `SellClassPackUseCase` |
| Undo a sale (mistakes) | `DELETE /api/class-pack-purchases/{purchaseId}` | `DeleteClassPackPurchaseUseCase` |
| A family's class balance | `GET /api/clients/{clientId}/class-balance` | `GetClientClassBalanceUseCase` |
| Fees of a month | `GET /api/fees?month=2026-09` | `ListMonthlyFeesUseCase` (+ `classPackClients`, `classPackSales`) |

`GET /api/business` gains `defaultMonthlyFeeChanges`; `GET /api/clients/{id}` replaces `monthlyFee` with `billingPlan` (in force today) and `billingPlanChanges`. `ClientResponse` drops `monthlyFee`.

## Contract

```json
PUT /api/business/monthly-fee            { "amount": 30000, "effectiveFrom": "2026-11" }
PUT /api/clients/{clientId}/billing-plan  { "kind": "ClassPacks", "customFee": null, "effectiveFrom": "2026-10" }

POST /api/class-packs   { "name": "8 clases", "classCount": 8, "price": 160, "validityMonths": 2 }

POST /api/clients/{clientId}/class-pack-purchases
{ "classPackId": "0192...", "price": 150, "purchasedOn": "2026-10-03", "method": "Cash", "notes": null }

GET /api/clients/{clientId}/class-balance
{
  "availableClasses": 5,
  "unpaidClasses": 0,
  "purchases": [
    { "id": "...", "name": "8 clases", "classCount": 8, "price": 150, "purchasedOn": "2026-10-03",
      "expiresOn": "2026-12-02", "usedClasses": 3, "remainingClasses": 5, "status": "Active" }
  ],
  "unpaidAttendances": []
}

GET /api/fees?month=2026-10 → { ..., "classPackSales": 150,
  "classPackClients": [ { "clientId": "...", "clientFullName": "...", "studentNames": [...], "availableClasses": 5, "unpaidClasses": 0 } ] }
```

Purchase `status`: `Active`, `UsedUp` or `Expired`. `classPackClients` lists families on `ClassPacks` that month with a student enrolled in it, ordered by unpaid classes first.

## Business rules

| # | Rule | Result |
|---|---|---|
| H1 | `effectiveFrom` has the `YYYY-MM` format, from 2000-01 to 12 months after the current month (same range as payments) | Validation |
| H2 | `CustomFee` requires `customFee`; the other kinds ignore it | Validation |
| P1 | Pack name 2–60 characters, unique among the business's packs | Validation / `409` |
| P2 | Classes 1–100; validity, if present, 1–24 months | Validation |
| P3 | Price and sale price follow the amount rule of fees (F1) | Validation |
| P4 | Only active packs can be sold | Validation |
| P5 | Sale date not after today; method required; notes ≤ 200 characters | Validation |
| P6 | Pack, purchase and client belong to the business | `404` |

## Data

- `DefaultMonthlyFeeChanges` (`TenantId`, `EffectiveFrom`, `Amount?`), unique `(TenantId, EffectiveFrom)`.
- `ClientBillingPlanChanges` (`TenantId`, `ClientId`, `EffectiveFrom`, `Kind`, `CustomFee?`), unique `(TenantId, ClientId, EffectiveFrom)`.
- `ClassPacks`, `ClassPackPurchases` (index `(TenantId, ClientId)`).
- Migration `AddFeeHistoryAndClassPacks` moves `Businesses.DefaultMonthlyFee` and `Clients.MonthlyFee` into the change tables effective from 2000-01, so nothing changes for existing data, then drops both columns.

## Tests (write first)

Unit: effective fee per month, billing plan per month, pack and purchase validation, the balance allocation (earliest expiry first, expired classes lost, unpaid classes, only months on packs), monthly list with a raise and with pack families.

Integration: a raise keeps past months, a family on packs leaves the fee list, selling a pack and attending uses a class, tenant isolation for the four new tables.
