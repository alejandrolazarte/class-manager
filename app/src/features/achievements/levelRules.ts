import { AchievementLevel } from "@/features/achievements/types";
import { TranslationKey } from "@/i18n/translate";

export const minLevels = 2;
export const maxLevels = 10;
export const levelNameMaxLength = 40;

export function levelErrorOf(levels: readonly AchievementLevel[]): TranslationKey | null {
  if (levels.length < minLevels || levels.length > maxLevels) {
    return "achievements.settings.errors.levelCount";
  }
  if (levels.some((level) => level.name.trim().length === 0)) {
    return "achievements.settings.errors.nameRequired";
  }
  if (levels.some((level) => level.name.trim().length > levelNameMaxLength)) {
    return "achievements.settings.errors.nameTooLong";
  }
  if (levels[0]?.requiredClasses !== 0) {
    return "achievements.settings.errors.firstLevel";
  }
  const increases = levels.every(
    (level, index) =>
      Number.isInteger(level.requiredClasses) &&
      (index === 0 || level.requiredClasses > (levels[index - 1]?.requiredClasses ?? 0)),
  );
  return increases ? null : "achievements.settings.errors.classesMustIncrease";
}
