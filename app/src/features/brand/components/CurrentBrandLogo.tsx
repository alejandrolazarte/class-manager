import { useCurrentBrand } from "@/features/brand/BrandProvider";
import { BrandLogo, BrandLogoSize } from "@/features/brand/components/BrandLogo";

export function CurrentBrandLogo({ size = "small" }: { size?: BrandLogoSize }) {
  const currentBrand = useCurrentBrand();
  return currentBrand?.brand ? (
    <BrandLogo
      displayName={currentBrand.brand.displayName}
      logoUri={currentBrand.logoUri}
      size={size}
    />
  ) : null;
}
