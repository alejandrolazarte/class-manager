import { View } from "react-native";

interface StepProgressProps {
  segmentFills: readonly number[];
}

function percentOf(fill: number): `${number}%` {
  return `${Math.round(Math.min(1, Math.max(0, fill)) * 100)}%`;
}

export function StepProgress({ segmentFills }: StepProgressProps) {
  return (
    <View className="flex-row gap-2" accessibilityElementsHidden>
      {segmentFills.map((fill, segmentIndex) => (
        <View
          key={segmentIndex}
          className="h-2.5 flex-1 overflow-hidden rounded-full bg-primary-soft"
        >
          <View className="h-full rounded-full bg-primary" style={{ width: percentOf(fill) }} />
        </View>
      ))}
    </View>
  );
}
