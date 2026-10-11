import { View } from "react-native";
import { AppText, TextTone } from "@/ui/AppText";

export type StatusTone = "success" | "warning" | "danger" | "neutral" | "primary";

interface StatusPillProps {
  label: string;
  tone: StatusTone;
  isSmall?: boolean;
}

const toneClassNames: Record<StatusTone, string> = {
  success: "bg-success-soft",
  warning: "bg-warning-soft",
  danger: "bg-danger-soft",
  neutral: "bg-muted",
  primary: "bg-primary-soft",
};

const textTones: Record<StatusTone, TextTone> = {
  success: "successSoft",
  warning: "warningSoft",
  danger: "dangerSoft",
  neutral: "muted",
  primary: "primarySoft",
};

export function StatusPill({ label, tone, isSmall = false }: StatusPillProps) {
  return (
    <View
      className={`self-start rounded-full ${isSmall ? "px-[7px] py-0.5" : "px-2 py-[3px]"} ${toneClassNames[tone]}`}
    >
      <AppText variant="pill" tone={textTones[tone]} numberOfLines={1}>
        {label}
      </AppText>
    </View>
  );
}
