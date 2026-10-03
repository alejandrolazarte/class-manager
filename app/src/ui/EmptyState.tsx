import { View } from "react-native";
import { translate } from "@/i18n/translate";
import { ThemeColorToken } from "@/theme/themeColorTokens";
import { AppText } from "@/ui/AppText";
import { Icon, IconName } from "@/ui/Icon";

interface EmptyStateProps {
  message: string;
  icon: IconName;
  iconTone?: ThemeColorToken;
  createActionLabel?: string;
}

export function EmptyState({
  message,
  icon,
  iconTone = "primary",
  createActionLabel,
}: EmptyStateProps) {
  return (
    <View className="items-center gap-3 px-5 py-9">
      <Icon name={icon} size="huge" tone={iconTone} />
      <AppText variant="bodyStrong" tone="muted" className="text-center font-label">
        {message}
      </AppText>
      {createActionLabel ? (
        <AppText variant="body" tone="subtle" className="text-center">
          {translate("common.emptyStateHint", { action: createActionLabel })}
        </AppText>
      ) : null}
    </View>
  );
}
