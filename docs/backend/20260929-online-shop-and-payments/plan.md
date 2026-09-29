# Plan — Family app, shop and online payments

Status: steps 1–3 done, the rest is a proposal. Steps 1–4 need no payment provider; Stripe Connect comes last.

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
- **Stripe Checkout**: the app opens Stripe's hosted page (cards, Apple Pay, Google Pay, 3-D Secure), so no card data touches our servers. It works the same on web and in the phone app (in-app browser, back with a deep link). Classes are consumed in person and merchandise is physical, so app store in-app purchase rules don't apply.
- **The webhook confirms the payment**, not the return page: `checkout.session.completed` marks the order `Paid` with method `Online`, and everything after that is the same as a payment confirmed at the branch. `checkout.session.expired` leaves the order `Requested` (the family can still pay at the branch or retry).
- `PaymentMethod` gains `Online`; the monthly fee gets its own online payment that records a `Payment` with that method.
- Webhooks are idempotent: every Stripe event id is stored once (`ProcessedStripeEvents`).
- A branch without Stripe keeps working exactly as in steps 1–4.
- The connected account configuration (the Stripe dashboard the branch gets, who is liable for negative balances) is chosen when we build it, following Stripe's current Connect guide for platforms in Spain.

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
| `onlinePayments.manage` (step 5) | Connect the branch's Stripe account. Brand owners and branch owners |

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
| Stripe account of the branch | `POST /api/online-payments/account/onboarding`, `GET /api/online-payments/account` | `onlinePayments.manage` | 5 |
| Stripe webhooks | `POST /api/stripe/webhooks` | anonymous, signature checked | 5 |
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
4. **Notifications.** Email (and later push) to the family when an order is ready to pick up and to the branch when an order is placed.
5. **Stripe account per branch.** Platform account in test mode, `Payments.Stripe`, onboarding and status in Ajustes → Cobros online, webhook endpoint with signature check and idempotency.
6. **Online payment.** "Pagar ahora" for orders and the monthly fee with Stripe Checkout; webhooks mark orders paid and record fee payments; refunds through Stripe.
7. **Later, when asked:** brand catalog shared by branches, shipping, discount codes, automatic monthly fee charges, invoices, Stripe Tax, a public shop page for people who are not families yet.

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
