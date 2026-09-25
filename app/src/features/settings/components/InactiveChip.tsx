import { Text, View } from "react-native";
import { translate } from "@/i18n/translate";

export function InactiveChip() {
  return (
    <View className="rounded-full bg-gray-200 px-2 py-1">
      <Text className="text-xs font-medium text-gray-700">{translate("common.inactive")}</Text>
    </View>
  );
}
