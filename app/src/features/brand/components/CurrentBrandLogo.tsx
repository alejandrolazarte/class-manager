import { useCurrentBrand } from "@/features/brand/BrandProvider";
import { BrandLogo, BrandLogoSize } from "@/features/brand/components/BrandLogo";

interface CurrentBrandLogoProps {
  size?: BrandLogoSize;
  fallbackName?: string;
}

export function CurrentBrandLogo({ size = "small", fallbackName }: CurrentBrandLogoProps) {
  const currentBrand = useCurrentBrand();
  if (currentBrand?.brand) {
    return (
      <BrandLogo
        displayName={currentBrand.brand.displayName}
        logoUri={currentBrand.logoUri}
        isLogoLoading={currentBrand.isLogoLoading}
        size={size}
      />
    );
  }
  return fallbackName === undefined ? null : (
    <BrandLogo displayName={fallbackName} logoUri={null} size={size} />
  );
}
