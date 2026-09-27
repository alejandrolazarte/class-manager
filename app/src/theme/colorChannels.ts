const hexColorPattern = /^#([0-9a-f]{2})([0-9a-f]{2})([0-9a-f]{2})$/i;
const hexadecimalRadix = 16;

export function toRgbChannels(hexColor: string): [number, number, number] {
  const match = hexColorPattern.exec(hexColor);
  if (!match) {
    throw new Error(`Theme colors must be six digit hex values, received "${hexColor}".`);
  }
  return [
    parseInt(match[1], hexadecimalRadix),
    parseInt(match[2], hexadecimalRadix),
    parseInt(match[3], hexadecimalRadix),
  ];
}
