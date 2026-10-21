# Orders paid in parts and handed over when the team decides

Follow-up to the [shop and payments plan](../20260929-online-shop-and-payments/plan.md). Until now an order had one status that went `Requested` → `Paid` → `Delivered`: it was paid in full or not at all, and it could only be handed over once paid. Monthly fees can already be paid in parts; orders couldn't.

## Decisions

- **Payment and hand-over are independent.** An order's `Status` only says whether it was handed over: `Pending` → `Delivered`, or `Pending` → `Cancelled`. How much was paid comes from its payments: `PaymentStatus` is `Unpaid`, `Partial` or `Paid`, like `FeeStatus`.
- **An order is handed over whole or not at all**: products and class packs together. Whoever sells decides when; it doesn't depend on the payment. An order can be handed over without being paid, and paid without being handed over.
- **Class packs are credited when the order is handed over**, not when it is paid. Handing over a pack-only order means crediting its classes. Paying an order can hand it over in the same step ("Se lo lleva ahora", on by default for pack-only orders), so a pack sold and paid at the branch is still credited at once.
- **Payments are their own records** (`OrderPayment`: amount, method, date, who recorded it), one or more per order, like fee payments. A payment can't be more than what is left to pay (`400 order.amount_over_balance`), and a fully paid or cancelled order takes no more payments (`409 order.nothing_owed`).
- **What is owed**: `Balance = Total − RefundedAmount − PaidAmount`, never below 0. A refund first lowers what is still owed; only what was paid beyond that is money given back (`CollectedAmount = min(PaidAmount, Total − RefundedAmount)`).
- **At the counter**, the sale says how much is paid now (`paidAmount`, the total by default, 0 to pay later) and whether it is handed over now. Without `paidAmount` the API takes the total, as before.
- **Cancelling** is for orders not handed over. The team can cancel one with payments, as Shopify does: "Cancelar pedido" is one of the order's actions (no longer inside the payment panel, so a fully paid order waiting to be picked up can be cancelled too), the [confirm dialog](../../frontend/20261011-confirm-order-cancellation/plan.md) adds "Se registra la devolución de $5.000. Devolver el dinero queda a cargo de {business}." — the app only records it — and everything paid is recorded as given back (`ReturnedAmount = PaidAmount − CollectedAmount`), so it no longer counts as collected. Counter-sale units of tracked products go back to stock; an app order's reservation is released. Keeping the money as credit for the client (as studio apps like Mindbody do) is left for later: there is no client balance yet.
- **The student cancels only before paying**: an order placed from the app and without payments. Once there is money involved, only the team cancels it, because the money is given back at the branch. A client's order with any payment no longer expires after 7 days.
- **Refunds** apply to orders handed over, or not handed over but with a payment (as before, when they were `Paid`). A pack line can only be refunded once its classes were credited.
- **The month summary** counts the payments made in the month (`Collected`), what all open orders still owe (`Unpaid`), and the part of the month's payments that went to class packs (`ClassPackSales`, in proportion to each order's lines).
- **Emails**: "Recibimos tu pago" is sent to a client's app order when it becomes fully paid. Its next step says the classes are credited or will be when it's handed over, that it was already handed over, where to get it, or that we'll tell them when it's ready.
- **Not built yet**: deleting a payment recorded by mistake (`OrderPayment` is soft deletable, but there is no endpoint or undo yet), choosing a past date for a payment from the app (the API accepts `paidOn`). Online payments (Stripe) will record an `OrderPayment` per checkout.

## Contract

```json
POST /api/orders { "clientId": "…", "lines": [ … ], "paidAmount": 5000, "method": "Cash", "isDelivered": false, "delivery": "Pickup" }

POST /api/orders/{id}/payments { "amount": 7000, "method": "Transfer", "paidOn": null, "isDelivered": true, "isReady": false }
  (replaces PUT /api/orders/{id}/payment)

PUT /api/orders/{id}/cancellation   (now also with payments: returns everything paid; 409 order.not_cancellable once handed over)

GET /api/orders?owing=true        (replaces requested=true: orders with something left to pay)

Order → { "status": "Pending", "paymentStatus": "Partial", "paidAmount": 5000, "balance": 7000, "returnedAmount": 0,
          "lastPaidOn": "2026-10-20", "payments": [ { "id": "…", "amount": 5000, "method": "Cash", "paidOn": "2026-10-20" } ], … }
  (method and paidOn are gone; the student app gets status, paymentStatus, paidAmount, balance, lastPaidOn and expiresAt, null once there is a payment)
```

## Data

The migration `AddOrderPayments` creates `OrderPayments` and moves every paid order's payment there (its total, method, date and who recorded it). `Paid` orders without products become `Delivered` (their classes were already credited); the other `Paid` and the `Requested` ones become `Pending`. `Orders.Method`, `PaidOn` and `PaymentRecordedByUserId` are dropped, and `ClassPackPurchases.Method` becomes nullable: a pack handed over before any payment has no method.
