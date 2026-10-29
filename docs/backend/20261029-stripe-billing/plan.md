# Backend plan — Paying the plan with Stripe (Europe)

Status: proposal. Decided by the owner on 2026-10-29: prices are **+ IVA**, the plan is paid **only on the web**, and **Stripe Invoicing** issues the invoices. Builds on the [subscriptions plan](../20261003-subscriptions/plan.md), which left "online billing" for later.

## Goal

- A brand owner in Spain (and the rest of the EU) pays `Lite` or `Pro` with a card from Ajustes → Plan, without writing to us, and changes plan, card or cancels by themselves.
- What the brand can do still comes only from its current `Subscription` row: Stripe never decides features. Stripe's webhooks write the same rows the operator writes today.
- Argentina keeps today's flow (contact, operator) until it gets its own provider (Mercado Pago), plugged into the same port.

Out of scope for the first version: `Enterprise` (agreed by hand, the operator keeps setting it), add-ons (extra branches, storage), yearly billing, coupons, our own branded billing emails.

## Decisions

### Stripe Billing on our own account, not Connect

- This is **us charging the brands**, so it runs on the platform's Stripe account with Stripe Billing (Products, Prices, Subscriptions). It has nothing to do with Stripe Connect in the [online payments plan](../20260929-online-shop-and-payments/plan.md), where each branch charges its families on its own account. Both can share the account later; they don't share code beyond the webhook signature check.
- **Stripe Checkout** (hosted page) to subscribe, and the **Customer Portal** (hosted) to change plan, update the card, download invoices and cancel. No card data and no billing screens of our own: less code, PCI SAQ A, 3-D Secure (SCA, required in the EU) handled by Stripe.

### Who, where and in which countries

- Only the **brand** pays, as today: a new brand-only permission `subscription.manage` (brand owners). `subscription.view` stays for seeing the plan.
- **Web only.** Ajustes → Plan shows **Elegir plan** / **Gestionar suscripción** only when `Platform.OS === 'web'`. In the iOS and Android apps the screen stays as today (plans and contact) and says to manage the plan from the web version, without a link, so App Store and Play rules on external payments don't apply. The API doesn't check the platform.
- **Countries:** self-service billing is offered when the brand's country is in the billing countries setting (`Billing:Countries`, Spain and the EU/EEA codes). The brand's country is the country of its first branch (organizations have no country of their own). Any other country sees the contact as today.
- **Currency: EUR** for every Stripe price. A Spanish branch that charges its families in USD still pays us in EUR.

### Prices are + IVA, with Stripe Tax

- List prices (`Lite` 19 €, `Pro` 39 €) are **before VAT**: Stripe prices use `tax_behavior = exclusive` and Checkout runs with `automatic_tax`. A customer in Spain pays 19 € + 21 % = 22,99 €.
- Checkout asks for the **billing address** and offers **tax ID collection** (NIF/CIF, EU VAT number). Stripe Tax applies Spanish VAT to customers in the Spanish VAT area (business or not), the reverse charge to EU businesses with a valid VAT number, and the customer's country VAT to EU consumers (OSS).
- **Canarias** is outside the EU VAT area: Stripe charges no VAT there, and IGIC is not covered by Stripe Tax. The pilot brand is in Tenerife, so the gestoría confirms how B2B services to Canarias are taxed before going live (open decision).
- The app shows "19 € + IVA" next to each paid plan in Ajustes → Plan (new UI text in `es.json` and `en.json`).
- We must register for tax in Stripe Tax (Spain; OSS if selling to EU consumers) before going live. Manual step for the owner in the runbook.

### Invoices: Stripe Invoicing

