import { AchievementSettings } from "@/features/achievements/types";

export function buildAchievementSettings(
  overrides: Partial<AchievementSettings> = {},
): AchievementSettings {
  return {
    noticedAbsencesKeepStreak: true,
    levels: [
      { name: "Inicial", requiredClasses: 0 },
      { name: "Base", requiredClasses: 10 },
      { name: "Explorador", requiredClasses: 25 },
    ],
    ...overrides,
  };
}
