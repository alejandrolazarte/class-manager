# Plan — Family app, shop and online payments (Stripe Connect)

Status: proposal, nothing built yet.

## Goal

A family opens the app, sees its children's classes, how many classes are left and what it owes, and pays from the phone: a class pack, the monthly fee, or products the branch sells (swimsuits, caps, the brand's merchandise). The money goes straight to the branch's own Stripe account; the app never holds it.

A branch decides what it sells:

- **Class packs** (the existing `ClassPack` catalog): buying one credits classes to the family, exactly like a sale recorded at the pool today.
- **The monthly fee**: paying it records a `Payment` for that month.
- **Products**: anything else, with stock loaded by the branch, or without stock (unlimited, or sold on order even when the shelf is empty) as the branch chooses.

## Decisions

### Money flow: Stripe Connect, one account per branch

- The app is the **platform**: one Stripe account owned by us, with Connect enabled.
- Each **branch** connects its own Stripe account through Stripe's hosted onboarding (Account Links). Nobody shares passwords or bank details with us. Tenerife, Valencia and Barcelona can be different people or companies, so the account belongs to the `Business`, not to the organization.
- **Direct charges** on the branch's account with an optional **application fee** for the platform: the branch is the seller in front of the family and its bank, pays Stripe's fees, handles refunds and disputes, and sees everything in its own Stripe dashboard. The platform fee starts at 0 % during the pilot.
- The connected account configuration (the Stripe dashboard the branch gets, who is liable for negative balances) is chosen when we implement it, following Stripe's current Connect guide for platforms in Spain. We don't build our own payouts, KYC or dashboards.

### Paying: Stripe Checkout, not our own card form

- The app creates a **Checkout Session** on the branch's account and opens Stripe's hosted page: cards, Apple Pay, Google Pay and 3-D Secure (required in Spain) with no card data touching our servers, so no PCI scope for us.
- It works the same on web (redirect) and on the phone (in-app browser, back to the app with a deep link).
- Classes are consumed in person and merchandise is physical, so app store in-app purchase rules don't apply; Stripe is allowed in the iOS and Android apps.
- **The webhook, not the return page, confirms the payment.** `checkout.session.completed` marks the order paid and fulfils it; the return page only shows "procesando" until the order is paid.

### Families get their own accounts

Today students and families are not users. This plan adds **family accounts**, a different kind of user from team members:

- `ClientAccounts` (`TenantId`, `ClientId`, `UserId`): an Identity user linked to a family (`Client`). The adult who pays is the user; children are the family's `Students` and never sign in.
- The branch invites the family from the family card: "Invitar a la app" sends a link by WhatsApp or email (the same single-use, 7-day, hashed-token mechanism as team invitations). Accepting creates the account or links an existing one.
- A user can be a family in one branch and a team member in another (a coach whose children swim). Sign-in opens the team app when the user is a member of the branch, and the family app otherwise; a switcher covers both, like branches today.
- Family access is enforced like tenancy: every family endpoint reads the `ClientId` from the user's `ClientAccounts` row, never from the request.

### Products and stock

- `Products` belong to a branch: name, description, photo, price (VAT included), active, and a **stock mode**:
  - `Unlimited`: no stock kept (a service, a digital material, "a pedido").
  - `Tracked`: stock is counted; when it reaches 0 the product shows "Agotado".
  - `TrackedWithBackorder`: stock is counted, but the family can still buy at 0 and the order waits until the branch restocks.
- **Variants** for sizes and colors (`ProductVariants`: name such as "Talle M", own stock). A product without sizes has one variant.
- Stock changes are an append-only ledger (`StockMovements`: restock, sale, refund, manual adjustment with a note), so the branch can see why a number changed; the current stock is its sum.
- **Reservation**: starting a checkout reserves the units for as long as the Checkout Session lives (30 minutes); `checkout.session.expired` releases them. Two families can't both buy the last swimsuit.
- Delivery in the first version is **pickup at the branch**: a paid product order shows "Para retirar" to the branch until someone marks it delivered. Shipping comes later if a brand asks for it.
- Brand merchandise shared by several branches comes later (brand catalog, step 5 of the roles plan). Until then each branch loads the products it sells.

### Orders connect everything

One `Order` per checkout, with lines of three kinds:

| Line | On payment |
|---|---|
| Class pack (`ClassPackId`, the student it is for) | Creates a `ClassPackPurchase` with method `Online` and a link to the order; the classes appear in the family's balance at once |
| Monthly fee (`Month`, amount = what is still owed) | Creates a `Payment` for that month with method `Online` |
| Product (`ProductVariantId`, quantity) | Turns the reservation into a sale in the stock ledger; the order waits for pickup |

- Lines keep a snapshot of name and price, like `ClassPackPurchase` does today.
- `PaymentMethod` gains `Online`. Online payments have no `RecordedByUserId`; the order is their record.
- The same orders also serve **sales at the counter**: a coach sells a swimsuit in person and records it as an order with method cash or card, so stock stays right whichever way it was sold.
- Refunds: the branch refunds from the order in the app (a Stripe refund on its account), or in its Stripe dashboard (`charge.refunded` keeps the app in sync). A refunded pack line removes its unused classes; a refunded fee line removes its `Payment`; a refunded product line returns stock when the branch says the item came back.
- Webhooks are idempotent: every Stripe event id is stored once (`ProcessedStripeEvents`) and processing the same event twice does nothing.

### Architecture

