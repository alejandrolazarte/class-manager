import { Pressable, Text } from "react-native";

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
    ? "border-brand bg-brand"
    : disabled
      ? "border-gray-200 bg-gray-100"
      : "border-gray-300 bg-white";
  const labelClassName = isSelected
    ? "text-white"
    : disabled
      ? "text-gray-400 line-through"
      : "text-gray-800";
  return (
    <Pressable
      accessibilityRole="button"
      accessibilityLabel={accessibilityLabel ?? label}
      accessibilityState={{ selected: isSelected, disabled }}
      disabled={disabled}
      onPress={onPress}
      className={`rounded-full border px-3 py-2 ${stateClassName}`}
    >
      <Text className={`text-sm ${labelClassName}`}>{label}</Text>
    </Pressable>
  );
}
