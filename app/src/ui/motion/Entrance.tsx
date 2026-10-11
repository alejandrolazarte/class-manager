import { PropsWithChildren, useEffect } from "react";
import { useWindowDimensions } from "react-native";
import { useAnimatedStyle, useSharedValue, withSpring, withTiming } from "react-native-reanimated";
import { AnimatedView } from "@/ui/motion/animatedComponents";
import { dialogInitialScale, motionDurations, motionEasing, softSpring } from "@/ui/motion/motion";
import { useReducedMotion } from "@/ui/motion/useReducedMotion";

export type EntranceKind = "fade" | "scale" | "slideUp" | "springRise" | "pop";

interface EntranceProps extends PropsWithChildren {
  kind: EntranceKind;
  className?: string;
}

const enteredProgress = 1;
const springRiseOffset = 24;

const durations: Record<Exclude<EntranceKind, "springRise" | "pop">, number> = {
  fade: motionDurations.dialog,
  scale: motionDurations.dialog,
  slideUp: motionDurations.sheet,
};

export function Entrance({ kind, className, children }: EntranceProps) {
  const isReducedMotion = useReducedMotion();
  const { height: windowHeight } = useWindowDimensions();
  const progress = useSharedValue(isReducedMotion ? enteredProgress : 0);
  useEffect(() => {
    if (isReducedMotion) {
      progress.set(enteredProgress);
      return;
    }
    progress.set(
      kind === "springRise" || kind === "pop"
        ? withSpring(enteredProgress, softSpring)
        : withTiming(enteredProgress, { duration: durations[kind], easing: motionEasing }),
    );
  }, [isReducedMotion, kind, progress]);
  const animatedStyle = useAnimatedStyle(() => {
    const value = progress.get();
    if (kind === "scale") {
      return {
        opacity: value,
        transform: [{ scale: dialogInitialScale + (1 - dialogInitialScale) * value }],
      };
    }
    if (kind === "pop") {
      return { transform: [{ scale: value }] };
    }
    if (kind === "slideUp") {
      return { transform: [{ translateY: (1 - value) * windowHeight }] };
    }
    if (kind === "springRise") {
      return {
        opacity: Math.min(1, value),
        transform: [{ translateY: (1 - value) * springRiseOffset }],
      };
    }
    return { opacity: value };
  });
  return (
    <AnimatedView style={animatedStyle} className={className}>
      {children}
    </AnimatedView>
  );
}
