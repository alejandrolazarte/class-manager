import { Medal, StudentAppAttendance, StudentAppLevel } from "@/features/studentApp/types";
import { MedalMetal } from "@/theme/achievementColors";
import { IconName } from "@/ui/Icon";

export const medalIcons: Record<Medal, IconName> = {
  FirstClass: "flag",
  TenClasses: "firstMilestone",
  FourWeekStreak: "streak",
  LeveledUp: "trendingUp",
  PerfectMonth: "verified",
  HundredClasses: "militaryTech",
};

export const allMedals = Object.keys(medalIcons) as Medal[];

export interface LevelProgress {
  current: StudentAppLevel | null;
  next: StudentAppLevel | null;
  classesToNext: number;
  percent: number;
}

const fullPercent = 100;

export function levelProgress(
  levels: readonly StudentAppLevel[],
  level: number,
  attendedClasses: number,
): LevelProgress {
  const current = levels[level - 1] ?? null;
  const next = levels[level] ?? null;
  if (current === null || next === null) {
    return { current, next, classesToNext: 0, percent: fullPercent };
  }
  const span = next.requiredClasses - current.requiredClasses;
  const done = Math.min(Math.max(attendedClasses - current.requiredClasses, 0), span);
  return {
    current,
    next,
    classesToNext: next.requiredClasses - current.requiredClasses - done,
    percent: Math.round((done / span) * fullPercent),
  };
}

export const medalMetals: Record<Medal, MedalMetal> = {
  FirstClass: "bronze",
  TenClasses: "silver",
  FourWeekStreak: "silver",
  LeveledUp: "gold",
  PerfectMonth: "gold",
  HundredClasses: "gold",
};

export type MedalGoalUnit = "classes" | "weeks";

export interface MedalGoal {
  medal: Medal;
  done: number;
  total: number;
  unit: MedalGoalUnit;
}

const firstClassGoal = 1;
const tenClassesGoal = 10;
const hundredClassesGoal = 100;
const streakMedalWeeks = 4;

export function medalGoal(medal: Medal, attendance: StudentAppAttendance): MedalGoal | null {
  const classesGoal = (total: number): MedalGoal => ({
    medal,
    done: Math.min(attendance.attendedClasses, total),
    total,
    unit: "classes",
  });
  switch (medal) {
    case "FirstClass":
      return classesGoal(firstClassGoal);
    case "TenClasses":
      return classesGoal(tenClassesGoal);
    case "HundredClasses":
      return classesGoal(hundredClassesGoal);
    case "FourWeekStreak":
      return {
        medal,
        done: Math.min(attendance.streakWeeks, streakMedalWeeks),
        total: streakMedalWeeks,
        unit: "weeks",
      };
    default:
      return null;
  }
}

export function nextMedalGoal(attendance: StudentAppAttendance): MedalGoal | null {
  return allMedals
    .filter((medal) => !attendance.medals.includes(medal))
    .map((medal) => medalGoal(medal, attendance))
    .filter((goal): goal is MedalGoal => goal !== null && goal.done < goal.total)
    .reduce<MedalGoal | null>(
      (closest, goal) =>
        closest === null || goal.done / goal.total > closest.done / closest.total ? goal : closest,
      null,
    );
}

export const streakGoalWeeks = [4, 8, 12, 26] as const;

export function nextStreakGoal(streakWeeks: number): number | null {
  return streakGoalWeeks.find((goalWeeks) => goalWeeks > streakWeeks) ?? null;
}

export function firstNewMedal(
  medals: readonly Medal[],
  seenMedals: readonly Medal[],
): Medal | null {
  return medals.find((medal) => !seenMedals.includes(medal)) ?? null;
}
