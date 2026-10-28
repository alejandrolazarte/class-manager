# Student class history in Clases

Date: 2026-10-28

Status: built. The owner picked the design on an interactive canvas (today's screen next to the proposal) before the work started.

## Why

The student app showed classes from today on only: Clases could not go back before the current week, and a past day said "No hay clases este día" even when the student had a class. Attendance only reached the student as numbers (classes attended, streak, last 12 weeks in Logros), never as "which class, which day, did I come".

## What the student sees

Clases uses the same agenda block as the team's Inicio (`AgendaCalendar` in `src/ui`):

- **Día / Mes.** Día shows the week strip; Mes shows the month calendar. The arrows move a week or a month, and **Hoy** comes back to today.
- **How far.** Back 52 weeks, the same window as the streak (`AttendanceStreaks.LookBackWeeks`); forward up to the second week after this one, as before.
- **A past day** lists each class with what happened: **Fuiste**, **Recuperaste** (a makeup they came to), **Faltaste**, **Avisaste** (they gave notice), **Cancelada** or **Sin registrar** (the team didn't take attendance).
- **Which pack paid it.** A past class paid with a class pack adds a line under it: "Del Pack 8 clases comprado el 10 de octubre". The purchase date tells two purchases of the same pack apart. A class paid with the monthly fee, or with no pack left to pay it, shows nothing.
- **Today and later** keep the rows they had: Hoy, Próxima, No va, Recuperación, Reservada and Cancelada, with the class detail and every action.
- **Actions only from today on.** "Reservar clases del pack" and "Recuperar clases" show while the selected day is today or later; a past day only lists its classes.
- **Dots.** Each day of the week strip and the month calendar has a dot: green when they came, red when they missed a class, grey for classes without attendance (still to come, or not recorded). A day where every class was cancelled is drawn as cancelled, as on the team calendar.
- **Month summary.** In Mes, "Fuiste a X de Y clases este mes" counts the past classes that were not cancelled.

## API

`GET /api/student-app/students/{studentId}/class-history?month=yyyy-MM` (`GetStudentAppClassHistoryUseCase`) returns the classes of that month before today, ordered by date and time. The student must belong to the signed-in account (`AccountStudentRules`), as for makeups and pack classes; another family's or business's student is `404`, and a bad month is `400`.

Which classes:

- Group classes on every day the student was enrolled (`IEnrollmentRepository.ListByStudentInPeriodAsync`), where the class group meets, from the enrollment start to its end. Only active class groups, as in the team agenda and the student home.
- Makeup and pack class bookings of the month.
- Private lessons the student was part of.

The status of a group class comes from its session and the student's attendance (`IAttendanceRepository.ListByStudentBetweenAsync`): cancelled session → Cancelled; present → Attended; an absence notice → AbsenceNotified; absent → Absent; nothing recorded → Unmarked. A private lesson uses its own attendance and cancellation.

Which pack paid a class is not stored: the class balance is recalculated every time (`ClassBalance.Calculate`). Classes are taken in date order, and each one is paid by the valid purchase that covers it and has classes left, the one that expires first (then the oldest purchase); see [class pack coverage](../../backend/20261028-class-pack-coverage/plan.md). The calculation now also returns that pairing (`ClassBalance.PaidClasses`), and the history reads it for the student's classes (`PaidWithPack`: the pack name and purchase date). Since it is a calculation, a later change (a corrected old attendance, a pack sold with an earlier date) can move a class to another purchase; the total left is always right. Packs belong to the client (the family), but the history only shows the selected student's classes.

Today stays out of the history: today's classes come from the home response (`nextClasses`), with their actions.

## Tests

- Unit: with two valid packs, each class names the purchase that paid it.
- Integration (`tests/ClassManager.Api.I.Tests/Endpoints/StudentAppClassHistory`): each status, the pack that paid a class and none for a monthly fee, classes before joining and from today on left out, makeups and private lessons, another family's and another business's student, an invalid month.
- App (`app/src/features/studentApp/__tests__`): a past day shows whether the student came and which pack paid it, offers no booking or makeup, and Mes shows the month summary.
