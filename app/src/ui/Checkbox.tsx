import { PropsWithChildren } from "react";
import { Pressable, View } from "react-native";
import { AppText } from "@/ui/AppText";
import { Icon } from "@/ui/Icon";
import { withRequiredMark } from "@/ui/RequiredFieldsLegend";

interface CheckboxProps {
  label: string;
  isChecked: boolean;
  onCheckedChange: (isChecked: boolean) => void;
  isRequired?: boolean;
}

export function Checkbox({
  label,
  isChecked,
  onCheckedChange,
  isRequired = false,
  children,
}: PropsWithChildren<CheckboxProps>) {
  return (
    <View className="gap-1">
      <Pressable
        accessibilityRole="checkbox"
        accessibilityLabel={label}
        accessibilityState={{ checked: isChecked }}
        onPress={() => onCheckedChange(!isChecked)}
        className="min-h-12 flex-row items-center gap-3 py-2"
      >
        <View
          className={`h-6 w-6 items-center justify-center rounded-md border-2 ${isChecked ? "border-primary bg-primary" : "border-subtle-foreground bg-surface"}`}
        >
          {isChecked ? <Icon name="checked" size="small" tone="primary-foreground" /> : null}
        </View>
        <AppText variant="body" className="min-w-0 flex-1">
          {withRequiredMark(label, isRequired)}
        </AppText>
      </Pressable>
      {children ? <View className="pl-9">{children}</View> : null}
    </View>
  );
}
