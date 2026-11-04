# Monthly plans in the shop and at the counter

Date: 2026-11-03

Status: agreed with the owner on 2026-11-03. Steps 1, 2 and 4 built; step 3 next. Builds on the [plans catalog](../20261031-plans-catalog/plan.md).

## Why

Since the plans catalog, a monthly plan is given to a person (`StudentPlanChanges`) and never sold: the counter sale, Vender pack and the student shop list only packs bought once, and `ClassPackPurchase` rejects a plan (`class_pack.renews_monthly`). The only places to give someone a plan are Nuevo alumno ("¿Qué paga?", required) and the student's Forma de cobro.

The owner's point: a monthly plan is a subscription like in any other app. The student picks it, it renews by itself every month, and the student can cancel it. The instructor decides whether each plan is offered in the shop. Nothing in the billing model prevents it: what a family owes is already the sum of its students' plans, and a plan already renews every month. What was missing is a way for the student (or the counter) to write the student's plan.

## Decisions

### Shown in the shop, per item

- Packs and plans get **Mostrar en la tienda** (`ClassPacks.IsVisibleInApp`), like products. The student shop lists only active items shown in the shop.
- New packs and plans are shown by default. Existing packs stay shown (as today); existing plans start hidden, so no business sees its plans appear in the shop without choosing it.

### Choosing a plan from the student app

- The shop lists the plans shown in the shop next to the packs: name, price per month, classes per month (or libre), description.
- Opening a plan lists the people of the family the account can buy for, each with their plan. The person who pays for the family chooses a plan for anyone in it; an account that can't buy (a minor whose guardian pays) sees the plan but can't choose it, as with packs.
- **When it starts**:
  - A person with no plan this month starts this month: they owe the plan's price for the current month, as when the instructor gives them a plan today.
  - A person who already has a plan this month changes from next month: this month is already owed with the plan they had.
- **Cancelling**: the person keeps the plan until the end of the month and has no plan from next month (they pay per class, with packs). A plan that has not started yet (chosen for next month) is just removed.
- Choosing or cancelling replaces any change the instructor had scheduled from that month on, so what the student sees is what will happen.
- The price is the plan's price (`CustomPrice` empty): a price of its own stays something only the team gives.
- Both actions ask first with `ConfirmDialog`: they touch money.

### At the counter (step 2)

- Venta en mostrador offers the plans too. Selling one gives the person the plan from the current month and records the payment of the current month in Cobros (`Payment`), not as an order line, so the month shows as paid and next month is owed as usual.
- A plan is sold on its own (decided by the owner on 2026-11-04): choosing a plan clears the other items, and adding a pack or product clears the plan, so the amount paid never has to be split between an order and a month.
- A person who already has a plan this month changes to the sold plan from this month (decided by the owner on 2026-11-04); changes scheduled for later months are removed, as when choosing from the shop.
- A plan needs a chosen student (a sale opened from the family card has no student yet), and is offered only to members who can record payments (`payments.record`), the permission of `POST /api/students/{id}/plan/sales`. Paying 0 or a part is allowed: the rest is owed in Cobros.

### Nuevo alumno (step 3)

- Once a plan can be chosen in the shop or at the counter, "¿Qué paga?" stops being required: leaving it empty registers the person with no plan, as the import already does.

## Steps

1. **Shop** (built): `IsVisibleInApp` on packs and plans with its migration and the toggle in the pack and plan form; the shop lists the plans; `PUT /api/student-app/students/{studentId}/plan` chooses (`classPackId`) or cancels (`null`) a plan with the rules above; the plan page in the shop with ConfirmDialog for both.
2. **Counter** (built): sell a plan in Venta en mostrador (gives the plan and records the month's payment) through `POST /api/students/{studentId}/plan/sales` (`SellStudentPlanUseCase`).
3. **Nuevo alumno**: "¿Qué paga?" optional.
4. **Team notice** (built): when a student chooses or cancels a plan from the app, the members who see every payment (`payments.view.all`) and the brand owners get an aviso ("Ana eligió el plan X · Desde diciembre.", "Ana dio de baja su plan · Sin plan desde diciembre.") that opens the family card. Choosing what the person already has for that month notifies nobody.

Later, with [online payments](../20260929-online-shop-and-payments/plan.md): a plan maps to a Stripe subscription and is charged automatically every month.

Each step: tests first, `[AuditPlain]`/`[AuditPersonal]` on every text property of the commands, texts in `es.json` and `en.json`, screenshots before and after for the screens that change.
