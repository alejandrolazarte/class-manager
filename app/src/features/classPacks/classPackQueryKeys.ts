export const classPackQueryKeys = {
  all: ["classPacks"] as const,
  catalog: (includeInactive: boolean) =>
    [...classPackQueryKeys.all, "catalog", includeInactive] as const,
  balance: (clientId: string) => [...classPackQueryKeys.all, "balance", clientId] as const,
};
