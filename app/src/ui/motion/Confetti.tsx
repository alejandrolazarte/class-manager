import { useEffect } from "react";
import { useWindowDimensions, View } from "react-native";
import {
  cancelAnimation,
  useAnimatedStyle,
  useSharedValue,
  withDelay,
  withRepeat,
  withTiming,
  Easing,
} from "react-native-reanimated";
import { confettiColors } from "@/theme/achievementColors";
import { AnimatedView } from "@/ui/motion/animatedComponents";
import { useReducedMotion } from "@/ui/motion/useReducedMotion";

const pieceCount = 24;
const pieceHeight = 14;
const startOffset = -40;
const fullTurnDegrees = 540;
const repeatForever = -1;
const baseFallMilliseconds = 3000;
const fallStepMilliseconds = 600;
const delayStepMilliseconds = 350;
const horizontalStep = 37;

interface ConfettiPieceProps {
  index: number;
  screenWidth: number;
  screenHeight: number;
}

function ConfettiPiece({ index, screenWidth, screenHeight }: ConfettiPieceProps) {
  const fall = useSharedValue(0);
  useEffect(() => {
    fall.set(
      withDelay(
        (index % 8) * delayStepMilliseconds,
        withRepeat(
          withTiming(1, {
            duration: baseFallMilliseconds + (index % 5) * fallStepMilliseconds,
            easing: Easing.linear,
          }),
          repeatForever,
        ),
      ),
    );
    return () => cancelAnimation(fall);
  }, [fall, index]);
  const pieceStyle = useAnimatedStyle(() => ({
    transform: [
      { translateY: startOffset + fall.get() * (screenHeight - startOffset) },
      { rotate: `${fall.get() * fullTurnDegrees}deg` },
    ],
  }));
  return (
    <AnimatedView
      style={[
        pieceStyle,
        {
          position: "absolute",
          top: 0,
          left: (index * horizontalStep) % Math.max(screenWidth, 1),
          width: 6 + (index % 3) * 2,
          height: pieceHeight,
          borderRadius: 2,
          backgroundColor: confettiColors[index % confettiColors.length],
        },
      ]}
    />
  );
}

export function Confetti() {
  const isReducedMotion = useReducedMotion();
  const { width, height } = useWindowDimensions();
  if (isReducedMotion) {
    return null;
  }
  return (
    <View
      testID="confetti"
      pointerEvents="none"
      accessibilityElementsHidden
      importantForAccessibility="no-hide-descendants"
      className="absolute inset-0 overflow-hidden"
    >
      {Array.from({ length: pieceCount }, (_, index) => (
        <ConfettiPiece key={index} index={index} screenWidth={width} screenHeight={height} />
      ))}
    </View>
  );
}
