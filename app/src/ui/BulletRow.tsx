import { View } from "react-native";
import { AppText } from "@/ui/AppText";

interface BulletRowProps {
  label: string;
  detail?: string;
}

export function BulletRow({ label, detail }: BulletRowProps) {
  return (
    <View className="flex-row items-center gap-2">
      <View className="h-1.5 w-1.5 rounded-full bg-primary" />
      <AppText variant="body" className="min-w-0 flex-1">
        {label}
      </AppText>
      {detail ? (
        <AppText variant="footnote" tone="subtle">
          {detail}
        </AppText>
      ) : null}
    </View>
  );
}
