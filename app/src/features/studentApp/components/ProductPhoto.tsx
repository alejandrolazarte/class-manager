import { CatalogImage } from "@/features/catalogImages/components/CatalogImage";

interface ProductPhotoProps {
  imageUrl: string | null;
  className: string;
}

export function ProductPhoto({ imageUrl, className }: ProductPhotoProps) {
  return (
    <CatalogImage
      imageUri={imageUrl}
      placeholderIcon="products"
      className={`w-full ${className}`}
    />
  );
}
