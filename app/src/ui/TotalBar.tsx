import { PropsWithChildren } from "react";
import { View } from "react-native";
import { AppText } from "@/ui/AppText";
import { translate } from "@/i18n/translate";

interface TotalBarProps extends PropsWithChildren {
  total: string;
}

export function TotalBar({ total, children }: TotalBarProps) {
  return (
    <View className="gap-2.5 rounded-t-3xl border-t border-border bg-surface px-5 pb-4 pt-3.5">
      <View className="flex-row items-baseline justify-between">
        <AppText variant="bodyStrong" tone="muted">
          {translate("common.total")}
        </AppText>
        <AppText variant="display" accessibilityLabel={`${translate("common.total")} ${total}`}>
          {total}
        </AppText>
      </View>
      {children}
    </View>
  );
}
