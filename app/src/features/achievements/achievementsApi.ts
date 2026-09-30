import { httpClient } from "@/api/httpClient";
import { AchievementSettings } from "@/features/achievements/types";

const achievementSettingsPath = "/api/business/achievements";

export function getAchievementSettings(): Promise<AchievementSettings> {
  return httpClient.get<AchievementSettings>(achievementSettingsPath);
}

export function updateAchievementSettings(
  settings: AchievementSettings,
): Promise<AchievementSettings> {
  return httpClient.put<AchievementSettings>(achievementSettingsPath, settings);
}
