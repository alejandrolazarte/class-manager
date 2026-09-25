import { ActivityIndicator, Linking, ScrollView, Text, View } from "react-native";
import {
  formatPhoneNumberForDisplay,
  toDialableDigits,
} from "@/features/clients/phoneNumberFormatting";
import { useClient } from "@/features/clients/useClient";
import { translate } from "@/i18n/translate";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";

const telephoneScheme = "tel:";
const emailScheme = "mailto:";
const whatsAppBaseUrl = "https://wa.me/";

interface ClientDetailScreenProps {
  clientId: string;
}

export function ClientDetailScreen({ clientId }: ClientDetailScreenProps) {
  const { data: client, isPending, isError, refetch } = useClient(clientId);

  if (isPending) {
    return <ActivityIndicator className="mt-6" />;
  }
  if (isError || client === undefined) {
    return (
      <View className="p-4">
        <Banner message={translate("clients.detail.notFound")}>
          <Button variant="secondary" label={translate("common.retry")} onPress={() => refetch()} />
        </Banner>
      </View>
    );
  }

  return (
    <ScrollView className="flex-1 bg-gray-50" contentContainerClassName="gap-4 p-4">
      <Text className="text-2xl font-bold text-gray-900">{client.fullName}</Text>
      <Text className="text-lg text-gray-700">
        {formatPhoneNumberForDisplay(client.phoneNumber)}
      </Text>
      <View className="flex-row gap-3">
        <View className="flex-1">
          <Button
            variant="secondary"
            label={translate("clients.detail.call")}
            onPress={() => Linking.openURL(`${telephoneScheme}${client.phoneNumber}`)}
          />
        </View>
        <View className="flex-1">
          <Button
            variant="secondary"
            label={translate("clients.detail.whatsApp")}
            onPress={() =>
              Linking.openURL(`${whatsAppBaseUrl}${toDialableDigits(client.phoneNumber)}`)
            }
          />
        </View>
      </View>
      {client.email ? (
        <View className="gap-1">
          <Text className="text-sm font-medium text-gray-500">
            {translate("clients.detail.email")}
          </Text>
          <Text
            accessibilityRole="link"
            className="text-base text-brand"
            onPress={() => Linking.openURL(`${emailScheme}${client.email}`)}
          >
            {client.email}
          </Text>
        </View>
      ) : null}
      {client.notes ? (
        <View className="gap-1">
          <Text className="text-sm font-medium text-gray-500">
            {translate("clients.detail.notes")}
          </Text>
          <Text className="text-base text-gray-900">{client.notes}</Text>
        </View>
      ) : null}
    </ScrollView>
  );
}
