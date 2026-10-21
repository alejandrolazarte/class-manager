# Order detail in Cobros › Pedidos

Status: built.

## Problem

Since orders can be paid in parts and handed over at any time ([orders paid in parts](../../backend/20261020-order-partial-payments/plan.md)), each row in Cobros › Pedidos grew up to six actions of the same weight (Cobrar, Avisar que está listo, Entregar, Pagos, Devolver, Cancelar pedido) in two lines, two pills of the same color for delivery and payment, and "Devuelto" next to "Pagado" once everything was given back. "Devolver" registered money given back without asking, while "Cancelar pedido" asked first.

## What changes

- **The list is for scanning.** A row shows the client, the total, the order line, the money as text (the existing "Pagó {paid} · falta {balance}", "Falta pagar {balance}", "Devuelto {amount}", or "Pagado") with a thin bar while it is partly paid, where it is picked up, and one pill for the delivery (plus "Listo"). Tapping the row opens the order. The payment pill is gone: an order with everything given back no longer says "Pagado".
- **One action per row**, the next step: "Cobrar" while something is owed, else "Entregar" while it was not handed over. "Cobrar" opens the same payment panel as before, now in a `BottomSheet`.
- **The order detail** (`/{tab}/orders/{orderId}`, in Cobros, Hoy and Ajustes) has every action that left the row: "Qué lleva" with the total; "Entrega" with "Avisar que está listo" and "Entregar"; "Pagos" with each payment and its delete button (and Deshacer); "Cobrar" fixed at the bottom while something is owed; and, after the content, "Devolver" (secondary) and "Cancelar pedido" (`dangerGhost`), only when they apply. No "⋯" menu: two actions don't need one.
- **"Devolver" asks first** with `ConfirmDialog`, like "Cancelar pedido": "¿Registrar la devolución?" and, for products, "Se registra la devolución de {amount}. Devolver el dinero queda a cargo de {business}." (for packs, the existing unused-classes note).
- **API**: `GET /api/orders/{id}` reads one order with the same access rules as the list (`404` for an order outside the member's scope).

Nothing is removed: every action, toast, undo and confirmation is still there, in the detail. The canvas used to agree on it: "Pedidos — rediseño UX".
