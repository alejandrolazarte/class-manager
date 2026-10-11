import { cssInterop } from "nativewind";
import { Pressable } from "react-native";
import Animated from "react-native-reanimated";

export const AnimatedView = Animated.View;

export const AnimatedPressable = Animated.createAnimatedComponent(Pressable);

cssInterop(AnimatedView, { className: "style" });
cssInterop(AnimatedPressable, { className: "style" });
