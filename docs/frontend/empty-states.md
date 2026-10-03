# Empty states

An empty list never offers a second button for the action the screen already has. The create action lives in one place, the floating action button (`FloatingActionButton`), whether the list is empty or not. The empty state only explains what is missing and points to that button.

## Why

- **One primary action per screen.** Material Design recommends a single floating action button for the main action of a screen. A centered "Create your first…" button next to it is the same action twice, competing for attention.
- **What well-known apps do.** Platform apps such as Google Keep, Gmail and Google Drive on Android, and Notes and Reminders on iOS, keep their create action in its usual place and show only a message on an empty list.
- **The button never moves.** The user learns where "New…" is on the first visit and finds it in the same place once the list fills up.

Considered and rejected: a centered button on the empty state and no floating button until the first item exists. It guides a brand-new user a little more, but the action moves the moment the first item is created, and it is not what the platform apps do.

## The rule

- Keep the floating action button visible when the list is empty. Never show it only when the list has items (`items.length > 0 ? <FloatingActionButton …/> : undefined`).
- `EmptyState` takes no children, so it cannot hold a button. Pass `createActionLabel` with the floating button's label and it adds "Tocá {action} para empezar." Leave it out when the member cannot create (no permission), so the hint never points to a button that is not there.
- Empty states for a filter or a search ("No hay clases el lunes", "No encontramos alumnos para…") don't need the hint: the list is not empty, the filter is.

```tsx
<EmptyState
  icon="classes"
  message={translate("classGroups.week.emptyTitle")}
  createActionLabel={canManageClassGroups ? translate("classGroups.week.newClassGroup") : undefined}
/>
```

## Enforced by

- The type of `EmptyState`: it has no `children`, so a button inside it does not compile.
- `app/src/ui/__tests__/When_a_list_is_empty/`:
  - `Then_the_empty_state_has_no_buttons` fails if any component closes an `</EmptyState>` tag (content inside it);
  - `Then_the_floating_button_stays_visible` fails if a `FloatingActionButton` is shown only when a list has items (`.length > 0 ? <FloatingActionButton`).
- Screen tests for Clases (`When_there_are_no_classes`), Alumnos (`When_there_are_no_students`) and Inicio (`When_the_day_has_no_classes`) check there is exactly one create button on the empty screen.
