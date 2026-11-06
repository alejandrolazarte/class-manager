const percentLocale = "es-AR";
const percentDecimals = 2;
const percentSign = " %";

export function formatPercent(percent: number): string {
  return (
    new Intl.NumberFormat(percentLocale, { maximumFractionDigits: percentDecimals }).format(
      percent,
    ) + percentSign
  );
}
