import { CatalogImage } from "@/features/catalogImages/components/CatalogImage";

interface ProductPhotoProps {
  imageUrl: string | null;
  heightClassName: string;
  isDimmed?: boolean;
}

export function ProductPhoto({ imageUrl, heightClassName, isDimmed = false }: ProductPhotoProps) {
  return (
    <CatalogImage
      imageUri={imageUrl}
      placeholderIcon="products"
      className={`w-full ${heightClassName}`}
      isDimmed={isDimmed}
    />
  );
}
