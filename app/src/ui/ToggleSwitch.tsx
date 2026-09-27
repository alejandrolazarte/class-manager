import { Pressable, View } from "react-native";
import { AppText } from "@/ui/AppText";

interface ToggleSwitchProps {
  label: string;
  value: boolean;
  onValueChange: (value: boolean) => void;
}

export function ToggleSwitch({ label, value, onValueChange }: ToggleSwitchProps) {
  return (
    <Pressable
      accessibilityRole="switch"
      accessibilityLabel={label}
      accessibilityState={{ checked: value }}
      onPress={() => onValueChange(!value)}
      className="flex-row items-center gap-3 rounded-2xl border-[1.5px] border-border bg-surface px-4 py-3.5"
    >
      <AppText variant="bodyStrong" className="flex-1 font-label">
        {label}
      </AppText>
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
