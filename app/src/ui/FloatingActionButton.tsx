import { Pressable } from "react-native";
import { Icon } from "@/ui/Icon";

interface FloatingActionButtonProps {
  accessibilityLabel: string;
  onPress: () => void;
}

export function FloatingActionButton({ accessibilityLabel, onPress }: FloatingActionButtonProps) {
  return (
    <Pressable
      accessibilityRole="button"
      accessibilityLabel={accessibilityLabel}
      onPress={onPress}
      className="absolute bottom-6 right-6 h-14 w-14 items-center justify-center rounded-full bg-primary shadow-lg"
    >
      <Icon name="add" size="extraLarge" tone="primary-foreground" />
    </Pressable>
  );
}
