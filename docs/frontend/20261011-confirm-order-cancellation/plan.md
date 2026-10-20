# Confirm before cancelling an order

Date: 2026-10-11

Status: built. Found while drawing the [Calma redesign](../20261011-calma-redesign/plan.md); it is a behavior change, so it is not part of the redesign.

## Problem

"Cancelar pedido" cancels the order on the first tap, with no question:

- Student app, Mis pedidos: `StudentAppOrderCard` calls `useCancelStudentAppOrder` directly and shows the "Pedido cancelado" toast.
- Team app, Cobros → Pedidos: the payment panel (`PaymentPanel`, opened with "Cobrar") calls `useCancelOrder` directly and shows the same toast.

A cancelled order can't be brought back, and it touches money. The rule in `CLAUDE.md` (Deleting) says such actions ask first with `ConfirmDialog`: a short question, an optional consequence, **Volver** and a verb, never "Sí, …". Today both buttons skip it.

## What to build

- Both buttons open a `ConfirmDialog` before cancelling. Nothing is sent until the user confirms; **Volver** closes the dialog and leaves the order as it was.
- The confirm button keeps today's label, "Cancelar pedido", and shows the loading state while the request runs; the "Pedido cancelado" toast stays.
- New texts, confirmed with the owner:
  - Student: "¿Cancelar el pedido n.º {number}?" · "Si lo querés de nuevo, vas a tener que pedirlo otra vez."
  - Team: "¿Cancelar el pedido n.º {number} de {client}?" (just "¿Cancelar el pedido n.º {number}?" for an order without a client) · "No se puede deshacer. Si tiene email, le avisamos que se canceló."
  `OrderNotificationService` only emails the client when the client has an account or an email, so the team text doesn't promise the email.
- No API change: the endpoints and the cancel rules stay.

## Tests

For each app, following the `When_`/`Then_` layout:

- When "Cancelar pedido" is pressed → then the confirm dialog opens and no cancel request is sent.
- When the dialog is dismissed with Volver → then the order is not cancelled.
- When the cancellation is confirmed → then the cancel request is sent and "Pedido cancelado" is shown.

The existing tests that cancel an order press the confirm button too.

## Order with the redesign

It can go before or after any Calma step. If it lands after step 2, the dialog already has the Calma look and motion; if before, it takes it with the rest of `ConfirmDialog`. Its PR follows the Calma PR rules for screenshots anyway (before/after of both cards with the dialog open).
