import { toRgbChannels } from "@/theme/colorChannels";

const hexadecimalRadix = 16;
const channelHexLength = 2;

function toHexChannel(channel: number): string {
  return Math.round(channel).toString(hexadecimalRadix).padStart(channelHexLength, "0");
}

export function mixColors(
  baseHexColor: string,
  targetHexColor: string,
  targetWeight: number,
): string {
  const base = toRgbChannels(baseHexColor);
  const target = toRgbChannels(targetHexColor);
  return `#${base
    .map((channel, index) => toHexChannel(channel + (target[index] - channel) * targetWeight))
    .join("")}`;
}
