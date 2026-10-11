import { View } from "react-native";
import { ThemeColorToken } from "@/theme/themeColorTokens";
import { Icon, IconName } from "@/ui/Icon";

type IconCircleSize = "medium" | "large";

interface IconCircleProps {
  icon: IconName;
  tone?: ThemeColorToken;
  size?: IconCircleSize;
}

const backgroundClassNames: Partial<Record<ThemeColorToken, string>> = {
  success: "bg-success-soft",
  danger: "bg-danger-soft",
  warning: "bg-warning-soft",
  primary: "bg-primary-soft",
};

const sizeClassNames: Record<IconCircleSize, string> = {
  medium: "h-11 w-11",
  large: "h-14 w-14",
};

export function IconCircle({ icon, tone = "muted-foreground", size = "medium" }: IconCircleProps) {
  return (
    <View
      className={`items-center justify-center rounded-full ${sizeClassNames[size]} ${backgroundClassNames[tone] ?? "bg-muted"}`}
    >
      <Icon name={icon} size={size === "large" ? "large" : "medium"} tone={tone} />
    </View>
  );
}
