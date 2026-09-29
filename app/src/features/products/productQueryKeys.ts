export const productQueryKeys = {
  all: ["products"] as const,
  catalog: (includeInactive: boolean) =>
    [...productQueryKeys.all, "catalog", includeInactive] as const,
  movements: (productId: string) => [...productQueryKeys.all, "movements", productId] as const,
};
