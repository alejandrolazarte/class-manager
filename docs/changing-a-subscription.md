# Changing a brand's subscription

Date: 2026-10-11

A platform operator changes a brand's plan and price, and gives or removes add-ons, from the backoffice at `admin.laztty.com`: **Marcas** → the brand → **Cambiar plan** or **Dar add-on** (see the [backoffice plan](backend/20261017-backoffice/plan.md)). Both follow the same rules as the script below.

[`scripts/set-subscription.sql`](../scripts/set-subscription.sql) stays as the fallback when the backoffice is not reachable: it runs in the Azure portal and needs nothing installed.

## What it does

- Finds the brand (organization) by its **brand owner's email**. It stops with an error if the email owns no brand or more than one, or if the plan doesn't exist.
- In one transaction, ends the current subscription (`IsDeleted = 1`, `DeletedAt = now`) and starts the new one with the given plan, price, currency and note, following the [records convention](../CLAUDE.md#records-that-change-over-time). No row is deleted, so the history stays.
- The new subscription has no `ExpiredOn`: it lasts until the next change. That includes `free`, which only expires after 30 days when it is created by sign-up.
- Add-ons belong to the previous subscription and don't carry over.
- Ends by listing the brand's subscriptions: the new one is the row with `IsDeleted = 0`.

## Running it

1. Azure portal → the SQL database → **Query editor**, and sign in with your Microsoft Entra account. If it says your IP is not allowed, use the link it shows to add it (or SQL server → **Networking**), and remove that rule when you finish.
2. Paste the whole script and fill in the five values at the top:
   - `@OwnerEmail`: the email the brand owner signs in with.
   - `@PlanCode`: `free`, `lite`, `pro` or `enterprise`.
   - `@Price`: agreed monthly price (`0`, `19.50`).
   - `@Currency`: ISO 4217 code, `EUR` by default.
   - `@Note`: why this price, up to 200 characters.
3. **Run**. The brand's members see the new plan when the app reloads `GET /api/me` (reload the app or sign in again).

## Your own business

Sign up from the app like any customer, then give the brand `enterprise` at 0 with this script. Don't add your business, your account or your email to a migration or a seed script: migrations run in every environment (development, tests, e2e), and the repository is public.
