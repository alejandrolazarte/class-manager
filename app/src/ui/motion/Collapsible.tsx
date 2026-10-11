import { PropsWithChildren, useEffect, useRef, useState } from "react";
import { LayoutChangeEvent, View } from "react-native";
import { SharedValue, useAnimatedStyle, useSharedValue, withTiming } from "react-native-reanimated";
import { scheduleOnRN } from "react-native-worklets";
import { AnimatedView } from "@/ui/motion/animatedComponents";
import { motionDurations, motionEasing } from "@/ui/motion/motion";
import { useReducedMotion } from "@/ui/motion/useReducedMotion";

type CollapsiblePhase = "collapsed" | "expanding" | "expanded" | "collapsing";

interface CollapsibleProps extends PropsWithChildren {
  isExpanded: boolean;
}

const collapsedHeight = 0;

function animateOpen(
  height: SharedValue<number>,
  targetHeight: number,
  setPhase: (phase: CollapsiblePhase) => void,
) {
  height.set(
    withTiming(
      targetHeight,
      { duration: motionDurations.collapse, easing: motionEasing },
      (isFinished) => {
        if (isFinished) {
          scheduleOnRN(setPhase, "expanded");
        }
      },
    ),
  );
}

export function Collapsible({ isExpanded, children }: CollapsibleProps) {
  const isReducedMotion = useReducedMotion();
  const [phase, setPhase] = useState<CollapsiblePhase>(isExpanded ? "expanded" : "collapsed");
  const height = useSharedValue(collapsedHeight);
  const contentHeight = useRef(collapsedHeight);

  const [lastIsExpanded, setLastIsExpanded] = useState(isExpanded);
  if (isExpanded !== lastIsExpanded) {
    setLastIsExpanded(isExpanded);
    if (isReducedMotion) {
      setPhase(isExpanded ? "expanded" : "collapsed");
    } else if (isExpanded) {
      setPhase(phase === "expanded" ? phase : "expanding");
    } else {
      setPhase(phase === "collapsed" ? phase : "collapsing");
    }
  }

  useEffect(() => {
    if (phase !== "collapsing") {
      return;
    }
    height.set(contentHeight.current);
    height.set(
      withTiming(
        collapsedHeight,
        { duration: motionDurations.collapse, easing: motionEasing },
        (isFinished) => {
          if (isFinished) {
            scheduleOnRN(setPhase, "collapsed");
          }
        },
      ),
    );
  }, [phase, height]);

  useEffect(() => {
    if (phase === "collapsed") {
      contentHeight.current = collapsedHeight;
      height.set(collapsedHeight);
    }
    if (phase === "expanding" && contentHeight.current > collapsedHeight) {
      animateOpen(height, contentHeight.current, setPhase);
    }
  }, [phase, height]);

  const onContentLayout = (event: LayoutChangeEvent) => {
    const wasMeasured = contentHeight.current > collapsedHeight;
    contentHeight.current = event.nativeEvent.layout.height;
    if (phase === "expanding" && !wasMeasured) {
      animateOpen(height, contentHeight.current, setPhase);
    }
  };

  const isSettled = phase === "expanded";
  const animatedStyle = useAnimatedStyle(() => ({
    height: isSettled ? "auto" : height.get(),
  }));

  if (phase === "collapsed") {
    return null;
  }
  const isCollapsing = phase === "collapsing";
  return (
    <AnimatedView style={animatedStyle} className={isSettled ? "" : "overflow-hidden"}>
      <View
        onLayout={onContentLayout}
        pointerEvents={isCollapsing ? "none" : "auto"}
        accessibilityElementsHidden={isCollapsing}
        importantForAccessibility={isCollapsing ? "no-hide-descendants" : "auto"}
        className={isSettled ? "" : "absolute left-0 right-0 top-0"}
      >
        {children}
      </View>
    </AnimatedView>
  );
}
