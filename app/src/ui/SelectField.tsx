import { Pressable, View } from "react-native";
import { AppText } from "@/ui/AppText";
import { Icon, IconName } from "@/ui/Icon";
import { withRequiredMark } from "@/ui/RequiredFieldsLegend";

interface SelectFieldProps {
  label: string;
  icon: IconName;
  value: string | null;
  placeholder: string;
  onPress: () => void;
  isRequired?: boolean;
  accessibilityLabel?: string;
  accessibilityHint?: string;
}

export function SelectField({
  label,
  icon,
  value,
  placeholder,
  onPress,
  isRequired = false,
  accessibilityLabel = label,
  accessibilityHint,
}: SelectFieldProps) {
  return (
    <View className="gap-1.5">
      <AppText variant="label" tone="muted">
        {withRequiredMark(label, isRequired)}
      </AppText>
      <Pressable
        accessibilityRole="button"
        accessibilityLabel={accessibilityLabel}
        accessibilityHint={accessibilityHint}
        onPress={onPress}
        className="h-[46px] flex-row items-center gap-2 rounded-[10px] border border-border bg-surface pl-3.5 pr-3 active:bg-muted"
      >
        <AppText
          variant="input"
          tone={value === null ? "subtle" : "default"}
          numberOfLines={1}
          className="min-w-0 flex-1"
        >
          {value ?? placeholder}
        </AppText>
        <Icon name={icon} tone="muted-foreground" />
      </Pressable>
    </View>
  );
}
