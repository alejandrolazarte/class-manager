import { View } from "react-native";
import { GrowingFill } from "@/ui/motion/GrowingFill";

type ProgressTone = "primary" | "success" | "warning";

interface ProgressBarProps {
  ratio: number;
  tone?: ProgressTone;
  isThin?: boolean;
}

const fillClassNames: Record<ProgressTone, string> = {
  primary: "bg-primary",
  success: "bg-success",
  warning: "bg-warning",
};

const fullPercentage = 100;

export function ProgressBar({ ratio, tone = "primary", isThin = false }: ProgressBarProps) {
  const percentage = Math.max(0, Math.min(fullPercentage, Math.round(ratio * fullPercentage)));
  return (
    <View
      accessibilityRole="progressbar"
      accessibilityValue={{ min: 0, max: fullPercentage, now: percentage }}
      className={`flex-1 overflow-hidden rounded-full bg-muted ${isThin ? "h-[3px]" : "h-1"}`}
    >
      <GrowingFill ratio={ratio} className={`rounded-full ${fillClassNames[tone]}`} />
    </View>
  );
}
