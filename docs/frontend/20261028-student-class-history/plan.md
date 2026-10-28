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

Today stays out of the history: today's classes come from the home response (`nextClasses`), with their actions.

## Tests

- Integration (`tests/ClassManager.Api.I.Tests/Endpoints/StudentAppClassHistory`): each status, classes before joining and from today on left out, makeups and private lessons, another family's and another business's student, an invalid month.
- App (`app/src/features/studentApp/__tests__`): a past day shows whether the student came, and Mes shows the month summary.
