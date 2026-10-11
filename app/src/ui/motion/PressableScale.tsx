import { GestureResponderEvent, PressableProps, StyleProp, ViewStyle } from "react-native";
import { useAnimatedStyle, useSharedValue, withTiming } from "react-native-reanimated";
import { AnimatedPressable } from "@/ui/motion/animatedComponents";
import { motionDurations, motionEasing, pressedScale } from "@/ui/motion/motion";
import { useReducedMotion } from "@/ui/motion/useReducedMotion";

interface PressableScaleProps extends Omit<PressableProps, "style"> {
  className?: string;
  style?: StyleProp<ViewStyle>;
}

const restingScale = 1;

export function PressableScale({
  onPressIn,
  onPressOut,
  style,
  ...pressableProps
}: PressableScaleProps) {
  const isReducedMotion = useReducedMotion();
  const scale = useSharedValue(restingScale);
  const animatedStyle = useAnimatedStyle(() => ({ transform: [{ scale: scale.get() }] }));
  const animateTo = (target: number) => {
    if (!isReducedMotion) {
      scale.set(withTiming(target, { duration: motionDurations.press, easing: motionEasing }));
    }
  };
  return (
    <AnimatedPressable
      {...pressableProps}
      style={[style, animatedStyle]}
      onPressIn={(event: GestureResponderEvent) => {
        animateTo(pressedScale);
        onPressIn?.(event);
      }}
      onPressOut={(event: GestureResponderEvent) => {
        animateTo(restingScale);
        onPressOut?.(event);
      }}
    />
  );
}
