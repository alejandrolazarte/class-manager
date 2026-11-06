# Plan — Family app, shop and online payments

Status: steps 1–4 done, step 5 in progress, the rest is a proposal. Steps 1–4 need no payment provider; Stripe Connect comes last.

## Goal

A family, or an adult who is the student, opens the app, sees their classes, how many classes are left and what they owe, and orders from the phone: a class pack, or products the branch sells (swimsuits, caps, the brand's merchandise).

First the order is **paid at the branch**, as today: the branch confirms the payment and the app does the rest (credits the classes, moves the stock). When Stripe Connect arrives, the same orders can also be **paid online**, with the money going straight to the branch's own Stripe account.

A branch decides what it sells:

- **Class packs** (the existing `ClassPack` catalog): once paid, the classes are credited, exactly like a sale recorded at the pool today.
- **The monthly fee**: shown with what is owed; paying it records a `Payment` for that month.
- **Products**: anything else, with stock loaded by the branch, or without stock (unlimited, or sold on order even when the shelf is empty) as the branch chooses.

## Decisions

### Families and adult students get their own accounts

Today students and families are not users. This plan adds **family accounts**, a different kind of user from team members:

- `ClientAccounts` (`TenantId`, `ClientId`, `UserId`): an Identity user linked to a `Client`. The `Client` is whoever pays: a parent with children, or an adult who takes the classes themselves (a `Client` whose only student is the same person). Children are the family's `Students` and never sign in.
- The branch invites from the family card: "Invitar a la app" sends a link by WhatsApp or email (the same single-use, 7-day, hashed-token mechanism as team invitations). Accepting creates the account or links an existing one.
- A user can be a family in one branch and a team member in another (a coach whose children swim). Sign-in opens the team app when the user is a member of the branch, and the family app otherwise; a switcher covers both, like branches today.
- Family access is enforced like tenancy: every family endpoint reads the `ClientId` from the user's `ClientAccounts` row, never from the request.
- **Family and team are separated end to end** (built in step 1, see [authorization](../../authorization.md#families-and-adult-students)): the session kind (`team` / `family`) is fixed at sign-in and kept on refresh, team endpoints reject family tokens, family endpoints (`/api/family/...`, their own `family` policy) reject team tokens, and family use cases never reuse team ones.

### Products and stock

- `Products` belong to a branch: name, description, photo, price (VAT included), active, visible in the app or only at the counter, and a **stock mode**:
  - `Unlimited`: no stock kept (a service, a digital material, "a pedido").
  - `Tracked`: stock is counted; when it reaches 0 the product shows "Agotado".
  - `TrackedWithBackorder`: stock is counted, but a family can still order at 0 and the order waits until the branch restocks.
- **Variants** for sizes and colors (`ProductVariants`: name such as "Talle M", own stock). A product without sizes has one variant.
- Stock changes are an append-only ledger (`StockMovements`: restock, reservation, sale, cancellation, refund, manual adjustment with a note), so the branch can see why a number changed; the current stock is its sum.
- Delivery in the first version is **pickup at the branch**: a paid product order shows "Para retirar" to the branch until someone marks it delivered. Shipping comes later if a brand asks for it.
- Brand merchandise shared by several branches comes later (brand catalog, step 5 of the roles plan). Until then each branch loads the products it sells.

### Orders connect everything

One `Order` per purchase, from the family app or at the counter, with lines of two kinds (the monthly fee is paid, not ordered; see below):

| Line | When the order is paid |
|---|---|
| Class pack (`ClassPackId`) | Creates a `ClassPackPurchase` linked to the order; the classes appear in the family's balance at once |
| Product (`ProductVariantId`, quantity) | Turns the reserved units into a sale in the stock ledger; the order waits for pickup |

- Lines keep a snapshot of name and price, like `ClassPackPurchase` does today.
- **Order states**: `Requested` (placed by the family, to pay at the branch) → `Paid` (payment method and who confirmed it) → `Delivered` (products picked up); or `Cancelled` by the family before it is paid, or by the branch. A counter sale starts as `Paid`.
- **Reservation**: a requested order reserves tracked units until it is paid or cancelled, so two families can't both order the last swimsuit. The branch sees open orders oldest first; an order left unpaid for 7 days is cancelled and its units released.
- Refunds of paid orders are recorded by the branch: a pack line removes its unused classes, a product line returns stock when the item came back.
- The **monthly fee** is not an order line: the family sees what it owes for the month and pays at the branch, where it is recorded as today. With online payments it gets its own "Pagar" (step 6).

### Online payments, last: Stripe Connect, one account per branch

- The app is the **platform**: one Stripe account owned by us, with Connect enabled.
- Each **branch** connects its own Stripe account through Stripe's hosted onboarding (Account Links). Nobody shares passwords or bank details with us. Tenerife, Valencia and Barcelona can be different people or companies, so the account belongs to the `Business`, not to the organization.
- **Direct charges** on the branch's account with an optional **application fee** for the platform: the branch is the seller in front of the family and its bank, pays Stripe's fees, handles refunds and disputes, and sees everything in its own Stripe dashboard. The platform fee starts at 0 % during the pilot.
- **Stripe Checkout**: the app opens Stripe's hosted page (cards, Apple Pay, Google Pay, 3-D Secure), so no card data touches our servers. It works the same on web and in the phone app (in-app browser, back with a deep link). Classes are consumed in person and merchandise is physical, so app store in-app purchase rules don't apply. Each checkout sends the order's lines with their names and amounts (`price_data`), so no product or price is created in the branch's Stripe account and owners never keep a catalog there: the catalog stays in the app.
- **The webhook confirms the payment**, not the return page: `checkout.session.completed` marks the order `Paid` with method `Online`, and everything after that is the same as a payment confirmed at the branch. `checkout.session.expired` leaves the order `Requested` (the family can still pay at the branch or retry).
- `PaymentMethod` gains `Online`; the monthly fee gets its own online payment that records a `Payment` with that method.
- Webhooks are idempotent: every Stripe event id is stored once (`ProcessedStripeEvents`).
- A branch without Stripe keeps working exactly as in steps 1–4.
- **Full Stripe dashboard** (decided with the owner on 2026-11-05): each connected account is created with the full dashboard, Stripe collects its requirements, the account pays Stripe's fees and is liable for refunds, disputes and negative balances (the classic "Standard" account, set with `controller` properties). The branch manages everything about Stripe in Stripe; the app only connects it and charges through it. The platform is never liable for the branch's losses.
- **Several branches can share one account** (decided with the owner on 2026-11-05): branches owned by the same person or company want one Stripe account, one IBAN and one set of invoices. See [step 5 in detail](#step-5-in-detail-the-stripe-account-of-each-branch).

### Architecture

- Family endpoints are a separate group (`/api/family/...`) that only accepts family users and reads their `ClientId` from `ClientAccounts`.
- Orders, products and stock live in `Core` like the rest of the domain; paying at the branch needs nothing external.
- For step 5, `Core` gets a port, `IOnlinePaymentGateway` (connect account, onboarding link, account status, create checkout, refund), and never references Stripe. `src/Payments.Stripe` implements it with the official `Stripe.net` package, the same way `src/Security` hides Identity. Tests use a fake gateway and post signed fake webhook events; Stripe itself is tried by hand in test mode. Keys live in the environment's secrets, and `api.stripe.com` must be allowed in the development environment's network policy.

## Permissions

| Permission | For |
|---|---|
| `students.manage` (exists) | Invite a family to the app |
| `products.view` / `products.manage` | See / create and edit products, variants, stock |
| `orders.view.own` / `orders.view.all` | See orders of the member's families / of every family |
| `orders.manage` | Confirm payment, mark delivered, cancel, refund, record counter sales |
| `onlinePayments.manage` (step 5) | Connect the branch's Stripe account, see its status, stop using it. Brand owners and branch owners |
| `onlinePayments.share` (step 5, brand-only) | Make the branch use another branch's Stripe account. Brand owners |

A family account has no team permissions.

## Contract

| Operation | Endpoint | Who | Step |
|---|---|---|---|
| Invite a family / adult student | `POST /api/clients/{id}/app-invitation` | `students.manage` | 1 |
| Accept the invitation | `POST /api/auth/family-invitations/accept` | anonymous | 1 |
| Family home: students, next classes, class balance, fee of the month | `GET /api/family` | family | 1 |
| Products and variants | `GET/POST/PUT /api/products`, `POST /api/products/{id}/stock` | `products.*` | 2 |
| Orders of the branch, counter sale | `GET/POST /api/orders` | `orders.*` | 2 |
| Confirm payment, deliver, cancel, refund | `PUT /api/orders/{id}/payment`, `/delivered`, `/cancellation`, `POST /api/orders/{id}/refunds` | `orders.manage` | 2 |
| Shop: packs and products shown in the app | `GET /api/family/shop` | family | 3 |
| Place an order to pay at the branch | `POST /api/family/orders` | family | 3 |
| My orders, cancel a requested one | `GET /api/family/orders`, `PUT /api/family/orders/{id}/cancellation` | family | 3 |
| Status of the branch's Stripe account (and the brand's accounts, with `onlinePayments.share`) | `GET /api/online-payments` | `onlinePayments.manage` | 5 |
| Connect or continue the onboarding, answers `{ url }` | `POST /api/online-payments/onboarding` | `onlinePayments.manage` | 5 |
| Read the account from Stripe again (return from onboarding) | `POST /api/online-payments/refresh` | `onlinePayments.manage` | 5 |
| Use another branch's account | `PUT /api/online-payments/account` `{ accountId }` | `onlinePayments.share` | 5 |
| Stop using the account | `DELETE /api/online-payments/account` | `onlinePayments.manage` | 5 |
| Stripe Connect webhook | `POST /api/online-payments/stripe/webhooks` | anonymous, signature checked | 5 |
| Pay an order or the monthly fee online | `POST /api/family/checkouts` | family | 6 |

## The app

### What the family or adult student sees

- **Inicio**: each student (or "Tus clases" when the adult is the only student) with their next classes, classes left or the month's fee and what is owed.
- **Tienda** (step 3): class packs first ("8 clases · 2 meses · 120 €"), then products with photo, price, sizes and "Agotado" / "A pedido". A small cart and "Pedir": the order shows "Pagalo en la sede". With online payments (step 6), "Pagar ahora" too.
- **Mis pedidos**: orders with status (pendiente de pago, pagado, para retirar, entregado, cancelado).
- The branch's theme and icon, so it looks like the branch's app.

### What the branch sees

- Family card: "Invitar a la app", whether the family has an account, its orders.
- Ajustes → **Productos**: products, variants, stock and its movements, "Cargar stock", shown in the app or not.
- **Pedidos**: open orders first ("Pendiente de pago", "Para retirar"), confirm payment with the method, mark delivered, cancel, refund; "Venta en mostrador".
- Ajustes → **Cobros online** (step 5): "Conectar con Stripe" and its status.

## Delivery

Each step is usable on its own; steps 1–4 need no payment provider.

1. **Family and adult student accounts — done** ([authorization](../../authorization.md#families-and-adult-students)). Invitation from the family card ("Invitar a la app"), accept screen (`/accept-family-invitation`), sign-in opens the family shell (`/family`), Inicio is read only: students with their next classes (14 days, cancellations, reschedules and substitutes applied), the month's fee or the class balance, and "se paga en la sede". Family and team sessions are separated end to end (kind fixed at sign-in, disjoint policies and endpoints, tests for both directions). Not built yet: opening the family side when the same user is also a team member, e2e for the invitation flow (needs the invitation email in e2e), the branch's theme in the family shell.
2. **Products and orders at the counter — done** ([authorization](../../authorization.md#products-and-orders), [domain model](../domain-model.md#product-and-productvariant-added-with-the-shop)). Ajustes → Productos (sizes as a comma-separated list, stock mode, shown in the app or not, load and adjust stock with its last movements) and Ajustes → Pedidos ("Para retirar" / todos, mark delivered, refund a line). "Venta en mostrador" from Pedidos (products, no family) and from the family card (packs and products): a paid order that credits the classes at once and moves stock. Not built yet: product photos, product sales in the monthly totals.
3. **Shop in the family app, pay at the branch — done.** "Tienda" (packs, then products with sizes and "Agotado" / "A pedido") and "Mis pedidos" from the family home; ordering reserves tracked units, a family has at most 5 unpaid orders and can cancel them. In Ajustes → Pedidos the branch filters "Pendientes de pago", confirms the payment with its method (classes credited, products "Para retirar") or cancels. Unpaid orders are cancelled after 7 days by a job that runs every hour. Stock is taken under a per-size lock, at the counter too. Not built yet: e2e for the whole flow (needs the invitation email in e2e), notifications (step 4), product photos, fine-grained family permissions (one `family` policy is enough while every family can see and order).
4. **Delivery and notifications — done.** An order with products is picked up at the branch or handed over in a class one of the family's students attends: the family chooses it in the shop, the branch sets it at the counter (or changes it through the API). The branch marks it "Listo para entregar" (or while confirming the payment), and the coach sees "Para entregar en esta clase" in the class screen and marks it delivered. Emails (plain text, Spanish): new app order → branch staff with `orders.manage` and brand owners; payment confirmed, ready (where to get it) and cancelled (by the branch or unpaid for 7 days) → the family's account emails, or the client's email when it has no account. A failed email is logged and never undoes the operation. Not built yet: push notifications, changing the delivery from the app after ordering, a substitute coach seeing the class's deliveries (only the class's own coach and members with `attendance.record.all` do).
5. **Stripe account per branch.** Platform account in test mode, onboarding and status in Ajustes → Cobros online, sharing an account between branches of the brand, webhook endpoint with signature check ([in detail](#step-5-in-detail-the-stripe-account-of-each-branch)).
6. **Online payment.** First the [platform fee](#the-platform-fee-on-online-sales) per plan and per brand in the backoffice (6a), then "Pagar ahora" for orders and the monthly fee with Stripe Checkout; webhooks mark orders paid and record fee payments; refunds through Stripe.
7. **Later, when asked:** brand catalog shared by branches, shipping, discount codes, automatic monthly fee charges, invoices, Stripe Tax, a public shop page for people who are not families yet.

## Step 5 in detail: the Stripe account of each branch

### Model

- `OnlinePaymentAccounts` (new, **owned by the brand**, not by a branch, so it has `OrganizationId` and no `TenantId`, like the `billing` schema): `Id`, `OrganizationId`, `ExternalAccountId` (Stripe `acct_…`, unique), `CreatedForBusinessId` (the branch that connected it), `DetailsSubmitted`, `ChargesEnabled`, `PayoutsEnabled`, `CreatedAt`, `StatusReadAt` (when the flags were last read from Stripe). It is the one table a branch reads outside its tenant on purpose: an account can be used by several branches of the same brand, so it can't belong to one of them. Every use case checks that the account's `OrganizationId` is the current branch's.
- `Businesses.OnlinePaymentAccountId` (nullable): the account the branch charges through. Several branches can point to the same account.
- **Status**, computed from Stripe's flags: `NotConnected` (no account), `Pending` (onboarding not finished: `DetailsSubmitted` is false), `Restricted` (finished but Stripe can't charge yet, it is reviewing or needs more data: `ChargesEnabled` is false), `Active` (`ChargesEnabled`). Only `Active` branches will offer "Pagar ahora" in step 6.
- An account is never deleted: Stripe keeps it, and a branch that stops using it and connects again gets the same one back (the account created for it), instead of a second Stripe account.

### An add-on first, a feature of plans later

Like [paying the plan online](../20261029-stripe-billing/plan.md#an-add-on-first-a-feature-of-every-plan-later) (decided with the owner on 2026-11-06): online payments are a feature, `online-payments`, in the catalog as an **add-on at 0 €** that **no plan includes**. While we test them, the operator gives it to one brand at a time with **Dar add-on** in the backoffice, and only that brand sees Ajustes → **Cobros online**. Both permissions (`onlinePayments.manage`, `onlinePayments.share`) are gated by the feature (`FeatureGatedPermissions` in the API and `featureGatedPermissions` in the app), so without it the endpoints answer `403 feature.not_in_plan` and the app hides the row and the screen. The Connect webhook is not gated: Stripe must always reach it. Add-ons belong to a subscription and don't carry over, so a brand that changes plan needs the add-on again. **Making it a feature for everyone** is data, not code: a migration adds `online-payments` to the plans that include it.

### Who

- `onlinePayments.manage` (branch permission, assignable to custom roles; branch owners and brand owners have it): see the status, connect the branch, continue the onboarding, stop using the account.
- `onlinePayments.share` (brand-only, like `branches.create`; brand owners): see the brand's other accounts and make the branch use one of them. A branch owner can't point their branch to another branch's account, because the branches may belong to different people.

### Flow

1. Ajustes → **Cobros online** shows the status. Outside the countries Stripe supports for us (EU and EEA, the same list as the plans' billing) it says online payments aren't available there yet.
2. **Conectar con Stripe** (`POST /api/online-payments/onboarding`): the first time it creates the Stripe account (country of the branch, email of the person connecting, `business_id` and `organization_id` in its metadata) and stores it; every time it creates an **Account Link** (`account_onboarding`) and the app opens it. The branch types its legal details and IBAN on Stripe's page; nothing of that reaches us.
3. Stripe returns to `{WebApp:Url}/settings/online-payments?onboarding=return`; the app calls `POST /api/online-payments/refresh`, which reads the account from Stripe and stores its flags. `?onboarding=refresh` (the link expired) starts the onboarding again.
4. The **webhook** `POST /api/online-payments/stripe/webhooks` (a separate Stripe endpoint that listens to **connected accounts**, with its own signing secret `Stripe:ConnectWebhookSecret`) receives `account.updated`, checks the signature, reads the account from Stripe again and stores its flags. It is idempotent without a table: it always stores Stripe's current state. Step 6 adds the payment events and their `ProcessedStripeEvents`.
5. **Usar la cuenta de otra sede** (`PUT /api/online-payments/account` `{ accountId }`, `onlinePayments.share`): lists the brand's accounts with the branches that use each one, and points the current branch to the chosen one.
6. **Dejar de usar esta cuenta** (`DELETE /api/online-payments/account`, asks first with `ConfirmDialog`): the branch stops charging online; Stripe and the other branches that share the account are untouched.
7. With an `Active` account the screen links to Stripe's dashboard (`https://dashboard.stripe.com/`), where the branch signs in with its own Stripe user.

### Code

- `Core`: `OnlinePaymentAccount`, the port `IOnlinePaymentGateway` (create an account, create an onboarding link, read an account, read a webhook event) and the use cases. `Core` never references Stripe.
- `Infrastructure/OnlinePayments`: `StripeOnlinePaymentGateway` with `Stripe.net` and the same `Stripe:SecretKey` as the plans (it is the same platform account). Without the key the gateway is a placeholder that fails when used, so development and CI run without Stripe. A separate `Payments.Stripe` library is not worth it yet: nothing else uses it.
- Errors: `onlinePayments.not_available` (country), `onlinePayments.account_not_found` (another brand's account, or an unknown one), `onlinePayments.not_connected` (refresh or stop without an account).

### Setting it up

1. In the Stripe account (the sandbox first): **Connect → Get started**, with the platform profile Stripe asks for.
2. **Developers → Webhooks → Add destination**, events from **Connected accounts**, event `account.updated`, URL `https://api.laztty.com/api/online-payments/stripe/webhooks`. Copy its signing secret.
3. The API already has `Stripe__SecretKey` (the plans use the same key). Add the Connect signing secret:

```powershell
az containerapp secret set -n class-manager-api -g class-manager-rg --secrets "stripe-connect-webhook-secret=whsec_..."; az containerapp update -n class-manager-api -g class-manager-rg --set-env-vars "Stripe__ConnectWebhookSecret=secretref:stripe-connect-webhook-secret"
```

### The platform fee on online sales

Decided with the owner on 2026-11-06. Most competitors charge a monthly plan and no fee on payments; Momence's free plan takes 5 %. [Plans, prices and competition](https://github.com/alejandrolazarte/Documentation/blob/main/projects/class-manager/planes-precios-y-competencia.md) proposed online payments with no fee in Pro and a small one (about 1 %) in Lite.

- **Every plan has a sales fee**, a percent (`Plans.SalesFeePercent`, 0 to 100, two decimals). It starts at **0 % in every plan** for the pilot and the operator changes it in the backoffice (**Planes**), with no code or deploy.
- **A brand can have its own fee** that replaces its plan's: for example a brand on Enterprise for free that pays a percent of its sales instead. It is set in the backoffice on the brand (**Comisión por ventas**) and doesn't touch any other brand.
- **The brand's fee doesn't belong to its subscription.** Subscriptions are replaced on every plan change (by the operator or by Stripe), and add-ons don't carry over; an agreed fee should stay until the operator removes it. So it is its own record, `SalesFeeOverrides` (`SubscriberId`, `Percent`, `Note`, `CreatedOn`, soft delete; one current per brand), replaced like the other records that change over time.
- **The fee of a sale** is the brand's fee if it has one, otherwise its current plan's, read when the checkout is created. Step 6 sends it to Stripe as the `application_fee_amount` of each direct charge, rounded down to the cent, so the fee never takes more than the percent. A sale already paid keeps the fee it had.
- **Before charging a fee above 0 %**, the owner must be registered as self-employed and invoices the fee to the branch ([autónomo y cobros](https://github.com/alejandrolazarte/Documentation/blob/main/projects/class-manager/autonomo-y-cobros.md)). The app only records and charges it.

| Operation | Endpoint (backoffice) | Who |
|---|---|---|
| The plans with their fee | `GET /api/subscription-catalog` (adds `salesFeePercent`) | `platform.view` |
| Change a plan's fee | `PUT /api/plans/{code}/sales-fee` `{ percent }` | `platform.manage` |
| The brand's fee (its own or its plan's) | `GET /api/brands/{id}/sales-fee` | `platform.view` |
| Give the brand its own fee | `PUT /api/brands/{id}/sales-fee` `{ percent, note }` | `platform.manage` |
| Back to the plan's fee (asks first: it changes what the brand pays) | `DELETE /api/brands/{id}/sales-fee` | `platform.manage` |

## Testing

- Integration tests with a fake gateway and signed fake events: connecting creates one account and returns its link, connecting again reuses it, a branch of another brand can't see or use the account (`404`), sharing needs `onlinePayments.share`, the webhook with a bad signature is `400`, `account.updated` turns the status `Active`, stopping keeps the account for the other branches.
- App (jest): the status and its actions, the confirmation before stopping, sharing only for brand owners.
- By hand in the Stripe sandbox: Connect enabled, onboarding with Stripe's test data, the connected accounts webhook pointing to the endpoint.

## Testing

- Unit tests for stock (modes, reservations, backorder, cancellation, refund) and order states.
- Integration tests with Testcontainers: tenant isolation for every new table, a family can only read and order for its own `ClientId`, a team member can't use family endpoints and a family user can't use team endpoints, paying an order credits classes and moves stock exactly once.
- e2e for the critical flow: the branch invites a family, the family signs in, orders a pack, the branch confirms the payment, the family sees the classes.
- Step 5–6: a fake `IOnlinePaymentGateway` and signed fake webhook events in tests; by hand in Stripe test mode with test onboarding data, test cards (`4242 4242 4242 4242`, declined, 3-D Secure) and the Stripe CLI (`stripe listen`) forwarding webhooks to a local API.

## Open decisions

| Decision | Proposal |
|---|---|
| Unpaid requested orders | Cancelled after 7 days, units released; the branch can cancel sooner |
| Families that don't want an account | The branch keeps working by hand; nothing forces families into the app |
| Self sign-up without an invitation | Not in the first version; the branch invites. A public link per branch comes with the public shop page |
| Platform fee | 0 % in the pilot; decide the business model before going live with other businesses ([branding and plans](../../branding-and-plans.md)) |
| Who pays Stripe's fees | The branch (direct charges) |
| VAT and invoices | Prices include VAT and the branch issues its invoices as today; Stripe receipts are not invoices |
| Bizum | Check what Stripe offers in Spain when we build step 6 |
