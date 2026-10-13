import { ReactNode } from "react";
import { Pressable, View } from "react-native";
import { AppText } from "@/ui/AppText";
import { Icon } from "@/ui/Icon";
import { withRequiredMark } from "@/ui/RequiredFieldsLegend";

interface CheckboxProps {
  label: string;
  isChecked: boolean;
  onCheckedChange: (isChecked: boolean) => void;
  isRequired?: boolean;
  labelContent?: ReactNode;
}

export function Checkbox({
  label,
  isChecked,
  onCheckedChange,
  isRequired = false,
  labelContent,
}: CheckboxProps) {
  return (
    <Pressable
      accessibilityRole="checkbox"
      accessibilityLabel={label}
      accessibilityState={{ checked: isChecked }}
      aria-checked={isChecked}
      onPress={() => onCheckedChange(!isChecked)}
      className="min-h-12 flex-row items-center gap-3 py-2"
    >
      <View
        className={`h-6 w-6 items-center justify-center rounded-md border-2 ${isChecked ? "border-primary bg-primary" : "border-subtle-foreground bg-surface"}`}
      >
        {isChecked ? <Icon name="checked" size="small" tone="primary-foreground" /> : null}
      </View>
      <AppText variant="body" className="min-w-0 flex-1">
        {labelContent ?? label}
        {isRequired ? withRequiredMark("", isRequired) : null}
      </AppText>
    </Pressable>
  );
}
