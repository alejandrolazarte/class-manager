import { PropsWithChildren } from "react";
import { View } from "react-native";
import { ThemeColorToken } from "@/theme/themeColorTokens";
import { AppText } from "@/ui/AppText";
import { Icon, IconName } from "@/ui/Icon";

interface EmptyStateProps extends PropsWithChildren {
  message: string;
  icon?: IconName;
  iconTone?: ThemeColorToken;
}

export function EmptyState({ message, icon, iconTone = "primary", children }: EmptyStateProps) {
  return (
    <View className="items-center gap-3 px-5 py-9">
      {icon ? <Icon name={icon} size="huge" tone={iconTone} /> : null}
      <AppText variant="bodyStrong" tone="muted" className="text-center font-label">
        {message}
      </AppText>
      {children}
    </View>
  );
}
