import { PropsWithChildren, ReactNode } from "react";
import { Modal, Pressable, ScrollView } from "react-native";
import { SafeAreaView } from "react-native-safe-area-context";
import { translate } from "@/i18n/translate";
import { ThemeScope } from "@/theme/ThemeScope";

interface BottomSheetProps extends PropsWithChildren {
  onClose: () => void;
  header?: ReactNode;
}

export function BottomSheet({ onClose, header, children }: BottomSheetProps) {
  return (
    <Modal transparent visible animationType="slide" onRequestClose={onClose}>
      <ThemeScope className="flex-1 justify-end">
        <Pressable
          accessibilityRole="button"
          accessibilityLabel={translate("common.close")}
          onPress={onClose}
          className="absolute inset-0 bg-inverse/45"
        />
        <SafeAreaView
          edges={["bottom"]}
          className="max-h-[92%] w-full max-w-2xl self-center overflow-hidden rounded-t-[28px] bg-background"
        >
          {header}
          <ScrollView
            contentContainerClassName="gap-3.5 px-5 pb-7 pt-[18px]"
            keyboardShouldPersistTaps="handled"
          >
            {children}
          </ScrollView>
        </SafeAreaView>
      </ThemeScope>
    </Modal>
  );
}
