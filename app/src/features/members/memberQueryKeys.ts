export const memberQueryKeys = {
  all: ["members"] as const,
  current: () => [...memberQueryKeys.all, "current"] as const,
  team: () => [...memberQueryKeys.all, "team"] as const,
};
