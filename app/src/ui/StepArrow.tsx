import { Pressable } from "react-native";
import { Icon } from "@/ui/Icon";

interface StepArrowProps {
  direction: "previous" | "next";
  label: string;
  onPress: () => void;
}

export function StepArrow({ direction, label, onPress }: StepArrowProps) {
  return (
    <Pressable
      accessibilityRole="button"
      accessibilityLabel={label}
      onPress={onPress}
      className="px-4 py-2"
    >
      <Icon name={direction} tone="primary" />
    </Pressable>
  );
}
