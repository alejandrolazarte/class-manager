import { useRouter } from "expo-router";
import { Pressable, ScrollView, Text, View } from "react-native";
import { useSession } from "@/features/authentication/useSession";
import { useCurrentBusiness } from "@/features/business/CurrentBusinessProvider";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { Button } from "@/ui/Button";

interface SettingsRowProps {
  label: string;
  detail?: string;
  onPress: () => void;
}

function SettingsRow({ label, detail, onPress }: SettingsRowProps) {
  return (
    <Pressable
      accessibilityRole="button"
      accessibilityLabel={label}
      onPress={onPress}
      className="flex-row items-center gap-3 border-b border-gray-100 bg-white px-4 py-4"
    >
      <View className="flex-1 gap-1">
        <Text className="text-base font-semibold text-gray-900">{label}</Text>
        {detail ? <Text className="text-sm text-gray-600">{detail}</Text> : null}
      </View>
      <Text className="text-xl text-gray-400">›</Text>
    </Pressable>
  );
}

export function SettingsScreen() {
  const router = useRouter();
  const business = useCurrentBusiness();
  const { signOut } = useSession();
  return (
    <ScrollView className="flex-1 bg-gray-50" contentContainerClassName="gap-6 pb-12 pt-4">
      <View>
        <SettingsRow
          label={translate("settings.business")}
          detail={business.name}
          onPress={() => router.push(routes.businessSettings)}
        />
        <SettingsRow
          label={translate("settings.instructors")}
          onPress={() => router.push(routes.instructors)}
        />
      </View>
      <View className="px-4">
        <Button variant="secondary" label={translate("settings.signOut")} onPress={signOut} />
      </View>
    </ScrollView>
  );
}
