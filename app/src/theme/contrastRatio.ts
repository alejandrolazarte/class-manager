import { toRgbChannels } from "@/theme/colorChannels";

const maximumChannelValue = 255;
const linearThreshold = 0.03928;
const linearDivisor = 12.92;
const gammaOffset = 0.055;
const gammaDivisor = 1.055;
const gammaExponent = 2.4;
const redWeight = 0.2126;
const greenWeight = 0.7152;
const blueWeight = 0.0722;
const flareOffset = 0.05;

function toLinearChannel(channel: number) {
  const normalizedChannel = channel / maximumChannelValue;
  return normalizedChannel <= linearThreshold
    ? normalizedChannel / linearDivisor
    : ((normalizedChannel + gammaOffset) / gammaDivisor) ** gammaExponent;
}

function relativeLuminance(hexColor: string) {
  const [red, green, blue] = toRgbChannels(hexColor).map(toLinearChannel);
  return redWeight * red + greenWeight * green + blueWeight * blue;
}

export function contrastRatio(firstHexColor: string, secondHexColor: string) {
  const firstLuminance = relativeLuminance(firstHexColor);
  const secondLuminance = relativeLuminance(secondHexColor);
  const lighter = Math.max(firstLuminance, secondLuminance);
  const darker = Math.min(firstLuminance, secondLuminance);
  return (lighter + flareOffset) / (darker + flareOffset);
}
