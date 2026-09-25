export const businessQueryKeys = {
  all: ["business"] as const,
  current: () => [...businessQueryKeys.all, "current"] as const,
};
