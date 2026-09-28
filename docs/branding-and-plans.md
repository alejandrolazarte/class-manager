# Branding and plans

Date: 2026-09-28

Product decisions on what a business can brand, and which parts could be paid. Pricing itself is still an [open decision](mvp-plan.md#open-decisions); this only records where branding fits. The technical plan is [themes, business theme and business icon](frontend/20260928-themes-and-business-icon/plan.md).

## What each user sees

- A user in a business **with** its own theme starts with the business theme.
- A user in a business **without** one starts with the default theme (`aqua`).
- Any user can pick another theme at any time, unless the business locks its theme (paid, below).

## Free and paid

| Feature | Plan | Why |
|---|---|---|
| Built-in themes, light/dark | Free | Basic personalization; costs nothing to keep and makes the app feel like one's own from day one |
| Business icon from the catalog | Free | Same |
| Business theme from its brand colors, default for the team | Paid ("Marca propia" or part of a Pro plan) | Studios and chains with an established brand value it, and they are the ones that pay the most |
| Lock the business theme for members | Paid | Studios with a brand want coaches (and, later, students) to see it; an independent instructor doesn't need it |
| Custom logo instead of the catalog icon | Paid, later | Waits for something students see (a payment receipt) |

**When a business stops paying:** the colors stay stored but are no longer applied; everyone falls back to `aqua` or the theme they chose. Nothing breaks, and paying again brings the theme back.

**Timing:** while only the owner and coaches use the app, a business theme is nice to have, not a need. It becomes a strong selling point once students see something branded (receipts, a student portal, emails). Until then it can be free during the pilot as a hook, announced as part of the paid plan later.

## Brands with two or more colors

The business theme takes **up to two colors**:

| Color | Used for | Tokens |
|---|---|---|
| **Main** (required) | Buttons, selected tab, links, headers, the business icon background | `primary*` (exists today) |
| **Accent** (optional) | Small highlights only: the eyebrow above titles, the "Hoy" chip, badges, the floating add button | New `accent*` tokens |

- **Without an accent, `accent*` equals `primary*`.** Built-in themes do the same unless a design gives them one, so screens look as they do today.
- **The owner chooses which color is the main one.** Many brands have a color that doesn't work as the main UI color (yellow, light green, beige: white text on it is unreadable). The preview shows both options, and the derivation darkens or lightens each color until its text passes WCAG AA, so an unusable choice still becomes legible.
- **A third color or more is not used in the interface.** More than two colors in a working app makes states (paid, overdue, cancelled) harder to read, since those already use green, amber and red. Extra brand colors belong in the logo, which shows the full brand as it is.
- Status colors (success, warning, danger) never take brand colors: "paid" must look the same in every business.

Adding the accent means a new token family (see "Adding a token" in [theming](frontend/theming.md)), its contrast pairs, and a second nullable column (`Business.AccentColor`). It is a follow-up to the one-color business theme, not part of its first version.
