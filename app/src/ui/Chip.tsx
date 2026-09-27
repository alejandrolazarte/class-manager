import { Pressable } from "react-native";
import { AppText, TextTone } from "@/ui/AppText";

interface ChipProps {
  label: string;
  isSelected: boolean;
  onPress: () => void;
  disabled?: boolean;
  accessibilityLabel?: string;
}

export function Chip({
  label,
  isSelected,
  onPress,
  disabled = false,
  accessibilityLabel,
}: ChipProps) {
  const stateClassName = isSelected
    ? "border-primary bg-primary"
    : disabled
      ? "border-border bg-muted"
      : "border-border-strong bg-surface";
  const labelTone: TextTone = isSelected ? "onPrimary" : disabled ? "disabled" : "default";
  return (
    <Pressable
      accessibilityRole="button"
      accessibilityLabel={accessibilityLabel ?? label}
      accessibilityState={{ selected: isSelected, disabled }}
      disabled={disabled}
      onPress={onPress}
      className={`rounded-full border px-3 py-2 ${stateClassName}`}
    >
      <AppText variant="caption" tone={labelTone} className={disabled ? "line-through" : ""}>
        {label}
      </AppText>
    </Pressable>
  );
}
