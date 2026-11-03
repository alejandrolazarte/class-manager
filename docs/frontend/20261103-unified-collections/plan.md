# Plan — Cobros in one view

## Why

Cobros had two views, Cuotas (later Clases) and Pedidos. One order can hold class packs and products, so no split by view worked: a pack sold with a product was listed in Pedidos while its money was counted somewhere else, and nothing said how much came in during the month.

Cobros is now one view of the month, with everything that is charged: monthly plans and orders (class packs and products).

## Screen

- **Header**: "Cobros" (or "Cobros de tus alumnos" with `payments.view.own`) and the month with arrows.
- **Summary** (`CollectionsSummary`): collected in the month out of the total (collected plus what is owed), and two columns: **Clases** (monthly plan payments plus the class pack share of order payments, "Planes X · Packs Y") and **Productos**. Below, how many owe and how much, or "Todo cobrado"; how many orders are waiting to be handed over; and the monthly plan payments of families that now pay with packs.
- **Filters**: Todos · Planes · Packs · Productos (`SegmentedControl`), plus the **Deben** and **Para entregar** chips, which can be turned off again. An order with packs and products shows, whole, under Packs and under Productos.
- **List**: monthly plan rows (`ClientFeeRow`, opens Registrar pago) and the month's orders (`OrderRow`), under "Plan mensual" and "Pedidos" when both show.
- The floating button opens a counter sale.
- Members who can't see orders get only the plans: no kind filter, no products column, no "Para entregar".

## Class balance

The families that pay with packs and their classes ("Quedan 3 clases", "Debe 2 clases") are not money, so they left Cobros for a **Saldo de clases** tab in Alumnos (members who see payments), with the Todos / Deben clases filter. Each row opens the family's card.

## API

- `GET /api/orders?month=2026-10` lists the orders of a month: not cancelled, and still owed, waiting to be handed over, or with a payment in that month. The list adds up to the order summary of the month.
- `GET /api/orders/summary?month=…` returns `Collected` and `Unpaid` for every order, plus `ClassPacksCollected` and `ProductsCollected`: each order's payments split in proportion to its lines (`OrderIncome`).
- `GET /api/fees` no longer returns `ClassPackSales`.
- `/fees?view=orders` is gone: the order aviso opens `/fees`.
