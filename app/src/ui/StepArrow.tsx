import { IconButton } from "@/ui/IconButton";

interface StepArrowProps {
  direction: "previous" | "next";
  label: string;
  onPress: () => void;
  disabled?: boolean;
}

export function StepArrow({ direction, label, onPress, disabled = false }: StepArrowProps) {
  return (
    <IconButton
      icon={direction}
      accessibilityLabel={label}
      onPress={onPress}
      variant="compact"
      tone="muted-foreground"
      disabled={disabled}
    />
  );
}
