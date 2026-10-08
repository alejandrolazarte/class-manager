import { Modal, Pressable, View } from "react-native";
import { translate } from "@/i18n/translate";
import { ThemeScope } from "@/theme/ThemeScope";
import { AppText } from "@/ui/AppText";
import { Button } from "@/ui/Button";

interface ConfirmDialogProps {
  title: string;
  message?: string;
  confirmLabel: string;
  onCancel: () => void;
  onConfirm: () => void;
  isConfirming?: boolean;
}

export function ConfirmDialog({
  title,
  message,
  confirmLabel,
  onCancel,
  onConfirm,
  isConfirming = false,
}: ConfirmDialogProps) {
  return (
    <Modal transparent visible animationType="fade" onRequestClose={onCancel}>
      <ThemeScope className="flex-1 items-center justify-center px-6">
        <Pressable
          accessibilityRole="button"
          accessibilityLabel={translate("common.back")}
          onPress={onCancel}
          className="absolute inset-0 bg-inverse/45"
        />
        <View
          accessibilityRole="alert"
          className="w-full max-w-sm gap-2 rounded-[28px] bg-background px-6 pb-5 pt-6"
        >
          <AppText variant="headline" accessibilityRole="header">
            {title}
          </AppText>
          {message ? (
            <AppText variant="body" tone="muted">
              {message}
            </AppText>
          ) : null}
          <View className="mt-3 flex-row justify-end gap-2">
            <Button
              variant="secondary"
              size="medium"
              label={translate("common.back")}
              onPress={onCancel}
            />
            <Button
              variant="danger"
              size="medium"
              label={confirmLabel}
              onPress={onConfirm}
              isLoading={isConfirming}
            />
          </View>
        </View>
      </ThemeScope>
    </Modal>
  );
}
