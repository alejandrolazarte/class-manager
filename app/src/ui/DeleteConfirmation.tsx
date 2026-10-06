import { View } from "react-native";
import { translate } from "@/i18n/translate";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";

interface DeleteConfirmationProps {
  question: string;
  confirmLabel?: string;
  onCancel: () => void;
  onConfirm: () => void;
  isDeleting?: boolean;
}

export function DeleteConfirmation({
  question,
  confirmLabel = translate("common.confirmDelete"),
  onCancel,
  onConfirm,
  isDeleting = false,
}: DeleteConfirmationProps) {
  return (
    <Banner tone="warning" icon="delete" message={question}>
      <View className="flex-row gap-2">
        <View className="flex-1">
          <Button
            variant="secondary"
            size="medium"
            label={translate("common.cancel")}
            onPress={onCancel}
          />
        </View>
        <View className="flex-1">
          <Button
            variant="danger"
            size="medium"
            label={confirmLabel}
            onPress={onConfirm}
            isLoading={isDeleting}
          />
        </View>
      </View>
    </Banner>
  );
}
