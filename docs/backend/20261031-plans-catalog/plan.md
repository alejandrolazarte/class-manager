# Plans catalog: monthly plans and class packs in one place

Date: 2026-10-31

Status: proposal, agreed with the owner in conversation; nothing built yet. Builds on [default billing mode](../20261020-default-billing-mode/plan.md) (every family has its billing plan written down since #325 and #326), [fee history and class packs](../20260927-fee-history-and-class-packs/plan.md) and [class pack coverage](../20261028-class-pack-coverage/plan.md).

## Why

Today a business charges in two unrelated ways:

- **A monthly fee**: one amount in Ajustes › Forma de cobro (`DefaultMonthlyFeeChanges`), a switch between "cuota mensual" and "packs de clases" for the whole business, and per family "Cuota general", "Cuota propia" or "Paga por clases" (`ClientBillingPlanChanges`). One fee per family.
- **Class packs**: a catalog (`ClassPacks`: name, classes, price, validity, which classes) that families buy (`ClassPackPurchases`, price frozen at purchase).

That split keeps raising questions with no good answer: does switching the business to packs move every family, does a new fee amount apply to everybody or only to new students, what is a family's "cuota propia". Other apps for studios don't have a business-wide billing mode: the business sells products (memberships and packs) and each person has the ones they bought.

The owner's observation: **everything is a pack**. A monthly fee is "unlimited classes for a month, renewed every month"; "2 veces por semana" is "8 classes a month, renewed every month"; a 4-class pack is "4 classes, valid one month, not renewed". The word *cuota* is also ambiguous in Spanish: it means both the monthly fee and paying something in instalments, which are independent things.

## Decisions

### One catalog: every item is a pack with a few properties

| Property | Values | Today |
|---|---|---|
| Classes | a number, or **libre** (unlimited) | number only |
| Which classes | any group class, or some (`ClassPackClassGroups`), private lessons by duration | exists |
| Validity | months | exists (`ValidityMonths`) |
| **Renews every month** | yes (what we call a cuota today) / no (bought once) | new |
| Price | for renewing items, effective from a month (price history) | single price |
| Offered | active or not: an item no longer offered stays with whoever has it | exists (`IsActive`) |

- The catalog stays the `ClassPack` entity, extended: `ClassCount` becomes nullable (null = libre), `RenewsMonthly` is added, and renewing items get a price history (`ClassPackPriceChanges`: `EffectiveFrom` month and `Price`, a business date like today's fee changes). Non-renewing items keep a single price: a new price applies to the next purchases and never to packs already bought.
- UI name: open decision (see the end). Not "Planes", which is already Ajustes › Plan (the business's subscription to the app).
- When the business creates its first item, the screen offers two shortcuts: **Cuota mensual libre** (libre, renews monthly) and **Pack de clases** (N classes, one month, doesn't renew), so a small instructor doesn't need to understand every property to start.

### Each person has their own items

- A renewing item is held **per person** (student), not per family: `StudentPlans` (`StudentId`, `ClientId`, `ClassPackId`, optional `CustomPrice`, `CreatedOn` month it applies from, `ExpiredOn` optional). Changing someone's plan follows the records convention: `Delete(now)` on the current row and add the new one in one save (`ISoftDeletable`, `ICreatedOn`, `IExpiredOn`; active = `IsActiveOn(date)`).
- Non-renewing packs stay bought by the family as today (`ClassPackPurchases`), used by whichever student attends, with the coverage rules already built.
- A person can have a renewing item **and** buy packs (for extra classes or other classes): attendance is paid first by the renewing item when it covers that class and has classes left that month, then by packs as today.
- A renewing item with N classes gives N classes each month, valid until the end of that month (they don't carry over). A libre item doesn't count classes.
- **CustomPrice** replaces today's "Cuota propia": a price for that person only (a scholarship, an agreement). The catalog price doesn't apply to them while it is set.

### Raising a price: no special option

- Raising the price of a renewing item from a month applies to **everyone who has it** from that month. That's the usual inflation raise.
- To charge more **only to new students**, the instructor creates a new item with the new price and stops offering the old one: whoever had the old one keeps it and its price.
- Taking an item out of the catalog never removes it from the people who have it; moving them to another item is an explicit action on each person (later, a bulk "Pasar a …" with a confirm dialog).

This replaces the "¿A quién se aplica?" question discussed for the fee amount.

### Family discounts

- The business sets **family discount tiers** in Ajustes: for example "2 personas: 10 %", "3 o más: 20 %" (`FamilyDiscountTiers`: minimum people, percent; effective from a month like prices).
- In a month, the tier is chosen by how many people of the family have an active renewing item that month, and the percent applies to each of them. A sibling who leaves changes the tier from the next month.
- Each item says whether family discounts apply to it (`AppliesFamilyDiscount`, on by default for renewing items, off for packs).
- A person with a `CustomPrice` gets no tier discount on top: the custom price is the final price.
- A couple is one family (one contact) with two people who attend, so it falls in "2 personas". Shared items ("Plan pareja", one item covering two people) are left out: they complicate attendance and class balances, and the tiers plus custom prices cover the cases we know.

### What a family owes each month

- The family still pays **one amount per month** and payments stay as today (`Payment`: client, month, amount). What changes is how the amount due is computed: the sum, over the family's people, of the price of their renewing item that month (price history or `CustomPrice`) minus the family discount.
- Cobros keeps its two lists: families with renewing items (owed, paid, balance, status as `FeeRules` does today) and families paying per class (classes available and owed, as today). The family card shows one line per person:

  > Tomás · Libre · 25.000 − 10 %
  > Lucía · Libre · 25.000 − 10 %
  > **Total del mes: 45.000**

- A renewing item creates a debt every month, whether the person attends or not, as the fee does today. A pack never does.

### New students

- Nuevo alumno asks, for each person who attends, **what they pay**: one of the renewing items offered, or "Con packs" (nothing renewing; they buy packs). One item can be marked as the default and comes preselected. This replaces the "¿Cómo cobrás?" question added in #326 and Ajustes › Forma de cobro.

### Paying in parts and online

- Paying an item in parts (a 24-class yearly pack in 3 instalments) is a way of paying, not a property of the catalog. It is left for when families pay online ([online payments plan](../20260929-online-shop-and-payments/plan.md), Stripe Connect). Then a renewing item maps to a subscription, a pack to a one-off payment, and a pack in parts to an instalment plan. Nothing in this plan depends on it.

## Migration from today

One data migration, run when step 4 ships, so nobody pays a different amount the day after:

| Today | Becomes |
|---|---|
| The business's fee (`DefaultMonthlyFeeChanges`, mode `MonthlyFee`) | A catalog item "Cuota mensual": libre, renews monthly, any group class, with the same price history |
| A family on "Cuota general" | Each of its students gets "Cuota mensual" from the current month |
| A family on "Cuota propia" | Each of its students gets "Cuota mensual" with a `CustomPrice` |
| A family on "Paga por clases" | No renewing item; its packs stay as they are |
| Packs and purchases | Unchanged |
| Payments | Unchanged (still per family and month) |

Today a family pays **one** fee whatever the number of students, and per person it would pay one per student. To keep every family's amount the same, the migration gives the first student (oldest registration) the family's amount and the other students a `CustomPrice` of 0, shown as "Incluido en la cuota de la familia" until the instructor changes it. Open decision below.

After the migration `BusinessBillingMode`, `DefaultMonthlyFeeChanges`, `ClientBillingPlanChanges`, Ajustes › Forma de cobro, "Cuota general/propia" and their texts, tests and endpoints are deleted (a redesign replaces, it doesn't pile up).

## Steps

1. **Catalog**: `ClassCount` nullable (libre), `RenewsMonthly`, `AppliesFamilyDiscount`, price history for renewing items. The catalog screen (today Ajustes › Packs) shows both kinds, with the two shortcuts. Packs keep working exactly as today.
2. **Plans per person**: `StudentPlans`, assigning and changing a person's item from the student card, attendance paid by the renewing item first, the amount due per family computed from its people. Cobros and the family card read it. Behind the existing fee screens until step 4.
3. **Family discount tiers**: Ajustes screen and the computation, with the per-person lines on the family card.
4. **Migration and removal**: the data migration above; Nuevo alumno asks what each person pays; delete the billing mode, default fee, family plans and their screens.
5. **Later, when asked**: a weekly limit or a warning for items like "2 veces por semana", moving many people to another item at once, paying in parts and online with Stripe.

Each step: tests first, tenancy test for every new tenant-owned table, `[AuditPlain]`/`[AuditPersonal]` on every text property of the commands, texts in `es.json` and `en.json`, screenshots before and after for the screens that change.

## Open decisions

- **Name in the app** for the catalog: "Planes y packs", "Abonos y packs", "Lo que vendés"…
- **Families with several students at migration**: first student pays the family amount and the others are "incluido" at 0 (proposed), or every student gets the full price and the instructor fixes it (amounts change), or the migration asks the instructor in the app before applying.
- **Order of payment** when a person has a renewing item with N classes and also packs: renewing item first (proposed), or the one that expires first.
- **Family discount on packs**: off by default and set per item (proposed), or never.
