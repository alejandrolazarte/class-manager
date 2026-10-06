import { View } from "react-native";
import { translate } from "@/i18n/translate";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";

interface DeleteConfirmationProps {
  question: string;
  onCancel: () => void;
  onConfirm: () => void;
  isDeleting?: boolean;
}

export function DeleteConfirmation({
  question,
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
            label={translate("common.confirmDelete")}
            onPress={onConfirm}
            isLoading={isDeleting}
          />
        </View>
      </View>
    </Banner>
  );
}
