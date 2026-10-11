import { PropsWithChildren } from "react";
import { View } from "react-native";

export function InsetPanel({ children }: PropsWithChildren) {
  return <View className="gap-2.5 rounded-xl bg-muted p-3">{children}</View>;
}
