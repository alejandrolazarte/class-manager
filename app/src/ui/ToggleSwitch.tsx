import { Pressable, View } from "react-native";
import { AppText } from "@/ui/AppText";
import { Icon, IconName } from "@/ui/Icon";

interface ToggleSwitchProps {
  label: string;
  value: boolean;
  onValueChange: (value: boolean) => void;
  hint?: string;
  icon?: IconName;
}

export function ToggleSwitch({ label, value, onValueChange, hint, icon }: ToggleSwitchProps) {
  return (
    <Pressable
      accessibilityRole="switch"
      accessibilityLabel={label}
      accessibilityHint={hint}
      accessibilityState={{ checked: value }}
      onPress={() => onValueChange(!value)}
      className="min-h-12 flex-row items-center gap-3 py-2"
    >
      {icon ? <Icon name={icon} size="medium" tone="muted-foreground" /> : null}
      <View className="min-w-0 flex-1">
        <AppText variant="body">{label}</AppText>
        {hint ? (
          <AppText variant="caption" tone="subtle">
            {hint}
          </AppText>
        ) : null}
      </View>
      <View
        className={`h-8 w-[52px] flex-row items-center rounded-2xl px-1 ${value ? "justify-end bg-primary" : "justify-start bg-muted"}`}
      >
        <View
          className={`h-6 w-6 rounded-full ${value ? "bg-primary-foreground" : "bg-subtle-foreground"}`}
        />
      </View>
    </Pressable>
  );
}
