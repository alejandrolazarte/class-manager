# Navigation

The team app has five tabs (Inicio, Clases, Alumnos, Cobros, Ajustes), each with its own stack. Two rules keep "back" predictable.

## 1. A screen never leaves its tab

Opening a screen that lives in another tab's stack switches tabs, and back then stays in that other tab (for example, Avisos opened from Inicio used to return to Ajustes). So every screen a tab opens is registered in that tab's stack:

- **Shared screens** are registered in every tab that opens them, with the route file re-exporting the same screen component. Examples:
  - the client subtree `clients/[clientId]/…` (detail, new student, sell pack, counter sale, payment, new class pack) lives under both `/students` and `/fees`;
  - Pedidos is the second view of Cobros (`/fees?view=orders`), and also lives under `/today/orders` (the order aviso opens the Inicio one) and `/settings/orders` (for members who see orders but not Cobros); the counter sale lives under each of them (`/fees/orders/new`, …);
  - the new-instructor form lives under `/settings/instructors/new` and `/classes/instructors/new`;
  - the default monthly fee form lives under `/settings/monthly-fee` and `/fees/monthly-fee`.
- **Tab-aware routes**: shared screens build their links with the tab they were opened from (`routes.clientDetail(tab, clientId)`, `routes.orders(tab)`), using `useCurrentTab(clientTabs)` / `useCurrentTab(orderTabs)` from `src/navigation/useCurrentTab.ts`.
- Opening a tab root (`/today`, `/fees`, …) is allowed: it is a deliberate tab switch.
- Every tab stack sets `initialRouteName: "index"`, so a screen opened from a push notification or a link still has the tab root under it, and back returns there.

Enforced by `src/navigation/__tests__/When_a_tab_screen_opens_another_screen/`:

- `Then_it_stays_in_its_own_tab` follows the imports of every route file in each tab (so a shared screen is checked once per tab that registers it) and fails on any `routes.*` link into another tab's nested screens;
- `Then_the_screen_exists_in_that_tab` fails when a link has no screen in the tab it points to.

## 2. ✕ closes a form, ← goes back

Following Material Design (full-screen dialogs use only ✕) and Apple's HIG (modal tasks are dismissed, navigation goes back):

- **✕ (Cerrar)**: screens where the user fills in or edits something (create/edit forms, record a payment, sell a pack, pick a student to enroll). Closing discards what was typed.
- **← (Volver)**: screens that show information or lists (detail screens, Avisos, Pedidos, settings lists). Going back loses nothing.

Both call `router.back()`, and the Android back button behaves the same. Loading and error states of a form keep ✕ (`SettingsItemState navigation="close"`), so the icon doesn't change while the form loads.

Enforced by `src/navigation/__tests__/When_a_screen_edits_a_form/`: a screen with `useForm(` or `SettingsFormScreenLayout` must not show ←.

## 3. Back closes the sheet on top, not the screen behind it

Sheets, dialogs and full-screen pickers (`BottomSheet`, `FilterSheet`, `ConfirmDialog`, `FullScreenPage`, `SearchPage`) are not routes: they are a `Modal` drawn over the screen that owns them. Pressing back (the Android back button, a back gesture or the browser's back) closes only the one on top and leaves the screen as it was.

- **Android and iOS apps**: the native `Modal` receives the back press and calls its `onRequestClose`.
- **Web and the installed web app (PWA)**: a `Modal` knows nothing about browser history, so back used to move the router to the previous page. A sheet owned by a tab screen (Ajustes) stayed open over the previous tab, because tabs keep their screens mounted; a sheet owned by a stack screen disappeared together with the screen. Now each of these primitives calls `useCloseOnBack`, which pushes a history entry (same URL, same router id) when the sheet opens. Back pops that entry: a capture `popstate` listener closes the top sheet and stops the event before expo-router sees it. A sheet closed any other way (its button, the backdrop) removes its own entry, so the next back goes to the previous page. It removes it after the navigation started in the same tap has reached the browser history (expo-router writes it in a microtask, so the sheet waits for the next task): a sheet whose button closes it and opens another screen ("Ver planes") sees the new screen on top and leaves its entry behind it, instead of going back and undoing the navigation; if the screen that owns it is removed (`router.back()` after confirming), the entry is removed first (`beforeRemove`) so the router goes back to the right page.

A new sheet-like component builds on these primitives, or calls `useCloseOnBack(onClose)` itself. The rules (`src/navigation/layerHistory.ts`) are covered by `src/navigation/__tests__/When_back_is_pressed_with_a_sheet_open/` and its siblings.
