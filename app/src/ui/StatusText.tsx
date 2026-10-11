import { AppText, TextTone } from "@/ui/AppText";
import { StatusTone } from "@/ui/StatusPill";

interface StatusTextProps {
  label: string;
  tone: StatusTone;
}

const textTones: Record<StatusTone, TextTone> = {
  success: "success",
  warning: "warning",
  danger: "danger",
  neutral: "muted",
  primary: "primary",
};

export function StatusText({ label, tone }: StatusTextProps) {
  return (
    <AppText variant="link" tone={textTones[tone]} numberOfLines={1}>
      {label}
    </AppText>
  );
}
