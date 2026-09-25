export const classGroupQueryKeys = {
  all: ["classGroups"] as const,
  active: ["classGroups", "active"] as const,
  includingInactive: ["classGroups", "includingInactive"] as const,
};
