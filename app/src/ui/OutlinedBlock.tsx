import { PropsWithChildren } from "react";
import { View } from "react-native";
import { PressableScale } from "@/ui/motion/PressableScale";

interface OutlinedBlockProps extends PropsWithChildren {
  className?: string;
  onPress?: () => void;
}

const blockClassName = "rounded-2xl border border-border px-4 py-3.5";

export function OutlinedBlock({ className, onPress, children }: OutlinedBlockProps) {
  if (onPress === undefined) {
    return <View className={`${blockClassName} ${className ?? ""}`}>{children}</View>;
  }
  return (
    <PressableScale
      accessibilityRole="button"
      onPress={onPress}
      className={`${blockClassName} active:bg-muted ${className ?? ""}`}
    >
      {children}
    </PressableScale>
  );
}
