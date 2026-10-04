export const studentAppQueryKeys = {
  all: ["student"] as const,
  home: () => [...studentAppQueryKeys.all, "home"] as const,
  shop: () => [...studentAppQueryKeys.all, "shop"] as const,
  orders: () => [...studentAppQueryKeys.all, "orders"] as const,
  news: () => [...studentAppQueryKeys.all, "news"] as const,
  pushKey: () => [...studentAppQueryKeys.all, "push-key"] as const,
  pushSubscription: () => [...studentAppQueryKeys.all, "push-subscription"] as const,
  makeups: (studentId: string) => [...studentAppQueryKeys.all, "makeups", studentId] as const,
  packClasses: (studentId: string) =>
    [...studentAppQueryKeys.all, "pack-classes", studentId] as const,
};
