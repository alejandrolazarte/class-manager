import { Pressable, Text } from "react-native";

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
      className="absolute bottom-6 right-6 h-14 w-14 items-center justify-center rounded-full bg-brand shadow-lg"
    >
      <Text className="text-3xl font-light text-white">+</Text>
    </Pressable>
  );
}
