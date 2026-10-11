const megabytesPerGigabyte = 1024;
const gigabyteDecimals = 1;
const megabyteUnit = "MB";
const gigabyteUnit = "GB";
const trailingZeroDecimal = /\.0$/;

export function formatMegabytes(megabytes: number): string {
  if (megabytes < megabytesPerGigabyte) {
    return `${megabytes} ${megabyteUnit}`;
  }
  const gigabytes = (megabytes / megabytesPerGigabyte)
    .toFixed(gigabyteDecimals)
    .replace(trailingZeroDecimal, "")
    .replace(".", ",");
  return `${gigabytes} ${gigabyteUnit}`;
}
