import { useMutation, useQueryClient } from "@tanstack/react-query";
import {
  applyCatalogImageDraft,
  CatalogImageDraft,
} from "@/features/catalogImages/catalogImageDraft";
import { CatalogImage } from "@/features/catalogImages/types";
import { productQueryKeys } from "@/features/products/productQueryKeys";
import {
  addProductImage,
  createProduct,
  recordStockMovement,
  removeProductImage,
  reorderProductImages,
  setProductActive,
  updateProduct,
} from "@/features/products/productsApi";
import { RecordStockMovementRequest, SaveProductRequest } from "@/features/products/types";

function useInvalidateProducts() {
  const queryClient = useQueryClient();
  return () => queryClient.invalidateQueries({ queryKey: productQueryKeys.all });
}

export function useSaveProduct() {
  const invalidate = useInvalidateProducts();
  return useMutation({
    mutationFn: ({ productId, request }: { productId?: string; request: SaveProductRequest }) =>
      productId === undefined ? createProduct(request) : updateProduct(productId, request),
    onSuccess: invalidate,
  });
}

export function useApplyProductImages() {
  const invalidate = useInvalidateProducts();
  return useMutation({
    mutationFn: ({
      productId,
      draft,
      savedImages,
    }: {
      productId: string;
      draft: CatalogImageDraft;
      savedImages: CatalogImage[];
    }) =>
      applyCatalogImageDraft(draft, productId, savedImages, {
        add: addProductImage,
        remove: removeProductImage,
        reorder: reorderProductImages,
      }),
    onSettled: invalidate,
  });
}

export function useSetProductActive() {
  const invalidate = useInvalidateProducts();
  return useMutation({
    mutationFn: ({ productId, isActive }: { productId: string; isActive: boolean }) =>
      setProductActive(productId, isActive),
    onSuccess: invalidate,
  });
}

export function useRecordStockMovement(productId: string) {
  const invalidate = useInvalidateProducts();
  return useMutation({
    mutationFn: (request: RecordStockMovementRequest) => recordStockMovement(productId, request),
    onSuccess: invalidate,
  });
}
