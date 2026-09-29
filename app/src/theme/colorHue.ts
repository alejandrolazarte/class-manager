import { toRgbChannels } from "@/theme/colorChannels";

const maximumChannelValue = 255;
const degreesPerSextant = 60;
const fullTurnDegrees = 360;
const halfTurnDegrees = 180;

export interface HueAndSaturation {
  hue: number;
  saturation: number;
}

export function toHueAndSaturation(hexColor: string): HueAndSaturation {
  const [red, green, blue] = toRgbChannels(hexColor).map(
    (channel) => channel / maximumChannelValue,
  );
  const maximum = Math.max(red, green, blue);
  const minimum = Math.min(red, green, blue);
  const chroma = maximum - minimum;
  if (chroma === 0) {
    return { hue: 0, saturation: 0 };
  }
  const lightness = (maximum + minimum) / 2;
  const saturation = chroma / (1 - Math.abs(2 * lightness - 1));
  const sextant =
    maximum === red
      ? ((green - blue) / chroma + 6) % 6
      : maximum === green
        ? (blue - red) / chroma + 2
        : (red - green) / chroma + 4;
  return { hue: sextant * degreesPerSextant, saturation };
}

export function hueDistance(firstHue: number, secondHue: number): number {
  const difference = Math.abs(firstHue - secondHue) % fullTurnDegrees;
  return difference > halfTurnDegrees ? fullTurnDegrees - difference : difference;
}
