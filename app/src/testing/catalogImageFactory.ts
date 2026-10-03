import { PickedFile } from "@/api/fileForm";
import { CatalogImage } from "@/features/catalogImages/types";

export function buildPickedImage(overrides: Partial<PickedFile> = {}): PickedFile {
  return {
    name: "gorro.png",
    uri: "file:///gorro.png",
    mimeType: "image/png",
    size: 120_000,
    ...overrides,
  };
}

export function buildCatalogImages(count: number): CatalogImage[] {
  return Array.from({ length: count }, (_, index) => ({
    id: `image-${index + 1}`,
    url: `https://files.example.com/public-files/business/products/image-${index + 1}.png`,
  }));
}
