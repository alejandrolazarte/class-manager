import { PropsWithChildren } from "react";
import { KeyboardAvoidingView, Platform, ScrollView } from "react-native";
import { SafeAreaView } from "react-native-safe-area-context";
import { AppText } from "@/ui/AppText";

interface AuthenticationScreenLayoutProps extends PropsWithChildren {
  title: string;
}

export function AuthenticationScreenLayout({ title, children }: AuthenticationScreenLayoutProps) {
  return (
    <SafeAreaView className="flex-1 bg-background">
      <KeyboardAvoidingView
        className="flex-1"
        behavior={Platform.OS === "ios" ? "padding" : undefined}
      >
        <ScrollView
          contentContainerClassName="w-full max-w-md gap-4 self-center p-6"
          keyboardShouldPersistTaps="handled"
        >
          <AppText variant="display" accessibilityRole="header">
            {title}
          </AppText>
          {children}
        </ScrollView>
      </KeyboardAvoidingView>
    </SafeAreaView>
  );
}
