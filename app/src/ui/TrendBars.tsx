import { View } from "react-native";

export type TrendBarsSize = "compact" | "full";

interface TrendBarsProps {
  values: readonly number[];
  size?: TrendBarsSize;
  isLowerBetter?: boolean;
  accessibilityLabel?: string;
}

const lowestBarShare = 0.15;
const flatBarShare = 0.6;
const percent = 100;
const fullSizeBarCount = 12;
const compactBarCount = 8;

const containerClassNames: Record<TrendBarsSize, string> = {
  compact: "h-[26px] w-16 gap-0.5",
  full: "h-28 w-full gap-1.5",
};

const barRadiusClassNames: Record<TrendBarsSize, string> = {
  compact: "rounded-sm",
  full: "rounded-md",
};

function barShares(values: readonly number[]): number[] {
  const lowest = Math.min(...values);
  const highest = Math.max(...values);
  if (highest === lowest) {
    return values.map(() => flatBarShare);
  }
  return values.map(
    (value) => lowestBarShare + ((1 - lowestBarShare) * (value - lowest)) / (highest - lowest),
  );
}

export function TrendBars({
  values,
  size = "compact",
  isLowerBetter = false,
  accessibilityLabel,
}: TrendBarsProps) {
  const shownValues = values
    .slice(-(size === "full" ? fullSizeBarCount : compactBarCount))
    .map((value) => (isLowerBetter ? -value : value));
  const shares = barShares(shownValues);
  return (
    <View
      accessible={accessibilityLabel !== undefined}
      accessibilityRole={accessibilityLabel === undefined ? undefined : "image"}
      accessibilityLabel={accessibilityLabel}
      testID="trend-bars"
      className={`flex-row items-end ${containerClassNames[size]}`}
    >
      {shares.map((share, index) => (
        <View
          key={`${index}-${share}`}
          testID={index === shares.length - 1 ? "trend-bar-latest" : "trend-bar"}
          style={{ height: `${Math.round(share * percent)}%` }}
          className={`flex-1 ${barRadiusClassNames[size]} ${index === shares.length - 1 ? "bg-primary" : "bg-primary-soft"}`}
        />
      ))}
    </View>
  );
}
