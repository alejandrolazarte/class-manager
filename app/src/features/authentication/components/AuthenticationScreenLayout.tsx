import { PropsWithChildren } from "react";
import { KeyboardAvoidingView, Platform, ScrollView, Text } from "react-native";
import { SafeAreaView } from "react-native-safe-area-context";

interface AuthenticationScreenLayoutProps extends PropsWithChildren {
  title: string;
}

export function AuthenticationScreenLayout({ title, children }: AuthenticationScreenLayoutProps) {
  return (
    <SafeAreaView className="flex-1 bg-gray-50">
      <KeyboardAvoidingView
        className="flex-1"
        behavior={Platform.OS === "ios" ? "padding" : undefined}
      >
        <ScrollView
          contentContainerClassName="w-full max-w-md gap-4 self-center p-6"
          keyboardShouldPersistTaps="handled"
        >
          <Text accessibilityRole="header" className="text-2xl font-bold text-gray-900">
            {title}
          </Text>
          {children}
        </ScrollView>
      </KeyboardAvoidingView>
    </SafeAreaView>
  );
}
