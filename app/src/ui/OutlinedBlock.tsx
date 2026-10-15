import { PropsWithChildren } from "react";
import { View } from "react-native";

interface OutlinedBlockProps extends PropsWithChildren {
  className?: string;
}

export function OutlinedBlock({ className, children }: OutlinedBlockProps) {
  return (
    <View className={`rounded-2xl border border-border px-4 py-3.5 ${className ?? ""}`}>
      {children}
    </View>
  );
}
