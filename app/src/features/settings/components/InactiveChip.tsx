import { View } from "react-native";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";

export function InactiveChip() {
  return (
    <View className="rounded-full bg-muted px-2 py-1">
      <AppText variant="badge" tone="muted">
        {translate("common.inactive")}
      </AppText>
    </View>
  );
}
