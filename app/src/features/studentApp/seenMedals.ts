import { allMedals } from "@/features/studentApp/studentAppAchievements";
import { Medal } from "@/features/studentApp/types";

export interface SeenMedalsStorage {
  read: (studentId: string) => Promise<Medal[] | null>;
  write: (studentId: string, medals: readonly Medal[]) => Promise<void>;
}

const seenMedalsKeyPrefix = "seen-medals.";

export function seenMedalsStorageKey(studentId: string): string {
  return `${seenMedalsKeyPrefix}${studentId}`;
}

function isMedal(value: unknown): value is Medal {
  return allMedals.some((medal) => medal === value);
}

export function parseSeenMedals(serializedMedals: string | null): Medal[] | null {
  if (serializedMedals === null) {
    return null;
  }
  try {
    const parsed: unknown = JSON.parse(serializedMedals);
    return Array.isArray(parsed) ? parsed.filter(isMedal) : null;
  } catch {
    return null;
  }
}
