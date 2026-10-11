import { PropsWithChildren, useEffect } from "react";
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

interface SkeletonFrameProps extends PropsWithChildren {
  className?: string;
}

const dimmedOpacity = 0.45;
const repeatForever = -1;
const reverseEachCycle = true;

export function SkeletonFrame({ className, children }: SkeletonFrameProps) {
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
      className={className}
    >
      {children}
    </AnimatedView>
  );
}
