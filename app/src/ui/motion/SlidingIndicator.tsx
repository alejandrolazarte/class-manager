import { useCallback, useEffect, useRef, useState } from "react";
import { LayoutChangeEvent, LayoutRectangle } from "react-native";
import { useAnimatedStyle, useSharedValue, withTiming } from "react-native-reanimated";
import { AnimatedView } from "@/ui/motion/animatedComponents";
import { motionDurations, motionEasing } from "@/ui/motion/motion";
import { useReducedMotion } from "@/ui/motion/useReducedMotion";

export function useOptionLayouts<TKey extends string>() {
  const [layouts, setLayouts] = useState<Partial<Record<TKey, LayoutRectangle>>>({});
  const onOptionLayout = useCallback(
    (key: TKey) => (event: LayoutChangeEvent) => {
      const layout = event.nativeEvent.layout;
      setLayouts((current) => {
        const previous = current[key];
        const isSame =
          previous !== undefined &&
          previous.x === layout.x &&
          previous.y === layout.y &&
          previous.width === layout.width &&
          previous.height === layout.height;
        return isSame ? current : { ...current, [key]: layout };
      });
    },
    [],
  );
  return { layouts, onOptionLayout };
}

interface SlidingIndicatorProps {
  layout: LayoutRectangle | undefined;
  className: string;
}

export function SlidingIndicator({ layout, className }: SlidingIndicatorProps) {
  const isReducedMotion = useReducedMotion();
  const left = useSharedValue(0);
  const top = useSharedValue(0);
  const width = useSharedValue(0);
  const height = useSharedValue(0);
  const hasPlaced = useRef(false);
  const x = layout?.x;
  const y = layout?.y;
  const layoutWidth = layout?.width;
  const layoutHeight = layout?.height;
  useEffect(() => {
    if (
      x === undefined ||
      y === undefined ||
      layoutWidth === undefined ||
      layoutHeight === undefined
    ) {
      return;
    }
    const targets = [
      [left, x],
      [top, y],
      [width, layoutWidth],
      [height, layoutHeight],
    ] as const;
    const slides = hasPlaced.current && !isReducedMotion;
    for (const [value, target] of targets) {
      value.set(
        slides
          ? withTiming(target, { duration: motionDurations.indicator, easing: motionEasing })
          : target,
      );
    }
    hasPlaced.current = true;
  }, [x, y, layoutWidth, layoutHeight, isReducedMotion, left, top, width, height]);
  const animatedStyle = useAnimatedStyle(() => ({
    left: left.get(),
    top: top.get(),
    width: width.get(),
    height: height.get(),
  }));
  if (layout === undefined) {
    return null;
  }
  return (
    <AnimatedView pointerEvents="none" style={animatedStyle} className={`absolute ${className}`} />
  );
}