- `Core` gets a port, `IOnlinePaymentGateway` (connect account, onboarding link, account status, create checkout, refund), and never references Stripe. `src/Payments.Stripe` implements it with the official `Stripe.net` package, the same way `src/Security` hides Identity. An analyzer rule keeps Stripe types out of `Core` and `Api`.
- Webhooks enter through one anonymous endpoint that checks Stripe's signature and hands the event to use cases.
- Tests never call Stripe: unit and integration tests use a fake gateway, and a test posts signed fake webhook events to the endpoint. Stripe itself is tried by hand in test mode.
- Secrets: the platform's `sk_test_` / `sk_live_` key and the webhook signing secret live in the environment's secrets (Azure App Service settings, GitHub Actions secrets), never in the repository. `api.stripe.com` must be allowed in the development environment's network policy.

## Permissions

| Permission | For |
|---|---|
| `onlinePayments.manage` | Connect or disconnect the branch's Stripe account, see its status, set what is sold online. Brand owners and branch owners |
| `products.view` / `products.manage` | See / create and edit products, variants, stock |
| `orders.view.own` / `orders.view.all` | See orders of the member's families / of every family |
| `orders.manage` | Mark delivered, refund, record counter sales |

A family account has no team permissions; family endpoints are a separate group (`/api/family/...`) that only accepts family users.

## Contract (first version)

| Operation | Endpoint | Who |
|---|---|---|
| Start or resume Stripe onboarding, returns a link | `POST /api/online-payments/account/onboarding` | `onlinePayments.manage` |
| Account status (pending, active, restricted) | `GET /api/online-payments/account` | `onlinePayments.manage` |
| Stripe webhooks | `POST /api/stripe/webhooks` | anonymous, signature checked |
| Products and variants | `GET/POST/PUT /api/products`, `POST /api/products/{id}/stock` | `products.*` |
| Orders of the branch | `GET /api/orders`, `PUT /api/orders/{id}/delivered`, `POST /api/orders/{id}/refunds` | `orders.*` |
| Invite a family | `POST /api/clients/{id}/app-invitation` | `students.manage` |
| Accept a family invitation | `POST /api/auth/family-invitations/accept` | anonymous |
| Family home: students, next classes, class balance, fee of the month | `GET /api/family` | family |
| Shop: packs and products sold online | `GET /api/family/shop` | family |
| Start a checkout (lines), returns the Stripe URL | `POST /api/family/checkouts` | family |
| My orders | `GET /api/family/orders` | family |

## The app

### What the family sees

- **Inicio**: each child with their next classes, classes left or the month's fee, and a "Pagar" button when something is owed.
- **Tienda**: class packs first ("8 clases · 2 meses · 120 €"), then products with photo, price, sizes and "Agotado" / "A pedido". A small cart, "Pagar" opens Stripe, the return shows "Pago recibido" once the webhook confirms it.
- **Mis compras**: orders with status (pagado, para retirar, entregado, reembolsado) and a receipt.
- The branch's theme and icon, so it looks like the branch's app.

### What the branch sees

- Ajustes → **Cobros online**: "Conectar con Stripe", the status, and what is sold online (packs, monthly fee, products).
- Ajustes → **Productos**: products, variants, stock and its movements, "Cargar stock".
- **Pedidos**: paid online and at the counter, "Para retirar" first, mark delivered, refund.
- Family card: "Invitar a la app", whether the family has an account, its online orders.
- Cuotas and the family's payments show online payments like any other, with method "Online".

## Delivery

Each step is usable on its own.

1. **Stripe account per branch.** Platform account in test mode, `Payments.Stripe`, onboarding and status in Ajustes → Cobros online, webhook endpoint with signature check and idempotency. Done when DF Tenerife (test) is connected and active in test mode.
2. **Products and orders at the counter.** Products, variants, stock ledger, counter sales and pickup, without families and without Stripe. Useful from day one for merchandise.
3. **Family accounts.** Invitation, sign-in, family home (read only: classes, balance, fee).
4. **Online checkout.** Shop, cart, Checkout Sessions with reservations, webhooks that fulfil packs, fees and products, "Mis compras", refunds.
5. **Later, when asked:** brand catalog shared by branches, shipping, discount codes, automatic monthly fee charges (Stripe subscriptions or saved cards), invoices, Stripe Tax, a public shop page for people who are not families yet.

## Testing

- Unit and integration tests with a fake `IOnlinePaymentGateway`; webhook tests post signed fake events and check that a pack gives classes, a fee records a payment, stock moves once, and a repeated event changes nothing.
- Tenant isolation tests for every new table, and a test proving a family can only read and buy for its own `ClientId`.
- By hand in Stripe test mode: connected accounts with Stripe's test onboarding data, test cards (`4242 4242 4242 4242`, declined cards, 3-D Secure cards), webhooks forwarded to a local API with the Stripe CLI (`stripe listen`).
- e2e: one critical flow with the fake gateway (family buys a pack, sees the classes).

## Open decisions

| Decision | Proposal |
|---|---|
| Platform fee | 0 % in the pilot; decide the business model before going live with other businesses ([branding and plans](../../branding-and-plans.md)) |
| Who pays Stripe's fees | The branch (direct charges) |
| A branch without Stripe | Everything works as today; the shop shows only "consultá en la sede" and online buttons are hidden |
| Families that don't want an account | The branch keeps recording payments by hand; nothing forces families into the app |
| VAT and invoices | Prices include VAT and the branch issues its invoices as today; Stripe receipts are not invoices. Revisit with Stripe Tax or an invoicing integration |
| Bizum | Not in the first version; check what Stripe offers in Spain when we build step 4 |
