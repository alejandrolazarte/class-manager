import { Easing } from "react-native-reanimated";

export const motionDurations = {
  press: 140,
  dialog: 260,
  collapse: 280,
  screen: 300,
  indicator: 320,
  sheet: 320,
  listItemEnter: 380,
  progress: 800,
  pulse: 1800,
} as const;

export const motionEasing = Easing.bezier(0.2, 0.8, 0.2, 1);

export const softSpring = { damping: 18, stiffness: 220, mass: 1 } as const;

export const pressedScale = 0.97;

export const listItemEnterOffset = 8;

export const listItemEnterStaggerMilliseconds = 45;

export const maximumStaggeredListItems = 8;

export const dialogInitialScale = 0.96;
