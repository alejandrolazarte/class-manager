import { httpClient } from "@/api/httpClient";
import { toImageDataUri } from "@/features/brand/imageDataUri";
import { PickedLogoFile } from "@/features/brand/pickLogoFile";
import { Brand, BrandAudience, UpdateBrandRequest } from "@/features/brand/types";

const brandPaths: Record<BrandAudience, string> = {
  team: "/api/business/brand",
  family: "/api/family/brand",
};
const logoSegment = "logo";
const fileFieldName = "file";
const genericImageMimeType = "image/png";

interface NativeFormFile {
  uri: string;
  name: string;
  type: string;
}

function logoPath(audience: BrandAudience): string {
  return `${brandPaths[audience]}/${logoSegment}`;
}

function logoForm(file: PickedLogoFile): FormData {
  const form = new FormData();
  if (file.webFile !== undefined) {
    form.append(fileFieldName, file.webFile, file.name);
  } else {
    const nativeFile: NativeFormFile = {
      uri: file.uri,
      name: file.name,
      type: file.mimeType ?? genericImageMimeType,
    };
    form.append(fileFieldName, nativeFile as unknown as Blob);
  }
  return form;
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
  return httpClient.putForm<Brand>(logoPath("team"), logoForm(file));
}

export function removeBrandLogo(): Promise<Brand> {
  return httpClient.delete<Brand>(logoPath("team"));
}
