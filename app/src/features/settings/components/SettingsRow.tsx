import { Pressable, View } from "react-native";
import { ThemeColorToken } from "@/theme/themeColorTokens";
import { AppText } from "@/ui/AppText";
import { Icon, IconName } from "@/ui/Icon";

export type SettingsRowTone = "primary" | "success" | "warning" | "neutral" | "danger";

const tileClassNames: Record<SettingsRowTone, string> = {
  primary: "bg-primary-soft",
  success: "bg-success-soft",
  warning: "bg-warning-soft",
  neutral: "bg-muted",
  danger: "bg-danger-soft",
};

const tileIconTones: Record<SettingsRowTone, ThemeColorToken> = {
  primary: "primary-soft-foreground",
  success: "success-soft-foreground",
  warning: "warning-soft-foreground",
  neutral: "muted-foreground",
  danger: "danger-soft-foreground",
};

interface SettingsRowProps {
  icon: IconName;
  tone: SettingsRowTone;
  label: string;
  value?: string;
  onPress: () => void;
  isDestructive?: boolean;
}

export function SettingsRow({
  icon,
  tone,
  label,
  value,
  onPress,
  isDestructive = false,
}: SettingsRowProps) {
  return (
    <Pressable
      accessibilityRole="button"
      accessibilityLabel={label}
      accessibilityHint={value}
      onPress={onPress}
      className="min-h-[52px] flex-row items-center gap-3 px-3.5 py-2.5 active:bg-muted"
    >
      <View
        className={`h-8 w-8 items-center justify-center rounded-[10px] ${tileClassNames[tone]}`}
      >
        <Icon name={icon} tone={tileIconTones[tone]} />
      </View>
      <AppText
        variant="bodyStrong"
        tone={isDestructive ? "danger" : "default"}
        numberOfLines={1}
        className="min-w-0 flex-1"
      >
        {label}
      </AppText>
      {value ? (
        <AppText variant="body" tone="subtle" numberOfLines={1}>
          {value}
        </AppText>
      ) : null}
      {isDestructive ? null : <Icon name="next" tone="subtle-foreground" />}
    </Pressable>
  );
}
