import { PickedFile } from "@/api/fileForm";

export const catalogImageUrl = "https://files.example.com/public-files/business/products/cap.png";

export function buildPickedImage(overrides: Partial<PickedFile> = {}): PickedFile {
  return {
    name: "gorro.png",
    uri: "file:///gorro.png",
    mimeType: "image/png",
    size: 120_000,
    ...overrides,
  };
}
