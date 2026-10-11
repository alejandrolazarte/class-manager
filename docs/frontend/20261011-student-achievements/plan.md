# Logros that make progress exciting

Date: 2026-10-11

Status: proposal, to design and decide later. Logros is the screen where a student sees their own progress, so it matters for coming back to class. The owner wants it to feel exciting: reaching a level or a medal should be a moment, not a line that changed.

The [Calma redesign](../20261011-calma-redesign/plan.md) only restyles Logros (its board is in the design artifact) and keeps what it shows today. Everything below is new behavior with its own design, texts and PRs, so it follows its own plan, not the redesign's "no new texts" rule.

## What Logros shows today

- Student picker (families with more than one student).
- Level: number, name, "Faltan N clases para …" or "¡Llegó al nivel más alto!", and a progress bar.
- Stats: classes attended, weeks of streak, medals earned out of six.
- Camino de niveles: every level of the business with "Desde N clases" and "Nivel actual".
- Medallas: six medals (Primera clase, 10 clases, Racha de 4 semanas, Subió de nivel, Mes perfecto, 100 clases), earned or locked.
- Últimas 12 semanas: attended, missed or no classes, and the best streak.

Data the app already gets (`StudentAppAttendance`): `level`, `attendedClasses`, `streakWeeks`, `streakSince`, `bestStreakWeeks`, `recentWeeks`, `medals` and the business `levels`. Medals have no earned date, and nothing records what the student has already seen.

## Ideas to explore

To be drawn as variants in the design artifact and picked by the owner; not all of them need to ship.

1. **The level-up moment.** The first time the student opens the app after reaching a new level, a full-screen moment: the level badge grows into place, a short burst of color, "¡Subiste a Nivel 4 · Avanzado!" and the name of the next level. Shown once per level.
2. **Medal unlock.** A newly earned medal turns over from locked to earned with a "Nueva" tag until it is seen, and Inicio shows a small banner pointing to it.
3. **Progress as a journey.** The path of levels as a road the student walks: their avatar on the current level, the next level visible with its name, and the classes left as steps ("4 clases más"). Opening Logros fills the ring and counts up the classes once.
4. **How to earn each medal.** Locked medals say what unlocks them ("Vení 4 semanas seguidas"), so they are goals instead of grey icons.
5. **Streak that celebrates.** This week's attendance marked as soon as the student comes ("Esta semana ya viniste"), and the streak shown with more presence than a number. No guilt messages when it breaks: the copy celebrates the best streak and invites to start again.
6. **Level colors and badges.** Each level gets its own badge, optionally in the business brand colors, so levels look different and worth reaching.
7. **Instructor recognition.** When an instructor's comment arrives on the day of a level-up, show them together.

## Constraints

- **Minors use this screen.** No rankings or comparisons between students, no public sharing for minors, nothing that pushes guilt or anxiety (no "you're losing your streak" pushes). Any sharing idea needs the [ages and minors](../../ages-and-minors.md) rules checked first.
- **Motion.** Calma keeps motion short and turns it off with "Reduce motion". A celebration is the one place a longer moment (about 1–1.5 s) makes sense; it must be skippable with a tap, shown once, and replaced by a still version with reduced motion. This exception is decided in this plan, not assumed.
- **Backend.** "Shown once" and "Nueva" need to know what the student already saw: a per-student record of the last celebrated level and seen medals (or earned dates for medals), checked against the [student accounts](../../student-accounts.md) rules (one account can see several students).
- **Business settings.** Levels and names come from the business (Ajustes → Logros y asistencia, `app/src/features/achievements`); new visuals must work with 2 to 10 levels and any name length.
- **Texts.** New copy is written for this plan and reviewed by the owner; existing texts keep their wording.

## Steps

1. Draw two or three directions for Logros and the level-up moment in the design artifact, with the student app's Calma look.
2. Owner picks; record the decision here.
3. Backend: what was already celebrated or seen, if the chosen direction needs it.
4. Build after Calma step 4 (student app), so it starts from the new primitives.

## Open questions

- Which moments matter most: level-up, medals, streak, or all three?
- Should the business be able to add its own medals, or only the six fixed ones?
- Should the team see a student's level and medals (for example in the class roster) to congratulate them in class?
