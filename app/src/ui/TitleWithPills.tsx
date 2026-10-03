import { ReactNode } from "react";
import { View } from "react-native";
import { AppText, TextVariant } from "@/ui/AppText";

interface TitleWithPillsProps {
  title: string;
  variant?: TextVariant;
  testID?: string;
  children?: ReactNode;
}

export function TitleWithPills({
  title,
  variant = "bodyStrong",
  testID,
  children,
}: TitleWithPillsProps) {
  return (
    <View className="flex-row flex-wrap items-center gap-x-2 gap-y-1">
      <AppText variant={variant} className="shrink" testID={testID}>
        {title}
      </AppText>
      {children}
    </View>
  );
}
