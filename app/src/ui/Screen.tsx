import { PropsWithChildren, ReactNode } from "react";
import { KeyboardAvoidingView, Platform, ScrollView, View } from "react-native";
import { SafeAreaView } from "react-native-safe-area-context";

interface ScreenProps extends PropsWithChildren {
  header?: ReactNode;
  overlay?: ReactNode;
}

export function Screen({ header, overlay, children }: ScreenProps) {
  return (
    <SafeAreaView edges={["top"]} className="flex-1 bg-background">
      <KeyboardAvoidingView
        className="flex-1"
        behavior={Platform.OS === "ios" ? "padding" : undefined}
      >
        {header}
        <View className="flex-1">{children}</View>
      </KeyboardAvoidingView>
      {overlay}
    </SafeAreaView>
  );
}

interface ScrollScreenProps extends ScreenProps {
  hasFloatingAction?: boolean;
  footer?: ReactNode;
}

export function ScrollScreen({
  header,
  overlay,
  hasFloatingAction = false,
  footer,
  children,
}: ScrollScreenProps) {
  return (
    <Screen overlay={overlay}>
      <ScrollView
        className="flex-1"
        contentContainerClassName={`w-full max-w-2xl gap-4 self-center ${hasFloatingAction ? "pb-28" : "pb-8"}`}
        keyboardShouldPersistTaps="handled"
      >
        {header}
        <View className="gap-4 px-5">{children}</View>
      </ScrollView>
      {footer ? <View className="w-full max-w-2xl self-center">{footer}</View> : null}
    </Screen>
  );
}

export function ScreenFooter({ children }: PropsWithChildren) {
  return <View className="border-t border-border bg-background px-5 pb-4 pt-3">{children}</View>;
}
