import { palette } from "@/theme/palette";

export const productColorSwatches = {
  black: "#1f2328",
  charcoal: "#4a5058",
  gray: "#9aa3ab",
  light_gray: "#d5d9dd",
  white: "#ffffff",
  cream: "#f3ead6",
  beige: "#d9c7a7",
  camel: "#b8865b",
  brown: "#7b5236",
  burgundy: "#7a2638",
  red: "#c8363d",
  coral: "#f07a6a",
  pink: "#e58fb0",
  fuchsia: "#d63384",
  lilac: "#b9a3e3",
  violet: "#7d55c7",
  navy: "#1f2f5c",
  blue: "#2f5fb3",
  sky_blue: "#7cc3e8",
  turquoise: "#2bb5b0",
  mint: "#8fd5c3",
  green: "#2e8b57",
  olive: "#6b7a3a",
  yellow: "#f2c94c",
  orange: "#e8772e",
} as const;

export type ProductColorKey = keyof typeof productColorSwatches;

const lightLuminanceThreshold = 150;
const redWeight = 299;
const greenWeight = 587;
const blueWeight = 114;
const weightTotal = 1000;
const hexRadix = 16;

function isLightSwatch(swatch: string): boolean {
  const red = Number.parseInt(swatch.slice(1, 3), hexRadix);
  const green = Number.parseInt(swatch.slice(3, 5), hexRadix);
  const blue = Number.parseInt(swatch.slice(5, 7), hexRadix);
  return (
    (red * redWeight + green * greenWeight + blue * blueWeight) / weightTotal >
    lightLuminanceThreshold
  );
}

export function checkColorOn(swatch: string): string {
  return isLightSwatch(swatch) ? palette.gray900 : palette.white;
}
