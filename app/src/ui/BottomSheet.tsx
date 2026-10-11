import { PropsWithChildren, ReactNode } from "react";
import { Modal, Pressable, ScrollView } from "react-native";
import { SafeAreaView } from "react-native-safe-area-context";
import { translate } from "@/i18n/translate";
import { ThemeScope } from "@/theme/ThemeScope";
import { Entrance } from "@/ui/motion/Entrance";

interface BottomSheetProps extends PropsWithChildren {
  onClose: () => void;
  header?: ReactNode;
}

export function BottomSheet({ onClose, header, children }: BottomSheetProps) {
  return (
    <Modal transparent visible animationType="none" onRequestClose={onClose}>
      <ThemeScope className="flex-1 justify-end">
        <Entrance kind="fade" className="absolute inset-0">
          <Pressable
            accessibilityRole="button"
            accessibilityLabel={translate("common.close")}
            onPress={onClose}
            className="absolute inset-0 bg-inverse/45"
          />
        </Entrance>
        <Entrance kind="slideUp" className="max-h-[92%] w-full max-w-2xl self-center">
          <SafeAreaView
            edges={["bottom"]}
            className="w-full shrink overflow-hidden rounded-t-[28px] bg-background"
          >
            {header}
            <ScrollView
              contentContainerClassName="gap-3.5 px-5 pb-7 pt-[18px]"
              keyboardShouldPersistTaps="handled"
            >
              {children}
            </ScrollView>
          </SafeAreaView>
        </Entrance>
      </ThemeScope>
    </Modal>
  );
}
