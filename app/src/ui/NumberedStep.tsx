import { View } from "react-native";
import { AppText } from "@/ui/AppText";

interface NumberedStepProps {
  number: number;
  title: string;
  detail?: string;
  hasDivider?: boolean;
}

export function NumberedStep({ number, title, detail, hasDivider = false }: NumberedStepProps) {
  return (
    <View
      className={`flex-row gap-3.5 py-3 ${detail ? "" : "items-center"} ${hasDivider ? "border-b border-border-subtle" : ""}`}
    >
      <View className="h-7 w-7 items-center justify-center rounded-full bg-primary-soft">
        <AppText variant="badge" tone="primarySoft">
          {number}
        </AppText>
      </View>
      <View className="min-w-0 flex-1 gap-0.5">
        <AppText variant="bodyStrong">{title}</AppText>
        {detail ? (
          <AppText variant="caption" tone="muted">
            {detail}
          </AppText>
        ) : null}
      </View>
    </View>
  );
}
