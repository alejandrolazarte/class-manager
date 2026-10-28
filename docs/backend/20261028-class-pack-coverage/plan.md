# Class packs only pay the classes they cover

Date: 2026-10-28

Status: built. The owner chose this over keeping the old behavior or a mixed rule.

## Why

A pack is for some group classes, or for private lessons (P7 in [fee history and class packs](../20260927-fee-history-and-class-packs/plan.md)), but the balance ignored that: each attended class was paid by the valid pack that expires first, whatever it covered. A family with "Pack Aquagym" (only Aquagym, expiring first) and a private lesson pack had its private lessons paid with Aquagym classes; the total left was right, but the Aquagym classes ran out early or were lost when that pack expired. Booking from the student app already respected what each pack covers; the balance did not. The student's class history ([student class history](../../frontend/20261028-student-class-history/plan.md)) made it visible: it named the pack that paid each class.

## The rule

- A pack for **some group classes** pays only those classes.
- A pack for **any group class** (new option) pays every group class, including classes created after it was sold.
- A pack for **private lessons** pays only private lessons.
- Each attended class is paid by the purchase that covers it, is valid that day and has classes left, the one that expires first (then the oldest purchase), as before. If none covers it, the class is **unpaid**, as when there is no pack.

Packs stay one of two kinds; "any group class" is an option of the group kind, chosen with the "Cualquier clase grupal" switch in Ajustes → Packs de clases, which hides the class list. An empty class list never means "any": the form still requires a class when the switch is off, so forgetting to pick one can't create a pack that pays everything.

## Enrolling outside the pack

Enrolling a student who pays per class in a class none of their active packs covers still asks first ("¿Inscribir a {student} igual?"). It now checks each sale's coverage, so a pack for any group class never asks, and the message says what really happens: "Su pack no incluye {classGroup}. Si lo inscribís, las clases que tome quedan debidas hasta que compre un pack que la incluya." (before, it said each class was deducted from the pack anyway).

## What each sale keeps

A sale keeps what the pack covered when it was sold (`ClassPackPurchases.CoversAnyGroupClass` and `ClassPackPurchaseClassGroups`), next to the lesson duration it already kept. Editing a pack's classes later changes future sales only. Booking a pack class from the student app reads the same copy.

Migration `ClassPackCoverageOnPurchases` adds both and copies each existing sale's coverage from its pack's current classes. Existing packs all have chosen classes (the form required it), so they become strict; none becomes "any group class" on its own.

## Before deploying

Balances can change: a class paid today by a pack that doesn't cover it moves to another pack or becomes unpaid, in the client card, Cobros and the student app. Run `scripts/class-pack-coverage-report.sql` in the Azure portal (SQL database → Query editor) on the production database **before merging** the pull request: merging to `main` deploys it. It lists, per business and client, the attended group classes and private lessons that none of the client's packs covers.

To keep a family's balance as it is, add the missing classes to the pack **before merging**: the migration copies each pack's classes into its existing sales, and a pack edited after the deployment only changes later sales. "Cualquier clase grupal" only exists after the deployment, so it is for new sales. Otherwise the classes become unpaid and the team decides with the family.

The list is a review list, not the exact new balance: a class in a month the family paid with the monthly fee shows up too, although it never used a pack.

## Tests

- Unit (`tests/ClassManager.Core.U.Tests/Domain/ClassPacks`): a pack that doesn't cover the class leaves it unpaid; "any group class" pays a class it doesn't list; group and private lesson packs each pay their own kind; the covering pack pays even if another expires first; a sale keeps its classes when the pack changes; "any group class" keeps no class list and takes no duration.
- Integration: the option is saved and returned; a class no pack covers is owed; a sale still pays a class removed from its pack later; a student books any class with an "any group class" pack; `ClassPackPurchaseClassGroups` of another business is not returned.
- Unit (enrollments): a pack for any group class enrolls without asking.
- App: the switch saves without choosing classes, and editing such a pack shows the switch on and no class list.
