export const familyQueryKeys = {
  all: ["family"] as const,
  home: () => [...familyQueryKeys.all, "home"] as const,
  shop: () => [...familyQueryKeys.all, "shop"] as const,
  orders: () => [...familyQueryKeys.all, "orders"] as const,
};
