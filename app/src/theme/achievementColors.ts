export type MedalMetal = "bronze" | "silver" | "gold";

interface MetalColors {
  rimLight: string;
  rimDark: string;
  faceLight: string;
  faceDark: string;
  ink: string;
}

export const medalMetalColors: Record<MedalMetal, MetalColors> = {
  bronze: {
    rimLight: "#f0b48a",
    rimDark: "#8a4a22",
    faceLight: "#f8d9bd",
    faceDark: "#c07a48",
    ink: "#6e3410",
  },
  silver: {
    rimLight: "#f4f7f9",
    rimDark: "#6f7d88",
    faceLight: "#ffffff",
    faceDark: "#b3bec6",
    ink: "#3d4954",
  },
  gold: {
    rimLight: "#ffe27a",
    rimDark: "#a86a04",
    faceLight: "#fff4b8",
    faceDark: "#efb31f",
    ink: "#7a4d00",
  },
};

export const medalShineColor = "#ffffff";

export const flameColors = {
  outer: "#ea580c",
  inner: "#fde68a",
  glow: "#fcd34d",
} as const;

export const confettiColors = ["#fcd34d", "#4ec0ed", "#ffffff", "#ea580c", "#98dcfb", "#f472b6"];
