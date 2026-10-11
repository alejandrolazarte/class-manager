import { View } from "react-native";
import { translate } from "@/i18n/translate";
import { ThemeColorToken } from "@/theme/themeColorTokens";
import { AppText } from "@/ui/AppText";
import { IconName } from "@/ui/Icon";
import { IconCircle } from "@/ui/IconCircle";

interface EmptyStateProps {
  message: string;
  icon: IconName;
  iconTone?: ThemeColorToken;
  createActionLabel?: string;
}

export function EmptyState({
  message,
  icon,
  iconTone = "muted-foreground",
  createActionLabel,
}: EmptyStateProps) {
  return (
    <View className="items-center gap-2 px-5 py-10">
      <IconCircle icon={icon} tone={iconTone} />
      <AppText variant="bodyStrong" className="text-center">
        {message}
      </AppText>
      {createActionLabel ? (
        <AppText variant="caption" tone="subtle" className="text-center">
          {translate("common.emptyStateHint", { action: createActionLabel })}
        </AppText>
      ) : null}
    </View>
  );
}
