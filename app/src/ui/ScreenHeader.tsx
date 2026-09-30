import { useRouter } from "expo-router";
import { ReactNode } from "react";
import { View } from "react-native";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { IconButton } from "@/ui/IconButton";

type HeaderNavigation = "back" | "close";

interface ScreenHeaderProps {
  title: string;
  eyebrow?: string;
  subtitle?: string;
  navigation?: HeaderNavigation;
  onNavigate?: () => void;
  navigationAction?: ReactNode;
  accessory?: ReactNode;
  leading?: ReactNode;
}

export function ScreenHeader({
  title,
  eyebrow,
  subtitle,
  navigation,
  onNavigate,
  navigationAction,
  accessory,
  leading,
}: ScreenHeaderProps) {
  const router = useRouter();
  return (
    <View>
      {navigation ? (
        <View className="flex-row items-center justify-between px-2 pt-2">
          <IconButton
            icon={navigation}
            accessibilityLabel={translate(navigation === "back" ? "common.back" : "common.close")}
            onPress={onNavigate ?? (() => router.back())}
          />
          {navigationAction}
        </View>
      ) : null}
      <View
        className={`flex-row items-center justify-between gap-3 px-5 ${navigation ? "" : "pt-3"}`}
      >
        {leading}
        <View className="min-w-0 flex-1 gap-0.5">
          {eyebrow ? (
            <AppText variant="eyebrow" tone="accent">
              {eyebrow}
            </AppText>
          ) : null}
          {title ? (
            <AppText variant="display" accessibilityRole="header">
              {title}
            </AppText>
          ) : null}
          {subtitle ? (
            <AppText variant="body" tone="muted" className="mt-0.5">
              {subtitle}
            </AppText>
          ) : null}
        </View>
        {accessory}
      </View>
    </View>
  );
}
