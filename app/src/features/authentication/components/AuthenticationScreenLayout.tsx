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
  footer?: ReactNode;
}

export function AuthenticationScreenLayout({
  title,
  eyebrow,
  onBack,
  progress,
  mark,
  footer,
  children,
}: AuthenticationScreenLayoutProps) {
  const isMarkInBackRow = onBack !== undefined && progress === undefined;
  return (
    <SafeAreaView className="flex-1 bg-background">
      <KeyboardAvoidingView
        className="flex-1"
        behavior={Platform.OS === "ios" ? "padding" : undefined}
      >
        <ScrollView
          contentContainerClassName="w-full max-w-md flex-grow self-center pb-7"
          keyboardShouldPersistTaps="handled"
        >
          {onBack ? (
            <View className="flex-row items-center gap-1 px-3 pt-2">
              <IconButton
                icon="back"
                accessibilityLabel={translate("common.back")}
                onPress={onBack}
              />
              <View className="flex-1 pr-3">{progress}</View>
              {isMarkInBackRow ? <View className="mr-3">{mark}</View> : null}
            </View>
          ) : null}
          <View className="gap-4 px-6 pt-4">
            {isMarkInBackRow ? null : mark}
            <View className="gap-0.5">
              {eyebrow ? (
                <AppText variant="label" tone="primary">
                  {eyebrow}
                </AppText>
              ) : null}
              <AppText variant="display" accessibilityRole="header">
                {title}
              </AppText>
            </View>
            {children}
          </View>
          {footer ? <View className="mt-auto items-center px-6 pt-6">{footer}</View> : null}
        </ScrollView>
      </KeyboardAvoidingView>
    </SafeAreaView>
  );
}
