# Backend plan — Paying the plan with Stripe (Europe)

Status: steps 2 to 6 built (model, sync, the Stripe webhook, checkout and portal endpoints, Ajustes → Plan on the web, the backoffice and plan offers), the rest is a proposal. Decided by the owner on 2026-10-29: prices are **+ IVA**, the plan is paid **only on the web**, and **Stripe Invoicing** issues the invoices, and paying online starts as an **add-on** given to one brand at a time for testing, until it is a feature of every plan. Builds on the [subscriptions plan](../20261003-subscriptions/plan.md), which left "online billing" for later.

## Goal

- A brand owner in Spain (and the rest of the EU) pays `Lite` or `Pro` with a card from Ajustes → Plan, without writing to us, and changes plan, card or cancels by themselves.
- What the brand can do still comes only from its current `Subscription` row: Stripe never decides features. Stripe's webhooks write the same rows the operator writes today.
- Argentina keeps today's flow (contact, operator) until it gets its own provider (Mercado Pago), plugged into the same port.

Out of scope for the first version: `Enterprise` (agreed by hand, the operator keeps setting it), add-ons (extra branches, storage), yearly billing, coupons, our own branded billing emails.

## Decisions

### Stripe Billing on our own account, not Connect

- This is **us charging the brands**, so it runs on the platform's Stripe account with Stripe Billing (Products, Prices, Subscriptions). It has nothing to do with Stripe Connect in the [online payments plan](../20260929-online-shop-and-payments/plan.md), where each branch charges its families on its own account. Both can share the account later; they don't share code beyond the webhook signature check.
- **Stripe Checkout** (hosted page) to subscribe, and the **Customer Portal** (hosted) to change plan, update the card, download invoices and cancel. No card data and no billing screens of our own: less code, PCI SAQ A, 3-D Secure (SCA, required in the EU) handled by Stripe.

### An add-on first, a feature of every plan later

- Paying online is a feature, `self-service-billing`, added to the `Features` class and the catalog as an **add-on at 0 €** that **no plan includes**. While we try it, the operator gives it to one brand with **Dar add-on** in the backoffice (an `ExpiredOn` is optional), and only that brand sees **Elegir plan**. Every other brand keeps the contact, as today.
- The feature gates **starting a checkout** only: `RequireFeature(self-service-billing)` on the checkout endpoint and `useFeature('self-service-billing')` in Ajustes → Plan. A brand that already pays through Stripe (its current subscription has an `ExternalSubscriptionId`) always reaches the portal, and the webhook always applies, with or without the add-on. That matters because add-ons belong to a subscription and don't carry over: when the checkout replaces the trial with the paid plan, the add-on stays behind on the trial row, and the brand must still be able to change its card or cancel. A pilot brand that cancels and wants to pay again needs the add-on again.
- **Making it a feature for everyone** is data, not code: a migration adds `self-service-billing` to the `Free`, `Lite` and `Pro` plans (`PlanFeature`). The add-on rows given during the tests can stay; they change nothing once the plan includes the feature. `Enterprise` doesn't get it, since its price is agreed by hand.
- Without the add-on, `GET /api/organization/subscription` answers `selfService = false` and the app shows today's screen, so a brand outside the test never sees a half-built flow.

### Who, where and in which countries

- Only the **brand** pays, as today: a new brand-only permission `subscription.manage` (brand owners). `subscription.view` stays for seeing the plan.
- **Web only.** Ajustes → Plan shows **Elegir plan** / **Gestionar suscripción** only when `Platform.OS === 'web'`. In the iOS and Android apps the screen stays as today (plans and contact) and says to manage the plan from the web version, without a link, so App Store and Play rules on external payments don't apply. The API doesn't check the platform.
- **Countries:** self-service billing is offered when the brand's country is in `SelfServiceBilling.CountryCodes` (Core): Spain and the rest of the EU and the EEA. It is a constant, not a setting, because adding a country also means checking its taxes in Stripe Tax; Argentina comes with its own provider. The brand's country is the country of its first branch (organizations have no country of their own). Any other country sees the contact as today.
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
- **Subscribing during the trial:** the paid plan applies at once (the brand gets `Pro` features the same day) and the first charge happens when the trial would have ended (`subscription_data.trial_end`), so nobody loses trial days by paying early. Checkout still collects the card. Stripe Checkout needs a trial of at least 48 hours, so in the trial's last two days the first charge is immediate.
- **Subscribing after the trial ended** (brand read-only): the first charge is immediate and the brand writes again as soon as the webhook arrives.
- Monthly, charged on the same day of the month; Stripe prorates changes between `Lite` and `Pro` through the portal (upgrade charged now, downgrade credited to the next invoice).

