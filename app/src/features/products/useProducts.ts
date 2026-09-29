import { useQuery } from "@tanstack/react-query";
import { productQueryKeys } from "@/features/products/productQueryKeys";
import { listProducts, listStockMovements } from "@/features/products/productsApi";

export function useProducts(includeInactive: boolean, { enabled = true } = {}) {
  return useQuery({
    queryKey: productQueryKeys.catalog(includeInactive),
    queryFn: () => listProducts(includeInactive),
    enabled,
  });
}

export function useStockMovements(productId: string) {
  return useQuery({
    queryKey: productQueryKeys.movements(productId),
    queryFn: () => listStockMovements(productId),
  });
}
