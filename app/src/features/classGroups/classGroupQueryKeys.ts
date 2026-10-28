export const classGroupQueryKeys = {
  all: ["classGroups"] as const,
  active: ["classGroups", "active"] as const,
  includingInactive: ["classGroups", "includingInactive"] as const,
  instructorWeek: (instructorId: string, ignoredClassGroupId: string) =>
    ["classGroups", "instructorWeek", instructorId, ignoredClassGroupId] as const,
};
