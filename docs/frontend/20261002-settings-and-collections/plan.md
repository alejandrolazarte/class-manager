# Plan — Ajustes grouped and Cobros (Cuotas | Pedidos)

Date: 2026-10-02

Follow-up to the Claude Design files "Ajustes" and "Cobros". The goal was to adopt their organization without losing any existing option.

## Ajustes

Before: one card with every row (each with a hint), then the notifications card, the appearance card and a sign-out button.

Now: the business card, then groups with an overline title, rows with a tinted icon tile and the current value on the right, and the app version at the bottom.

| Group (tile tone) | Rows | Value on the right |
|---|---|---|
| Negocio (main color) | Marca, Sedes, Cuota mensual | "Personalizada" when there are brand colors or a logo; current branch or count; the fee or "Sin definir" |
| Personas (accent) | Equipo, Roles, Profes, Logros y asistencia, Novedades | — |
| Tienda (main color) | Packs de clases, Productos, Pedidos (only for members who see orders but not Cobros) | Active packs and products |
| Tu teléfono (gray) | Apariencia, Notificaciones, Importar y exportar | Light/dark/system; "Activadas", "Desactivadas", "Bloqueadas" or "No disponibles" |
| Cuenta (gray; red only for Cerrar sesión) | Cambiar de cuenta, Cerrar sesión | — |

- **Icon tiles use the business theme, not the design's green and amber.** The design gives Personas and Tienda green and amber tiles, which are the status colors (paid, pending). Status colors never take brand colors ([branding and plans](../../branding-and-plans.md)), so those groups would ignore the owner's theme and borrow the meaning of "paid" and "pending". Tiles use the main color, the accent (equal to the main color when the brand has none) and gray; red only marks Cerrar sesión, the destructive action. A test checks that no settings tile uses the success or warning colors.
- **Apariencia** and **Notificaciones** open a bottom sheet with the same controls as before (light/dark/system, color themes or the brand lock, the push switch and its hint).
- Every row keeps the permission it had. A group with no visible rows is not shown.
- The design merges "Equipo y roles" in one row. Team and roles stay as two rows because the team screen doesn't link to roles, so merging them would hide the roles screen.
- The design's "Plan PRO · 1 sede" line on the business card is not shown: there are no plans yet ([branding and plans](../../branding-and-plans.md)).

## Cobros

The Cuotas tab is now **Cobros**. Members who can see orders get a switch at the top: **Cuotas | Pedidos · N**, where N is the number of orders in progress (unpaid plus paid and waiting to be handed over). The tab icon shows the same number as a badge.

- **Cuotas:** the screen it was, unchanged: month arrows, summary, Deben/Todos filter, families, and families paying per class.
- **Pedidos:** a summary card ("Por cobrar en pedidos": the unpaid total, how many are in progress, unpaid and to hand over), the existing filters and order cards with every action (confirm payment, mark ready, delivered, refund), and a floating "Venta en mostrador" button that opens `/fees/orders/new`.
- `/fees?view=orders` opens Cobros on Pedidos.
- Members who see payments but not orders only get Cuotas, without the switch.
- Pedidos leaves Ajustes for members who see Cobros. `/settings/orders` stays for members who see orders but not payments, and `/today/orders` stays for the order aviso.

Not taken from the design:
- The "Registrar pago" floating button on Cuotas: a payment needs a family, and there is no family picker yet. Payments are still recorded by opening the family from the list.
- The compact order rows: the existing cards keep the order actions.

## Tests

- Ajustes:
  - the rows are grouped;
  - Apariencia opens its options;
  - Pedidos is not in Ajustes when Cobros is visible, and is still there for members who see orders but not payments;
  - the existing tests for business settings, coach rows, team row and turning on notifications (now from the Notificaciones row) still pass.
- Cobros:
  - Pedidos can be opened from the switch;
  - the switch shows the number of orders in progress;
  - members without orders only see Cuotas;
  - the Pedidos summary shows the unpaid total.
