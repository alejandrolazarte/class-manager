import { toFileForm } from "@/api/fileForm";
import { httpClient } from "@/api/httpClient";
import { toImageDataUri } from "@/features/brand/imageDataUri";
import { PickedLogoFile } from "@/features/brand/pickLogoFile";
import { Brand, BrandAudience, UpdateBrandRequest } from "@/features/brand/types";

const brandPaths: Record<BrandAudience, string> = {
  team: "/api/business/brand",
  family: "/api/family/brand",
};
const logoSegment = "logo";

function logoPath(audience: BrandAudience): string {
  return `${brandPaths[audience]}/${logoSegment}`;
}

export function getBrand(audience: BrandAudience): Promise<Brand> {
  return httpClient.get<Brand>(brandPaths[audience]);
}

export async function getBrandLogo(audience: BrandAudience): Promise<string> {
  return toImageDataUri(await httpClient.getBytes(logoPath(audience)));
}

export function updateBrand(request: UpdateBrandRequest): Promise<Brand> {
  return httpClient.put<Brand>(brandPaths.team, request);
}

export function uploadBrandLogo(file: PickedLogoFile): Promise<Brand> {
  return httpClient.putForm<Brand>(logoPath("team"), toFileForm(file));
}

export function removeBrandLogo(): Promise<Brand> {
  return httpClient.delete<Brand>(logoPath("team"));
}
