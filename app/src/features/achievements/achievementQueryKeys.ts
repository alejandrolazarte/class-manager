export const achievementQueryKeys = {
  all: ["achievements"] as const,
  settings: () => [...achievementQueryKeys.all, "settings"] as const,
};
