# Frontend plan — Fee history and class packs (M6)

Backend: [plan](../../backend/20260927-fee-history-and-class-packs/plan.md).

## Screens

| Screen | Change |
|---|---|
| Settings → Cuota mensual | "Desde" month selector (default this month) and the list of changes ("Desde noviembre 2026: $ 30.000") |
| Settings → Packs de clases (new) | Catalog list with active/inactive; form: name, classes, price, validity in months (optional) |
| Family → Cobro (was Cuota) | Current plan ("Cuota general $ 25.000", "Cuota propia", "Paga por clases") and upcoming change; "Cambiar" opens the plan form: kind chips, amount for its own fee, "Desde" month |
| Family → Clases (plan on packs or with purchases) | Available and unpaid classes, each pack with used/left and expiry, "Vender pack" |
| Vender pack (new, `/students/clients/[clientId]/sell-pack`) | Pack chips from the active catalog, price prefilled (editable for discounts), date, method, notes |
| Cuotas | New "Por clases" section: each family on packs with "Quedan 5 clases" / "Debe 2 clases"; "Packs vendidos" total for the month; the "Deben" filter keeps families with unpaid classes |

## Tests (write first)

- Fee change sends the chosen `effectiveFrom`.
- Choosing "Paga por clases" sends `kind: ClassPacks`.
- Selling a pack prefills the catalog price and sends the edited price.
- Family balance shows left and unpaid classes.
- Fees screen lists pack families with their balance.
