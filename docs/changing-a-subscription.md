# Changing a brand's subscription

Date: 2026-10-11

There is no platform admin screen yet (see the [subscriptions plan](backend/20261003-subscriptions/plan.md#api)), so the operator changes a brand's plan and price with `scripts/set-subscription.cs`, run from their own PC against the database.

## What it does

- Finds the brand (organization) by its **brand owner's email** or by its **organization id**. If the email owns several brands, it lists them and asks for the id.
- In one transaction, ends the current subscription (`DeletedOn = now`) and starts the new one with the given plan, price, currency and note, following the [records convention](../CLAUDE.md#records-that-change-over-time). No row is deleted, so the history stays.
- The new subscription has no `ExpiredOn`: it lasts until the next change. That includes `free`, which only expires after 30 days when it is created by sign-up.
- Add-ons belong to the previous subscription and don't carry over.

## Running it

The connection string is read from `BUSINESS_DATABASE`, never from an argument, so it doesn't end up in the shell history. Against Azure SQL, sign in with `az login` and use `Authentication=Active Directory Default` (no password); the database firewall must allow your IP while you run it (Azure portal → SQL server → Networking), and remove the rule afterwards.

```powershell
$env:BUSINESS_DATABASE = "Server=tcp:<server>.database.windows.net,1433;Database=<database>;Authentication=Active Directory Default;Encrypt=True"
dotnet run scripts/set-subscription.cs -- <brand owner email | organization id> <plan> <price> [currency] [note]
```

- `plan`: a code in `billing.Plans` (`free`, `lite`, `pro`, `enterprise`).
- `price`: agreed monthly price, with a dot for decimals (`0`, `19.50`).
- `currency`: ISO 4217 code; `EUR` when omitted or passed as `""`.
- `note`: why this price, up to 200 characters.

Examples:

```powershell
dotnet run scripts/set-subscription.cs -- owner@example.com enterprise 0 "" "Own business"
dotnet run scripts/set-subscription.cs -- owner@example.com pro 19
```

After the change, the brand's members see the new plan when the app reloads `GET /api/me` (reload the app or sign in again).

## Your own business

Sign up from the app like any customer, then give the brand `enterprise` at 0 with this script. Don't add your business, your account or your email to a migration or a seed script: migrations run in every environment (development, tests, e2e), and the repository is public.
