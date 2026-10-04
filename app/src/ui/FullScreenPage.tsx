import { PropsWithChildren, ReactNode } from "react";
import { Modal, ScrollView, View } from "react-native";
import { SafeAreaView } from "react-native-safe-area-context";
import { translate } from "@/i18n/translate";
import { ThemeScope } from "@/theme/ThemeScope";
import { AppText } from "@/ui/AppText";
import { IconButton } from "@/ui/IconButton";

interface FullScreenPageProps extends PropsWithChildren {
  title: string;
  onClose: () => void;
  accessory?: ReactNode;
  footer?: ReactNode;
}

export function FullScreenPage({
  title,
  onClose,
  accessory,
  footer,
  children,
}: FullScreenPageProps) {
  return (
    <Modal visible animationType="slide" onRequestClose={onClose}>
      <ThemeScope className="flex-1 bg-background">
        <SafeAreaView edges={["top", "bottom"]} className="w-full max-w-2xl flex-1 self-center">
          <View className="flex-row items-center gap-1 p-2">
            <IconButton
              icon="back"
              accessibilityLabel={translate("common.back")}
              onPress={onClose}
            />
            <AppText
              variant="headline"
              accessibilityRole="header"
              numberOfLines={1}
              className="min-w-0 flex-1"
            >
              {title}
            </AppText>
            {accessory}
          </View>
          <ScrollView className="flex-1" keyboardShouldPersistTaps="handled">
            {children}
          </ScrollView>
          {footer}
        </SafeAreaView>
      </ThemeScope>
    </Modal>
  );
}
