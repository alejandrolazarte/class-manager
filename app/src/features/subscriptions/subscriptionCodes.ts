export const planCodes = {
  free: "free",
  lite: "lite",
  pro: "pro",
  enterprise: "enterprise",
} as const;

export type PlanCode = (typeof planCodes)[keyof typeof planCodes];

export const featureCodes = {
  students: "students",
  branches: "branches",
  team: "team",
  classPacks: "class-packs",
  importExport: "import-export",
  customRoles: "custom-roles",
  familyApp: "family-app",
  shop: "shop",
  brand: "brand",
} as const;

export type FeatureCode = (typeof featureCodes)[keyof typeof featureCodes];

export const featureDisplayOrder: readonly string[] = Object.values(featureCodes);

export const countedFeatureCodes: readonly string[] = [
  featureCodes.students,
  featureCodes.branches,
  featureCodes.team,
];

export const subscriptionErrorCodes = {
  inactive: "subscription.inactive",
  featureNotInPlan: "feature.not_in_plan",
  featureLimitReached: "feature.limit_reached",
} as const;
