import { IconButton } from "@/ui/IconButton";

interface StepArrowProps {
  direction: "previous" | "next";
  label: string;
  onPress: () => void;
}

export function StepArrow({ direction, label, onPress }: StepArrowProps) {
  return (
    <IconButton icon={direction} accessibilityLabel={label} onPress={onPress} variant="outlined" />
  );
}
