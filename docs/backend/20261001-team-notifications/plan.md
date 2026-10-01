# Team notifications

Date: 2026-10-01

Coaches and owners had no notifications: a new order sent an email to the staff, and "no voy" was only visible inside the class roster. The team now gets **avisos**, stored in the app (bell and list) and, when the device allows it, as web push notifications.

## What is notified, and to whom

| Event | Recipients |
|---|---|
| A family says a student won't come (`PUT /api/family/.../absences/...`) | The coach of that session |
| A family books a makeup class | The coach of that session |
| A family places an order from the app | Members who manage orders (`Orders.Manage`) and brand owners, the same people who get the order email |

- **Coach of the session:** members linked to the session's effective instructor (the substitute when there is one).
- **No coach in the app:** when no member is linked to that instructor, the branch owners get it instead. This covers the common case of an owner who also teaches (sign-up creates the owner's instructor record, but it is not linked to the owner's membership), and instructors without an app account.

## Design

- `TeamNotification` (tenant-owned): recipient user, title, body, app URL, created and seen times. Title and body are shortened to 120 and 300 characters.
- `MemberPushSubscription` (tenant-owned): a separate table from the family `PushSubscriptions`, so a device used for both a family and a team account keeps both subscriptions. Both share the endpoint and key validation (`PushSubscriptionRules`).
- `ITeamNotificationService` is called by the absence and makeup use cases after saving; orders reuse `OrderNotificationService`. `TeamNotifier` stores the notifications and enqueues the push.
- The push pipeline is shared: `PushOutbox` carries `FamilyPush` and `TeamPush` jobs, `PushDispatcher` sends each to its subscriptions and removes the ones the push service reports as gone, and `PushWorker` runs them in the background. Nothing is sent when VAPID keys are not configured; the avisos are still stored.

## API

| Operation | Endpoint |
|---|---|
| List (latest 50) and unread count | `GET /api/team/notifications` |
| Mark all as seen | `PUT /api/team/notifications/seen` |
| Push key | `GET /api/team/push-key` |
| Subscribe / unsubscribe this device | `PUT` / `DELETE /api/team/push-subscription` |

All require a team session (`RequireMember`) and only return the caller's own avisos.

## App

- The bell sits in the Inicio header (the team app's landing screen) and opens **Avisos** (`/today/notifications`, inside the Inicio stack so back returns to Inicio); opening the list marks them as seen and tapping an aviso opens the session or the orders list.
- **Ajustes** gets the same notifications card as the family app to turn push on or off on this device; signing out unsubscribes the device.
- The family and team apps share `usePushNotifications` and `NotificationSettings`; each passes its own API channel.
