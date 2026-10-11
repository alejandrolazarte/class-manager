import { PropsWithChildren, useEffect } from "react";
import { useAnimatedStyle, useSharedValue, withDelay, withTiming } from "react-native-reanimated";
import { AnimatedView } from "@/ui/motion/animatedComponents";
import {
  listItemEnterOffset,
  listItemEnterStaggerMilliseconds,
  maximumStaggeredListItems,
  motionDurations,
  motionEasing,
} from "@/ui/motion/motion";
import { useReducedMotion } from "@/ui/motion/useReducedMotion";

interface AnimatedListItemProps extends PropsWithChildren {
  index: number;
  className?: string;
}

const hiddenOpacity = 0;
const visibleOpacity = 1;

export function AnimatedListItem({ index, className, children }: AnimatedListItemProps) {
  const isReducedMotion = useReducedMotion();
  const entersAnimated = !isReducedMotion && index < maximumStaggeredListItems;
  const progress = useSharedValue(entersAnimated ? hiddenOpacity : visibleOpacity);
  useEffect(() => {
    if (entersAnimated) {
      progress.set(
        withDelay(
          index * listItemEnterStaggerMilliseconds,
          withTiming(visibleOpacity, {
            duration: motionDurations.listItemEnter,
            easing: motionEasing,
          }),
        ),
      );
    }
  }, [entersAnimated, index, progress]);
  const animatedStyle = useAnimatedStyle(() => ({
    opacity: progress.get(),
    transform: [{ translateY: (visibleOpacity - progress.get()) * listItemEnterOffset }],
  }));
  return (
    <AnimatedView style={animatedStyle} className={className}>
      {children}
    </AnimatedView>
  );
}
