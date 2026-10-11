import { Stack } from "expo-router";
import { ComponentProps } from "react";
import { motionDurations } from "@/ui/motion/motion";
import { useReducedMotion } from "@/ui/motion/useReducedMotion";

type StackScreenOptions = Exclude<
  NonNullable<ComponentProps<typeof Stack>["screenOptions"]>,
  (...parameters: never[]) => unknown
>;

export function useStackScreenOptions(): StackScreenOptions {
  const isReducedMotion = useReducedMotion();
  return {
    headerShown: false,
    animation: isReducedMotion ? "none" : "slide_from_right",
    animationDuration: motionDurations.screen,
  };
}
