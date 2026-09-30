import { hueDistance, toHueAndSaturation } from "@/theme/colorHue";
import { minimumStatusHueDistance, neutralSaturationLimit } from "@/theme/themeStatusHues";
import { neutralLightColors } from "@/theme/themes";

export type StatusColorName = "danger" | "warning" | "success";

const statusColorNames: readonly StatusColorName[] = ["danger", "warning", "success"];

export function findBrandColorClash(brandColor: string): StatusColorName | null {
  const brand = toHueAndSaturation(brandColor);
  if (brand.saturation < neutralSaturationLimit) {
    return null;
  }
  return (
    statusColorNames.find(
      (statusColorName) =>
        hueDistance(brand.hue, toHueAndSaturation(neutralLightColors[statusColorName]).hue) <
        minimumStatusHueDistance,
    ) ?? null
  );
}
