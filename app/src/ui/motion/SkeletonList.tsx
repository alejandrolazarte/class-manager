import { useEffect } from "react";
import { View } from "react-native";
import {
  cancelAnimation,
  useAnimatedStyle,
  useSharedValue,
  withRepeat,
  withTiming,
} from "react-native-reanimated";
import { translate } from "@/i18n/translate";
import { AnimatedView } from "@/ui/motion/animatedComponents";
import { motionDurations, motionEasing } from "@/ui/motion/motion";
import { useReducedMotion } from "@/ui/motion/useReducedMotion";

interface SkeletonListProps {
  rowCount?: number;
  className?: string;
}

const defaultRowCount = 4;
const dimmedOpacity = 0.45;
const repeatForever = -1;
const reverseEachCycle = true;

export function SkeletonList({ rowCount = defaultRowCount, className }: SkeletonListProps) {
  const isReducedMotion = useReducedMotion();
  const opacity = useSharedValue(1);
  useEffect(() => {
    if (isReducedMotion) {
      return;
    }
    opacity.set(
      withRepeat(
        withTiming(dimmedOpacity, { duration: motionDurations.progress, easing: motionEasing }),
        repeatForever,
        reverseEachCycle,
      ),
    );
    return () => cancelAnimation(opacity);
  }, [isReducedMotion, opacity]);
  const animatedStyle = useAnimatedStyle(() => ({ opacity: opacity.get() }));
  return (
    <AnimatedView
      accessible
      accessibilityRole="progressbar"
      accessibilityLabel={translate("common.loading")}
      style={animatedStyle}
      className={`gap-1 ${className ?? ""}`}
    >
      {Array.from({ length: rowCount }, (_, rowIndex) => (
        <View
          key={rowIndex}
          className="flex-row items-center gap-3 border-b border-border-subtle py-3.5"
        >
          <View className="h-9 w-9 rounded-full bg-muted" />
          <View className="flex-1 gap-2">
            <View className="h-2.5 w-3/4 rounded-full bg-muted" />
            <View className="h-2 w-2/5 rounded-full bg-muted" />
          </View>
        </View>
      ))}
    </AnimatedView>
  );
}
