import { PropsWithChildren, ReactNode } from "react";
import { KeyboardAvoidingView, Platform, ScrollView, View } from "react-native";
import { SafeAreaView } from "react-native-safe-area-context";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { IconButton } from "@/ui/IconButton";
import { StepProgress } from "@/ui/StepProgress";

interface StepScreenProps extends PropsWithChildren {
  step: number;
  stepCount: number;
  title: string;
  segmentFills: readonly number[];
  onBack: () => void;
  footer?: ReactNode;
}

export function StepScreen({
  step,
  stepCount,
  title,
  segmentFills,
  onBack,
  footer,
  children,
}: StepScreenProps) {
  return (
    <SafeAreaView edges={["top"]} className="flex-1 bg-background">
      <KeyboardAvoidingView
        className="w-full max-w-2xl flex-1 self-center"
        behavior={Platform.OS === "ios" ? "padding" : undefined}
      >
        <View className="flex-row items-center gap-2 px-2 pt-2">
          <IconButton icon="back" accessibilityLabel={translate("common.back")} onPress={onBack} />
          <View className="flex-1 pr-5">
            <StepProgress segmentFills={segmentFills} />
          </View>
        </View>
        <View className="gap-1 px-5 pb-3 pt-2">
          <AppText variant="eyebrow" tone="primary">
            {translate("common.step", { step, total: stepCount })}
          </AppText>
          <AppText variant="display" accessibilityRole="header">
            {title}
          </AppText>
        </View>
        <ScrollView
          className="flex-1"
          contentContainerClassName="gap-4 px-5 pb-6"
          keyboardShouldPersistTaps="handled"
        >
          {children}
        </ScrollView>
        {footer}
      </KeyboardAvoidingView>
    </SafeAreaView>
  );
}
