export const familyQueryKeys = {
  all: ["family"] as const,
  home: () => [...familyQueryKeys.all, "home"] as const,
  shop: () => [...familyQueryKeys.all, "shop"] as const,
  orders: () => [...familyQueryKeys.all, "orders"] as const,
  news: () => [...familyQueryKeys.all, "news"] as const,
  pushKey: () => [...familyQueryKeys.all, "push-key"] as const,
  pushSubscription: () => [...familyQueryKeys.all, "push-subscription"] as const,
  makeups: (studentId: string) => [...familyQueryKeys.all, "makeups", studentId] as const,
};
