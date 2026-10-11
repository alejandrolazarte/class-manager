import { useEffect } from "react";
import { useAnimatedStyle, useSharedValue, withTiming } from "react-native-reanimated";
import { AnimatedView } from "@/ui/motion/animatedComponents";
import { motionDurations, motionEasing } from "@/ui/motion/motion";
import { useReducedMotion } from "@/ui/motion/useReducedMotion";

interface GrowingFillProps {
  ratio: number;
  className: string;
}

const fullPercentage = 100;
const emptyRatio = 0;

export function GrowingFill({ ratio, className }: GrowingFillProps) {
  const isReducedMotion = useReducedMotion();
  const clampedRatio = Math.max(emptyRatio, Math.min(1, ratio));
  const fill = useSharedValue(isReducedMotion ? clampedRatio : emptyRatio);
  useEffect(() => {
    fill.set(
      isReducedMotion
        ? clampedRatio
        : withTiming(clampedRatio, { duration: motionDurations.progress, easing: motionEasing }),
    );
  }, [clampedRatio, isReducedMotion, fill]);
  const animatedStyle = useAnimatedStyle(() => ({ width: `${fill.get() * fullPercentage}%` }));
  return <AnimatedView style={animatedStyle} className={`h-full ${className}`} />;
}
