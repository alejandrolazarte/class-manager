import { PropsWithChildren, ReactNode } from "react";
import { KeyboardAvoidingView, Platform, ScrollView, View } from "react-native";
import { SafeAreaView } from "react-native-safe-area-context";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { IconButton } from "@/ui/IconButton";

interface AuthenticationScreenLayoutProps extends PropsWithChildren {
  title: string;
  eyebrow?: string;
  onBack?: () => void;
  progress?: ReactNode;
  mark?: ReactNode;
}

export function AuthenticationScreenLayout({
  title,
  eyebrow,
  onBack,
  progress,
  mark,
  children,
}: AuthenticationScreenLayoutProps) {
  return (
    <SafeAreaView className="flex-1 bg-background">
      <KeyboardAvoidingView
        className="flex-1"
        behavior={Platform.OS === "ios" ? "padding" : undefined}
      >
        <ScrollView
          contentContainerClassName="w-full max-w-md self-center pb-7"
          keyboardShouldPersistTaps="handled"
        >
          {onBack ? (
            <View className="flex-row items-center gap-2 px-2 pt-2">
              <IconButton
                icon="back"
                accessibilityLabel={translate("common.back")}
                onPress={onBack}
              />
              {progress ? <View className="flex-1 pr-5">{progress}</View> : null}
            </View>
          ) : null}
          <View className="gap-[18px] px-6 pt-2">
            {mark}
            <View className="gap-1">
              {eyebrow ? (
                <AppText variant="eyebrow" tone="primary">
                  {eyebrow}
                </AppText>
              ) : null}
              <AppText
                variant="display"
                className="text-[28px] leading-8"
                accessibilityRole="header"
              >
                {title}
              </AppText>
            </View>
            {children}
          </View>
        </ScrollView>
      </KeyboardAvoidingView>
    </SafeAreaView>
  );
}
