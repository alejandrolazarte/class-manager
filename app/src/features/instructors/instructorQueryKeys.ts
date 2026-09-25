export const instructorQueryKeys = {
  all: ["instructors"] as const,
  active: ["instructors", "active"] as const,
  includingInactive: ["instructors", "includingInactive"] as const,
};
