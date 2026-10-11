import { PropsWithChildren, useEffect } from "react";
import { View } from "react-native";
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

interface LivePulseProps extends PropsWithChildren {
  ringClassName: string;
}

const startingRingOpacity = 0.55;
const ringGrowth = 0.6;
const repeatForever = -1;

export function LivePulse({ ringClassName, children }: LivePulseProps) {
  const isReducedMotion = useReducedMotion();
  const cycle = useSharedValue(0);
  useEffect(() => {
    if (isReducedMotion) {
      return;
    }
    cycle.set(
      withRepeat(
        withTiming(1, { duration: motionDurations.pulse, easing: motionEasing }),
        repeatForever,
      ),
    );
    return () => cancelAnimation(cycle);
  }, [isReducedMotion, cycle]);
  const ringStyle = useAnimatedStyle(() => ({
    opacity: startingRingOpacity * (1 - cycle.get()),
    transform: [{ scale: 1 + ringGrowth * cycle.get() }],
  }));
  return (
    <View className="relative">
      {isReducedMotion ? null : (
        <AnimatedView
          pointerEvents="none"
          style={ringStyle}
          className={`absolute inset-0 ${ringClassName}`}
        />
      )}
      {children}
    </View>
  );
}
