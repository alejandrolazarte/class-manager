export const brandsQueryKey = ["backofficeBrands"];

export function brandQueryKey(brandId: string) {
  return [...brandsQueryKey, brandId];
}
