import { PropsWithChildren, useEffect } from "react";
import {
  cancelAnimation,
  useAnimatedStyle,
  useSharedValue,
  withRepeat,
  withTiming,
} from "react-native-reanimated";
import { AnimatedView } from "@/ui/motion/animatedComponents";
import { motionDurations, motionEasing } from "@/ui/motion/motion";
import { useReducedMotion } from "@/ui/motion/useReducedMotion";

const restingScale = 0.94;
const restingOpacity = 0.6;
const repeatForever = -1;
const reverseEachCycle = true;

export function Breathing({ children }: PropsWithChildren) {
  const isReducedMotion = useReducedMotion();
  const cycle = useSharedValue(0);
  useEffect(() => {
    if (isReducedMotion) {
      return;
    }
    cycle.set(
      withRepeat(
        withTiming(1, { duration: motionDurations.progress, easing: motionEasing }),
        repeatForever,
        reverseEachCycle,
      ),
    );
    return () => cancelAnimation(cycle);
  }, [isReducedMotion, cycle]);
  const animatedStyle = useAnimatedStyle(() => ({
    opacity: 1 - (1 - restingOpacity) * cycle.get(),
    transform: [{ scale: 1 - (1 - restingScale) * cycle.get() }],
  }));
  return <AnimatedView style={animatedStyle}>{children}</AnimatedView>;
}