- Every charge of a subscription produces a Stripe **invoice** with a sequential number (account-level numbering, our prefix), our tax details, the customer's name, address and tax ID, and the VAT breakdown. Stripe emails it and the Customer Portal lists them.
- The owner's legal and tax details (name or company name, NIF, address) are set once in Stripe's account settings and appear on every invoice.
- **VeriFactu (open):** invoicing software in Spain must comply from 2027. Before then we confirm Stripe's VeriFactu support, or send Stripe's invoices to software that complies. It does not block the first version.
- Billing emails (receipt, invoice, failed payment, card expiring) are **Stripe's own**, in the customer's language (`preferred_locales` from the brand owner's `PreferredLanguage`). Branded billing emails in our layout are a later step.

### Trial and timing

- Sign-up doesn't change: no card, `Free` for 30 days.
- **Subscribing during the trial:** the paid plan applies at once (the brand gets `Pro` features the same day) and the first charge happens when the trial would have ended (`subscription_data.trial_end`), so nobody loses trial days by paying early. Checkout still collects the card.
- **Subscribing after the trial ended** (brand read-only): the first charge is immediate and the brand writes again as soon as the webhook arrives.
- Monthly, charged on the same day of the month; Stripe prorates changes between `Lite` and `Pro` through the portal (upgrade charged now, downgrade credited to the next invoice).

### Stripe writes the same `Subscription` rows the operator writes

The [records convention](../../../CLAUDE.md#records-that-change-over-time) already models a plan change; Stripe events map onto it:

| Stripe | Our rows |
|---|---|
| `checkout.session.completed` (mode `subscription`) | `Delete(now)` the current subscription (the trial) and start the paid one: plan from the price's lookup key, `Price` = the Stripe price before tax, `Currency` = `EUR`, no `ExpiredOn`, `ExternalSubscriptionId` = Stripe's id |
| `customer.subscription.updated`, other price | Replace the row with the new plan and price, same `ExternalSubscriptionId` |
| `customer.subscription.updated`, `cancel_at_period_end` | Set `ExpiredOn` to the last day of the paid period; clearing it (the customer resumes) clears `ExpiredOn` |
| `customer.subscription.updated`, `past_due` | `PaymentStatus = PastDue`: the brand keeps working while Stripe retries (Smart Retries, about 2 weeks), and Inicio and Ajustes → Plan show "No pudimos cobrar tu plan" with **Actualizar tarjeta** for the brand owner |
| `invoice.paid` | `PaymentStatus = Current` |
| `customer.subscription.deleted` (cancelled, or retries exhausted) | `ExpiredOn` = today if not set: the brand turns read-only, as an ended trial does today |

- `ExpiredOn` and `IsDeleted` keep deciding whether a subscription is active. `PaymentStatus` only drives the notice; it never removes features.
- Stripe sends events out of order and more than once: every event id is stored once (`ProcessedBillingEvents`), and each handler reads the **current Stripe subscription** (`GET /v1/subscriptions/{id}`) instead of trusting the event payload, so applying an old event twice converges to Stripe's state.
- **The webhook confirms, never the return page.** The success URL only shows "Procesando el pago…" in Ajustes → Plan and reloads `GET /api/me` until the plan changes.
- A paid row is tied to Stripe (`ExternalSubscriptionId` not null). The backoffice shows the Stripe status and a link to the customer in the Stripe dashboard, and **refuses Cambiar plan** for it (`brand.subscription_external`): plan changes of a paying brand go through Stripe, or the rows and the charges disagree. Giving add-ons by hand keeps working.

### Mapping plans to Stripe prices

- Prices are found by **lookup key**, `{planCode}-monthly-eur` (`lite-monthly-eur`, `pro-monthly-eur`), so no Stripe ids live in code, settings or migrations, and test and live mode use the same keys.
- The webhook takes the plan code from the lookup key of the subscription's price; a price without a known lookup key is logged as an error and changes nothing.
- `Plan.ListPrice` stays the price the app shows. A startup check is not possible without calling Stripe, so the runbook says to change both together; a mismatch only affects what is shown, the charge is always Stripe's.

## Model changes

| Table (`billing` schema) | Change |
|---|---|
| `BillingCustomers` (new) | `SubscriberId` (PK, the organization) → `ExternalCustomerId` (Stripe `cus_…`). Created on the first checkout, reused for every later one and for the portal, so a brand has one Stripe customer |
| `Subscriptions` | `ExternalSubscriptionId` (nullable, `sub_…`, filtered unique index on current rows), `PaymentStatus` (nullable: `Current`, `PastDue`; null for rows set by hand) |
| `ProcessedBillingEvents` (new) | `EventId` (PK), `ProcessedAt`. Physical rows with no business meaning; kept 90 days |

None of these are tenant-owned, like the rest of the `billing` schema.

## Code

Same split as the rest of the subscriptions work ([library or part of Core](../20261003-subscriptions/plan.md#library-or-part-of-core)):

| Project | What |
|---|---|
| `src/Subscriptions` | The port `IBillingGateway` (create checkout, create portal session, get subscription) and the provider-neutral records it returns (`ExternalSubscription`: id, customer, lookup key, unit amount, status, period end, cancel at period end). The rules that turn an `ExternalSubscription` into row changes (`ExternalSubscriptionSync`), unit tested without Stripe |
| `src/Subscriptions.Stripe` (new, ARCH rule like ARCH007: may only reference `Subscriptions`) | `StripeBillingGateway` with the official `Stripe.net` package; webhook signature check (`EventUtility.ConstructEvent`); `AddStripeBilling()` with options `Stripe:SecretKey`, `Stripe:WebhookSecret` |
| `src/Subscriptions.AspNetCore` | Model of the new tables and columns; `MapBillingWebhook()` (anonymous, `.AllowInactiveSubscription()`) that verifies, deduplicates and calls the sync |
| App (`Core`, `Api`) | `StartPlanCheckoutUseCase` and `OpenBillingPortalUseCase` (commands, `subscription.manage`), the brand's country check, the customer's email and language, the return URLs |

Argentina later is a second implementation of `IBillingGateway` (Mercado Pago `preapproval`) chosen by the brand's country; the sync and the rows don't change.

## API

| Operation | Endpoint | Who |
|---|---|---|
| Start a checkout for a plan (`{ planCode }`), answers `{ url }` | `POST /api/organization/subscription/checkout` | `subscription.manage` |
| Open the Customer Portal, answers `{ url }` | `POST /api/organization/subscription/portal` | `subscription.manage`, brand with a billing customer |
| Stripe webhooks | `POST /api/billing/stripe/webhooks` | anonymous, signature checked |
| Whether self-service billing applies, payment status | added to `GET /api/organization/subscription` (`selfService`, `paymentStatus`, `isExternal`) | `subscription.view` |

Both `POST` endpoints allow an inactive subscription (a read-only brand must be able to pay). Checkout refuses `enterprise`, `free`, a plan the brand already pays through Stripe (it goes to the portal) and a country outside `Billing:Countries` (`billing.not_available`).

## In the app (web)

- Ajustes → Plan, brand owners on the web in a billing country: each paid plan shows its price "+ IVA" and **Elegir plan**; a brand already paying through Stripe sees **Gestionar suscripción** (portal) instead. `Enterprise` keeps the contact.
- The ended-subscription gate and `PlanOfferSheet` **Ver planes** keep going to Ajustes → Plan, where the button is.
- Return from Checkout: `?checkout=success` shows "Procesando el pago…" and reloads `GET /api/me` until the plan changes (a short limit, then "Puede tardar unos minutos"); `?checkout=cancelled` shows nothing special.
- Past due: a notice on Inicio and Ajustes → Plan with **Actualizar tarjeta** (portal) for brand owners, and nothing for the rest of the team.
- Native apps: no payment buttons; one line saying the plan is managed from the web version.

All new texts go in `es.json` and `en.json`; the screens use the existing `src/ui` primitives (`Button`, banners, skeletons).

## Security and operations

- Keys: a **restricted key** with only the Billing, Checkout, Customer Portal and Customers permissions it needs, and the webhook signing secret, in the App Service settings like the other secrets; never in the repository. Test mode keys in development.
- `api.stripe.com` is not reachable from the cloud development environment's network policy; tests never call Stripe (fake gateway, signed fake events), and Stripe is tried by hand in test mode with the Stripe CLI (`stripe listen --forward-to`).
- A webhook that fails answers `500` so Stripe retries for up to 3 days; Sentry reports it.
- Runbook (written in step 1): create the Stripe account (owner's legal entity), business details for invoices, invoice numbering and prefix, Stripe Tax registration, products and prices with their lookup keys, Customer Portal settings (allow switching between `Lite` and `Pro`, cancel at period end, update card, invoice history), Smart Retries and the emails Stripe sends, the webhook endpoint and its events, the keys. Linked from `README.md`.
- Terms of sale for the plans (B2B, no withdrawal right, renewal, cancellation) go in the legal texts before going live ([legal and privacy](../../legal-and-privacy.md)).

## Testing

- Unit (`Subscriptions.U.Tests`): every row in the event table above, out-of-order and repeated events converge, an unknown lookup key changes nothing.
- Integration (`Api.I.Tests`, fake gateway): a signed fake `checkout.session.completed` turns the trial into `Pro` and the brand gets `Pro` features; the same event twice changes nothing; a bad signature is `400`; an event for brand A never touches brand B; `subscription.deleted` makes the brand read-only; checkout is refused to a branch owner, for `enterprise` and outside the billing countries; the backoffice refuses **Cambiar plan** for a brand paying through Stripe.
- App (jest): the buttons appear only on the web, only for brand owners in a billing country; the success return polls `GET /api/me`; past due shows the notice.
- No e2e against Stripe: the hosted pages are Stripe's, and the critical part (the webhook) is covered by integration tests.

## Steps

1. **Stripe account and runbook** (owner, by hand, test mode first): account, tax registration, products and prices with lookup keys, portal, webhook. The runbook in `docs/`.
2. **Model and sync:** new tables and columns with their migration, `IBillingGateway`, `ExternalSubscriptionSync` with its unit tests.
3. **Stripe and webhook:** `Subscriptions.Stripe`, `POST /api/billing/stripe/webhooks` with signature check and idempotency; integration tests with signed fake events.
4. **Checkout and portal:** `subscription.manage`, the two endpoints, the country check, `GET /api/organization/subscription` additions.
5. **App:** Ajustes → Plan on the web, return page, past-due notice, native apps message; screenshots before and after for the owner.
6. **Backoffice:** Stripe status and dashboard link on the brand, **Cambiar plan** refused for paying brands.
7. **Go live:** live keys, live webhook, terms of sale, one real subscription paid and cancelled by the owner.

## Open decisions

| Decision | Proposal |
|---|---|
| Customers in Canarias (IGIC) | Ask the gestoría; Stripe Tax collects nothing there |
| VeriFactu (2027) | Check Stripe's support before 2027; otherwise export Stripe invoices to compliant software |
| Yearly billing | Later: a `{plan}-yearly-eur` price and `BillingPeriod = Yearly`, two months free |
| Add-ons (extra branch 20 €, storage 2 €/10 GB) through Stripe | Later: subscription items with their own lookup keys, mapped to `SubscriptionFeature` |
| `Enterprise` through Stripe | Later: the operator creates a Stripe subscription with an agreed price from the backoffice; until then it is set by hand |
| Argentina | Mercado Pago subscriptions behind `IBillingGateway`, prices in ARS, when Spain is live |
