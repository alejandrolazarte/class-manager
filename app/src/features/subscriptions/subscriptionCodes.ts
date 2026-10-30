export const planCodes = {
  free: "free",
  lite: "lite",
  pro: "pro",
  enterprise: "enterprise",
} as const;

export const featureCodes = {
  students: "students",
  branches: "branches",
  team: "team",
  classPacks: "class-packs",
  importExport: "import-export",
  customRoles: "custom-roles",
  studentApp: "student-app",
  shop: "shop",
  brand: "brand",
  documentStorage: "document-storage",
} as const;

export type FeatureCode = (typeof featureCodes)[keyof typeof featureCodes];

export const featureDisplayOrder: readonly string[] = Object.values(featureCodes);

export const countedFeatureCodes: readonly string[] = [
  featureCodes.students,
  featureCodes.branches,
  featureCodes.team,
  featureCodes.documentStorage,
];

export const selfServicePlanCodes: readonly string[] = [planCodes.lite, planCodes.pro];

export const subscriptionErrorCodes = {
  inactive: "subscription.inactive",
  featureNotInPlan: "feature.not_in_plan",
  featureLimitReached: "feature.limit_reached",
} as const;
