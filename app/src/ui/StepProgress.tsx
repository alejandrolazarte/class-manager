import { View } from "react-native";
import { GrowingFill } from "@/ui/motion/GrowingFill";

interface StepProgressProps {
  segmentFills: readonly number[];
}

export function StepProgress({ segmentFills }: StepProgressProps) {
  return (
    <View className="flex-row gap-2" accessibilityElementsHidden>
      {segmentFills.map((fill, segmentIndex) => (
        <View
          key={segmentIndex}
          className="h-2.5 flex-1 overflow-hidden rounded-full bg-primary-soft"
        >
          <GrowingFill ratio={fill} className="rounded-full bg-primary" />
        </View>
      ))}
    </View>
  );
}
