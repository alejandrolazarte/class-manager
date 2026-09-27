import { View } from "react-native";

type ProgressTone = "primary" | "success" | "onPrimary";

interface ProgressBarProps {
  ratio: number;
  tone?: ProgressTone;
  isThin?: boolean;
}

const fillClassNames: Record<ProgressTone, string> = {
  primary: "bg-primary",
  success: "bg-success",
  onPrimary: "bg-primary-foreground",
};

const trackClassNames: Record<ProgressTone, string> = {
  primary: "bg-muted",
  success: "bg-muted",
  onPrimary: "bg-primary-foreground/25",
};

const fullPercentage = 100;

export function ProgressBar({ ratio, tone = "primary", isThin = false }: ProgressBarProps) {
  const percentage = Math.max(0, Math.min(fullPercentage, Math.round(ratio * fullPercentage)));
  return (
    <View
      accessibilityRole="progressbar"
      accessibilityValue={{ min: 0, max: fullPercentage, now: percentage }}
      className={`flex-1 overflow-hidden rounded-full ${isThin ? "h-1.5" : "h-2"} ${trackClassNames[tone]}`}
    >
      <View
        style={{ width: `${percentage}%` }}
        className={`h-full rounded-full ${fillClassNames[tone]}`}
      />
    </View>
  );
}
