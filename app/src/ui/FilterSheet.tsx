import { PropsWithChildren } from "react";
import { View } from "react-native";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { BottomSheet } from "@/ui/BottomSheet";
import { Button } from "@/ui/Button";
import { IconButton } from "@/ui/IconButton";

interface FilterSheetProps extends PropsWithChildren {
  title: string;
  hint?: string;
  resultsLabel: string;
  onClear: () => void;
  onClose: () => void;
}

export function FilterSheet({
  title,
  hint,
  resultsLabel,
  onClear,
  onClose,
  children,
}: FilterSheetProps) {
  return (
    <BottomSheet onClose={onClose}>
      <View className="flex-row items-center justify-between">
        <AppText variant="headline">{title}</AppText>
        <IconButton icon="close" accessibilityLabel={translate("common.close")} onPress={onClose} />
      </View>
      {hint ? (
        <AppText variant="caption" tone="muted">
          {hint}
        </AppText>
      ) : null}
      <View>{children}</View>
      <View className="flex-row gap-2.5 pt-1">
        <View className="flex-1">
          <Button variant="secondary" label={translate("filters.clearOne")} onPress={onClear} />
        </View>
        <View className="flex-[1.6]">
          <Button label={resultsLabel} onPress={onClose} />
        </View>
      </View>
    </BottomSheet>
  );
}
