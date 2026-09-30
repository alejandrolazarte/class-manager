import { FamilyLevel, Medal } from "@/features/family/types";
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
  current: FamilyLevel | null;
  next: FamilyLevel | null;
  classesToNext: number;
  percent: number;
}

const fullPercent = 100;

export function levelProgress(
  levels: readonly FamilyLevel[],
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
