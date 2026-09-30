export interface AchievementLevel {
  name: string;
  requiredClasses: number;
}

export interface AchievementSettings {
  noticedAbsencesKeepStreak: boolean;
  levels: AchievementLevel[];
}
