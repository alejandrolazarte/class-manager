# Buttons

Every button in the app comes from `app/src/ui`. Each role has exactly one button, and each button has exactly one role. A screen picks the button for its role from the table below; it never styles a `Pressable` itself and never adds a variant for one screen.

## Why

- **One line, no exceptions.** Two buttons for the same role (a bordered and an outlined secondary, a text button and a link that look alike) make every screen a choice, and the app drifts into a different look per screen.
- **The look follows the role.** Someone who has seen "Guardar", "Cancelar" and "Quitar" once knows what any button in the app does by its look.
- **One place to change.** A redesign changes the primitive, and every screen takes it.

## Which button

### `Button`

Three levels in two tones, plus one for adding another item. Sizes: `large` (48 px) for the main button of a form or a screen, `small` (32 px, 13 px text, touch area stretched to 44 px) for an action inside a row next to its text (Cobrar, Devolver, Dar de baja, Cambiar, Reenviar), and `medium` (44 px) everywhere else (headers, sheets, dialogs, buttons under a section).

| Variant | Role | Examples |
|---|---|---|
| `primary` | The main action of the screen, one per screen | Guardar, Ingresar, Registrar pago, Cobrar |
| `secondary` | Any other action next to it, and every Cancelar and Volver (forms, sheets, dialogs) | Reintentar, Cancelar, Volver, Reactivar clase, Ajustar stock |
| `ghost` | An action in a header or a row, as text | Editar, Cambiar, Reenviar, Revocar |
| `danger` | Only the button that confirms something destructive | Cancelar la clase (in `ConfirmDialog` and the cancel panels) |
| `dangerOutline` | The destructive button that leads to that confirmation, or a destructive action that can be undone | Quitar del equipo, Cancelar pedido, Borrar rol, Desactivar clase |
| `dangerGhost` | A destructive action in a row, as text | Dar de baja, Quitar, Anular (a pack purchase) |
| `dashed` | Add another item inside a form | Agregar hijo/a u otra persona, Agregar productos o packs |

A button that switches between two states takes the variant of each state (`classGroup.isActive ? "dangerOutline" : "secondary"` for Desactivar clase / Reactivar clase).

### The rest

| Primitive | Role | Examples |
|---|---|---|
| `IconButton` (and `StepArrow`) | An action shown only as an icon; `accessibilityLabel` is required | ←, ✕, the month arrows, the news bell |
| `FloatingActionButton` | Create in a list; it stays visible when the list is empty ([Empty states](empty-states.md)) | Nueva clase, Nuevo alumno |
| `FloatingBarButton` | A floating bar that sums up what the user is building and opens it | Ver carrito (count and total) |
| `TonalButton` | A small shortcut next to a title | Hoy, Todos vinieron, Mis pedidos |
| `ActionTile` | Contact someone through another app | Llamar, WhatsApp, Email |
| `TextLink` | Go somewhere, or show and hide a section, as text | Ver todo, ¿Olvidaste tu contraseña?, Ver PDF |
| `InlineLink` | A link inside a sentence | the terms in the legal consent |
| `FooterLinks` | The legal links at the bottom of a screen | Términos, Privacidad |

`ghost` and `TextLink` look alike on purpose. Choose by what happens: `ghost` does something (edits, resends, changes), `TextLink` takes the user somewhere or opens a section.

## What every button shares

- Press feedback: it scales to 0.97 (`PressableScale`, see [Theming](theming.md#motion)).
- Disabled looks the same everywhere: grey (`disabled-foreground`), not see-through.
- While its action runs, `Button` shows a spinner and stays disabled (`isLoading`); a button that opens a section says so to screen readers (`isExpanded`).
- Texts and accessibility labels come from `es.json`, like every other text.

## What is not a button

Rows (`ListRow`, `HairlineRow`, `TimelineRow`), cards (a product, a pack), chips and selectors (`Chip`, `ChipGroup`, `SegmentedControl`, `Tabs`, color and theme pickers), calendar days and the attendance marks are pressable but are not buttons. They have their own primitives or are feature components listed in the test below.

## Adding a button

Use one of the buttons above. A new button, a new variant or a new tone is added only when no role above fits, and the pull request says:

1. which role is missing, and why none of the existing buttons covers it;
2. where it will be used (a role used once is usually an existing role);
3. that the new primitive goes in `app/src/ui` with its own test, is added to this page and replaces every screen that shows the same pattern.

A visual variant of an existing role (another border, another color for the same action) is never a reason for a new variant.

## Enforced by

- The types: `Button` accepts only the variants above, `TonalButton` only `primary`, `success` and `neutral`.
- `app/src/ui/__tests__/When_a_screen_needs_a_button/Then_it_uses_a_button_primitive` fails when a file outside `app/src/ui` uses `Pressable` or `PressableScale`. The files that are not buttons (cards, selectors, picker rows, attendance marks) are listed in the test, each with the reason; adding one there needs the same justification as a new button.

## History

- 2026-10-14: the button inventory (eight `Button` variants, seven other primitives and buttons built by hand in screens) was reduced to one button per role. `outline` was removed (its uses are `secondary`), "Cancelar" in `ghost` became `secondary`, the hand-made links and text buttons became `TextLink`, `Button` (`ghost`, `dangerGhost`) or `TonalButton` (`neutral`), the news bell an `IconButton`, the cart bar the `FloatingBarButton` primitive, and `IconButton` and `FooterLinks` got the press feedback and the grey disabled state. `Button` got the `small` size for actions inside rows, as the Calma boards draw them. The Calma design draws an outlined button ("Contorno") and a borderless "Volver" in the dialog; the app keeps one line instead (see the [Calma plan](20261011-calma-redesign/plan.md) log).