### Stripe writes the same `Subscription` rows the operator writes

The [records convention](../../../CLAUDE.md#records-that-change-over-time) already models a plan change; Stripe events map onto it:

| Stripe | Our rows |
|---|---|
| `checkout.session.completed` (mode `subscription`) | `Delete(now)` the current subscription (the trial) and start the paid one: plan from the `plan_code` metadata of the price's product, `Price` = the Stripe price before tax, `Currency` = `EUR`, no `ExpiredOn`, `ExternalSubscriptionId` = Stripe's id |
| `customer.subscription.updated`, other price | Replace the row with the new plan and price, same `ExternalSubscriptionId` |
| `customer.subscription.updated`, `cancel_at_period_end` | Set `ExpiredOn` to the last day of the paid period; clearing it (the customer resumes) clears `ExpiredOn` |
| `customer.subscription.updated`, `past_due` | `PaymentStatus = PastDue`: the brand keeps working while Stripe retries (Smart Retries, about 2 weeks), and Inicio and Ajustes → Plan show "No pudimos cobrar tu plan" with **Actualizar tarjeta** for the brand owner |
| `invoice.paid` | `PaymentStatus = Current` |
| `customer.subscription.deleted` (cancelled, or retries exhausted) | `ExpiredOn` = today if not set: the brand turns read-only, as an ended trial does today |

- `ExpiredOn` and `IsDeleted` keep deciding whether a subscription is active. `PaymentStatus` only drives the notice; it never removes features.
- Stripe sends events out of order and more than once: every event id is stored once (`ProcessedBillingEvents`), and each handler reads the **current Stripe subscription** (`GET /v1/subscriptions/{id}`) instead of trusting the event payload, so applying an old event twice converges to Stripe's state.
- **The webhook confirms, never the return page.** The success URL only shows "Procesando el pago…" in Ajustes → Plan and reloads `GET /api/me` until the plan changes.
- A paid row is tied to Stripe (`ExternalSubscriptionId` not null). The backoffice shows the Stripe status and a link to the customer in the Stripe dashboard, and **refuses Cambiar plan** for it (`backoffice.subscription_external`): plan changes of a paying brand go through Stripe, or the rows and the charges disagree. Giving add-ons by hand keeps working.

### Offering a plan: a payment link, and the plan changes only once paid

Decided with the owner on 2026-10-30. **Cambiar plan** in the backoffice stays a manual change: the brand gets the plan at once, nobody is charged, and the price is only recorded (pilots, `Enterprise` paid by bank transfer, fixing a mistake). To charge a brand a price agreed by hand, the operator **offers** the plan instead:

- **Ofrecer plan** (backoffice, on the brand): plan (`lite` or `pro`), monthly price before VAT, an optional last day and a note. It creates a `PlanOffer` for the brand and replaces its current offer, if any. The brand's subscription doesn't change: it keeps its trial or its plan until it pays.
- **Copiar link de pago**: the backoffice copies a link to the app, `{WebApp:Url}/settings/plan?offer=1`, for the operator to send by WhatsApp or email. Not a Stripe link: Checkout sessions expire after 24 hours, and the app link only works for a member of that brand with `subscription.manage`, so the payment can't end up on another brand.
- **In the app (web):** Ajustes → Plan shows the offer above the plans, "Te ofrecemos Lite · 10 € por mes + IVA" (and "hasta el {date}" when it has a last day), with a button that starts the checkout at the offered price. In the native apps, the offer shows with the line that says to pay from the web version.
- **The offer is its own permission to pay online:** it doesn't need the `self-service-billing` add-on (the operator offering it is the permission), but the brand's country must still be in the billing countries, because Stripe Tax is only set up for them.
- **Charging:** the checkout for an offered plan uses the offer's price (+ IVA, like every price), with the same trial rule: during the trial, the first charge is when the trial ends. The price is found or created in Stripe like any other.
- **The plan changes only when Stripe confirms the payment:** the webhook starts the paid subscription as today, and then marks the offer as used (`Delete(now)`). An offer whose last day passed is not shown and can't be paid; the checkout of that plan goes back to the list price.
- **Retirar oferta** deletes the current offer (soft delete, so it can be restored by offering it again).
- Replaces the earlier rule "a brand with an operator-set price for the same plan is charged that price": with offers, an agreed price is always an explicit offer, and a manual **Cambiar plan** never turns into a charge.

`PlanOffer` (billing schema, in `src/Subscriptions`): `Id`, `SubscriberId`, `PlanCode`, `Price`, `Currency`, `Note`, `CreatedOn`, `ExpiredOn` (optional, inclusive), `IsDeleted`, `DeletedAt`, `DeletedByUserId`; a filtered unique index keeps one current offer per brand. `GET /api/organization/subscription` adds `offer` (plan, price, currency, last day) when there is an active one.

### Products and prices are created by the app

Nobody creates products or prices by hand in Stripe: our database is the catalog, and the app makes Stripe match it when it needs to.

- **One product per plan**, with an id we choose (`plan-lite`, `plan-pro`) so the app finds it without storing Stripe ids, and with the metadata **`plan_code`** (`lite`, `pro`). The name ("Laztty Pro") is only for Stripe's dashboard and the invoices; it can change without breaking anything. The product carries the tax code for software as a service for business use (`txcd_10103001`), which Stripe Tax needs.
- **Prices are found or created by amount.** Before a checkout, the app looks among the product's active prices for one with the same amount, `EUR`, monthly and tax exclusive; it reuses it, or creates it (with `plan_code` in its metadata too). Stripe prices can't change, so a new amount is always a new price; old prices stay for the brands that already pay them.
- **The amount comes from our database:** the plan's `ListPrice`, or the price of an active offer for that plan (see [Offering a plan](#offering-a-plan-a-payment-link-and-the-plan-changes-only-once-paid)). Since step 6, a price set by hand with **Cambiar plan** is never charged: an agreed price is always an offer. Any other amount is just another price of the same product.
- **The webhook reads the plan from `product.metadata.plan_code`** (the subscription is read with its prices' products expanded), never from names or price ids, so agreed prices and price changes need nothing extra. A product without a known `plan_code` is logged and changes nothing. The plan doesn't come from the subscription's metadata, because switching plan in the portal changes the price, not that metadata.
- **The Customer Portal configuration is ours too:** every time a brand opens the portal, the app updates its own configuration (marked with the metadata `managed_by = subscriptions`) with the `lite` and `pro` list prices, switching with proration, cancelling at period end, card update and invoice history. A new list price in the database reaches the portal on the next opening, with no step in Stripe.
- Only the app's key writes to the catalog. Editing these products' metadata in Stripe's dashboard would change what the webhook reads, so the runbook says not to touch them there.
- `Plan.ListPrice` stays the price the app shows, and now it is also what Stripe charges new subscribers.

## Model changes

| Table (`billing` schema) | Change |
|---|---|
| `BillingCustomers` (new) | `SubscriberId` (PK, the organization) → `ExternalCustomerId` (Stripe `cus_…`). Created on the first checkout with the brand owner's name, email and language, reused for every later one and for the portal, so a brand has one Stripe customer. If creating the checkout fails right after creating the customer, the row is rolled back and the next attempt creates another Stripe customer; the unused one stays in Stripe and costs nothing |
| `Subscriptions` | `ExternalSubscriptionId` (nullable, `sub_…`, filtered unique index on current rows), `PaymentStatus` (nullable: `Current`, `PastDue`; null for rows set by hand) |
| `ProcessedBillingEvents` (new) | `EventId` (PK), `ProcessedAt`. Physical rows with no business meaning; a cleanup of rows older than 90 days comes later, they are a few per brand and month |

None of these are tenant-owned, like the rest of the `billing` schema.

## Code

Same split as the rest of the subscriptions work ([library or part of Core](../20261003-subscriptions/plan.md#library-or-part-of-core)):

| Project | What |
|---|---|
| `src/Subscriptions` | The port `IBillingGateway` (get a subscription, create a customer, a checkout and a portal session) and the provider-neutral records (`ExternalSubscription`: id, subscriber from the metadata `subscriber_id`, plan code from the product's metadata `plan_code`, unit amount, currency, status, last day before cancelling, day it ended; `BillingEvent`: event id and subscription id).; `BillingPlanPrice` (plan code, product name, amount, currency) for checkouts and the portal. The rules that turn an `ExternalSubscription` into row changes (`ExternalSubscriptionSync`), unit tested without Stripe |
| `src/Subscriptions.Stripe` (new; ARCH007 already covers it, so it can't use the app) | `StripeBillingGateway` with the official `Stripe.net` package and `StripePlanCatalog`, which finds or creates products, prices and the portal configuration; `MapStripeBillingWebhook(pattern)`, which checks the signature (`EventUtility.ConstructEvent`, without failing on another API version, since only ids are read and the subscription is fetched again) and hands the event to `IBillingWebhookProcessor`; `AddStripeBilling()` with options `Stripe:SecretKey`, `Stripe:WebhookSecret`. Without `SecretKey` the gateway is a placeholder that fails when used, so development and CI run without Stripe |
| `src/Subscriptions.AspNetCore` | Model of the new tables and columns; `IBillingWebhookProcessor`, which deduplicates by event id, reads the subscription through `IBillingGateway`, applies the sync and saves in one `SaveChanges` |
| App (`Core`, `Api`) | `StartPlanCheckoutUseCase` and `OpenBillingPortalUseCase` (commands, `subscription.manage`); `ISelfServiceBillingService`, which says whether the brand can pay online (the add-on, read from its current subscription even when it has ended, and the country), used by checkout and by `GET /api/organization/subscription`; the return URLs from `IWebAppLinks` (`/settings/plan`, `?checkout=success` or `cancelled`) |

Argentina later is a second implementation of `IBillingGateway` (Mercado Pago `preapproval`) chosen by the brand's country; the sync and the rows don't change.

## API

| Operation | Endpoint | Who |
|---|---|---|
| Start a checkout for a plan (`{ planCode }`), answers `{ url }` | `POST /api/organization/subscription/checkout` | `subscription.manage` |
| Open the Customer Portal, answers `{ url }` | `POST /api/organization/subscription/portal` | `subscription.manage`, brand with a billing customer |
| Stripe webhooks | `POST /api/billing/stripe/webhooks` | anonymous, signature checked |
| Whether self-service billing applies, payment status | added to `GET /api/organization/subscription` (`selfService`, `paymentStatus`, `isExternal`) | `subscription.view` |

Both `POST` endpoints allow an inactive subscription (a read-only brand must be able to pay). The add-on is not checked with `RequireFeature`, because an ended subscription has no features and the brand must still be able to pay: the use case reads it from the current subscription. Checkout refuses a brand without `self-service-billing` (`403 feature.not_in_plan`), any plan but `lite` and `pro` (`400 billing.plan_not_available`), a brand that already pays through Stripe (`409 billing.already_subscribed`; it goes to the portal) and a country outside the list (`403 billing.not_available`). The portal answers `404 billing.no_customer` to a brand that never started a checkout.

## In the app (web)

- Ajustes → Plan, brand owners on the web in a billing country whose brand has `self-service-billing`: each paid plan shows its price "+ IVA" and **Elegir plan**; a brand already paying through Stripe sees **Gestionar suscripción** (portal) instead. `Enterprise` keeps the contact.
- The ended-subscription gate and `PlanOfferSheet` **Ver planes** keep going to Ajustes → Plan, where the button is.
- Return from Checkout: `?checkout=success` shows "Procesando el pago…" and reloads `GET /api/me` until the plan changes (a short limit, then "Puede tardar unos minutos"); `?checkout=cancelled` shows nothing special.
- Past due: a notice on Inicio and Ajustes → Plan with **Actualizar tarjeta** (portal) for brand owners, and nothing for the rest of the team.
- Native apps: no payment buttons; one line saying the plan is managed from the web version.

All new texts go in `es.json` and `en.json`; the screens use the existing `src/ui` primitives (`Button`, banners, skeletons).

## Security and operations

- Keys: a **restricted key** with only the Billing, Checkout, Customer Portal and Customers permissions it needs, and the webhook signing secret, in the App Service settings like the other secrets; never in the repository. Test mode keys in development.
- `api.stripe.com` is not reachable from the cloud development environment's network policy; tests never call Stripe (fake gateway, signed fake events), and Stripe is tried by hand in test mode with the Stripe CLI (`stripe listen --forward-to`).
- A webhook that fails answers `500` so Stripe retries for up to 3 days; Sentry reports it.
- Runbook (written in step 1): create the Stripe account (owner's legal entity), business details for invoices, invoice numbering and prefix, Stripe Tax registration (products, prices and the portal configuration are created by the app), Smart Retries and the emails Stripe sends, the webhook endpoint and its events, the keys. Linked from `README.md`.
- Terms of sale for the plans (B2B, no withdrawal right, renewal, cancellation) go in the legal texts before going live ([legal and privacy](../../legal-and-privacy.md)).

## Testing

- Unit (`Subscriptions.U.Tests`): every row in the event table above, out-of-order and repeated events converge, an unknown plan code changes nothing.
- Integration (`Api.I.Tests`, fake gateway): a signed fake `checkout.session.completed` turns the trial into `Pro` and the brand gets `Pro` features; the same event twice changes nothing; a bad signature is `400`; an event for brand A never touches brand B; `subscription.deleted` makes the brand read-only; checkout is refused to a brand without the `self-service-billing` add-on (and allowed once the operator gives it), to a branch owner, for `enterprise` and outside the billing countries; the portal still opens after the checkout replaced the trial and its add-on; the backoffice refuses **Cambiar plan** for a brand paying through Stripe.
- App (jest): the buttons appear only on the web, only for brand owners in a billing country whose brand has the feature; the success return polls `GET /api/me`; past due shows the notice.
- No e2e against Stripe: the hosted pages are Stripe's, and the critical part (the webhook) is covered by integration tests.

## Steps

1. **Stripe account and runbook** (owner, by hand, test mode first): account, tax registration, webhook and keys; products, prices and the portal configuration are created by the app. The runbook in `docs/`.
2. ✅ **Model and sync:** new tables and columns with their migration, `IBillingGateway`, `ExternalSubscriptionSync` with its unit tests.
3. ✅ **Stripe and webhook:** `Subscriptions.Stripe`, `POST /api/billing/stripe/webhooks` with signature check and idempotency; integration tests with signed fake events and a fake gateway. The webhook endpoint in Stripe listens to `checkout.session.completed`, `customer.subscription.created`, `customer.subscription.updated`, `customer.subscription.deleted`, `invoice.paid` and `invoice.payment_failed`; any other event is recorded and ignored.
4. ✅ **Checkout and portal:** the `self-service-billing` feature (add-on at 0, in no plan), `subscription.manage`, the two endpoints, the country check, `GET /api/organization/subscription` additions.
5. ✅ **App:** Ajustes → Plan on the web (**Elegir plan** and "+ IVA" on `lite` and `pro`, **Gestionar suscripción** for a brand that pays online), the return from Checkout (polls `GET /api/me` and the subscription every 3 s for up to a minute), the past-due notice on Inicio and Ajustes → Plan, and the line for the native apps. The contact line stays, because `Enterprise` is still agreed by hand.
6. ✅ **Backoffice and offers:** Stripe status and dashboard link on the brand, **Cambiar plan** refused for paying brands; `PlanOffer` with **Ofrecer plan**, **Copiar link de pago** and **Retirar oferta**; the offer in Ajustes → Plan; the checkout at the offered price; the webhook marks the offer used. Tests: the plan doesn't change until the webhook confirms the payment, the offered price is charged, an expired offer is not shown and charges the list price, the link only works for that brand's members with `subscription.manage`, and the offer works without the add-on but not outside the billing countries.
   Built: `PlanOffer` in `src/Subscriptions/Offers` (table `billing.PlanOffers`, migration `AddPlanOffers`); `PUT` and `DELETE /api/brands/{id}/offer` in the backoffice (`platform.manage`), which refuse a plan other than `lite` and `pro` (`backoffice.plan_not_offerable`), a price of 0 (`backoffice.offer_price_not_positive`), a brand that pays through Stripe (`backoffice.subscription_external`) and a brand outside the billing countries (`backoffice.billing_not_available`), and always offer in `EUR`. **Retirar oferta** runs on the first tap and **Deshacer** offers the same terms again. The brand detail adds the current offer (with its payment link and whether it is still active), `isExternal` and `paymentStatus` on the subscription, and `billingCustomerUrl` (the customer in Stripe's dashboard, `Backoffice:StripeDashboardUrl` + `customers/{id}`). In the app, the offer shows only to people with `subscription.manage` while the brand doesn't pay through Stripe, with **Aceptar oferta** on the web. The webhook marks the offer used when the paid subscription it starts has the offer's plan, price and currency. The backoffice host needs `WebApp:Url` for the payment link ([backoffice deployment](../../backoffice-deployment.md)).
7. **Go live for one brand:** live keys, live webhook, terms of sale; the operator gives `self-service-billing` to one brand, and the owner pays and cancels one real subscription.
8. **For everyone:** a migration adds `self-service-billing` to `Free`, `Lite` and `Pro`.

## Open decisions

| Decision | Proposal |
|---|---|
| Customers in Canarias (IGIC) | Ask the gestoría; Stripe Tax collects nothing there |
| VeriFactu (2027) | Check Stripe's support before 2027; otherwise export Stripe invoices to compliant software |
| Yearly billing | Later: a `{plan}-yearly-eur` price and `BillingPeriod = Yearly`, two months free |
| Add-ons (extra branch 20 €, storage 2 €/10 GB) through Stripe | Later: one product per add-on with the metadata `feature_code`, found or created like the plans, as subscription items (an extra branch is a quantity), mapped to `SubscriptionFeature` |
| `Enterprise` through Stripe | Later: the operator creates a Stripe subscription with an agreed price from the backoffice; until then it is set by hand |
| Argentina | Mercado Pago subscriptions behind `IBillingGateway`, prices in ARS, when Spain is live |
