import { useMutation, useQueryClient } from "@tanstack/react-query";
import { removeBrandLogo, updateBrand, uploadBrandLogo } from "@/features/brand/brandApi";
import { brandQueryKeys } from "@/features/brand/brandQueryKeys";
import { PickedLogoFile } from "@/features/brand/pickLogoFile";
import { Brand, UpdateBrandRequest } from "@/features/brand/types";

function useStoreBrand() {
  const queryClient = useQueryClient();
  return (brand: Brand) => {
    queryClient.setQueryData(brandQueryKeys.current("team"), brand);
    return queryClient.invalidateQueries({ queryKey: brandQueryKeys.all });
  };
}

export function useUpdateBrand() {
  const storeBrand = useStoreBrand();
  return useMutation({
    mutationFn: (request: UpdateBrandRequest) => updateBrand(request),
    onSuccess: storeBrand,
  });
}

export function useUploadBrandLogo() {
  const storeBrand = useStoreBrand();
  return useMutation({
    mutationFn: (file: PickedLogoFile) => uploadBrandLogo(file),
    onSuccess: storeBrand,
  });
}

export function useRemoveBrandLogo() {
  const storeBrand = useStoreBrand();
  return useMutation({ mutationFn: () => removeBrandLogo(), onSuccess: storeBrand });
}
