import { useRouter } from "expo-router";
import { ActivityIndicator, Linking, ScrollView, Text, View } from "react-native";
import {
  formatPhoneNumberForDisplay,
  toDialableDigits,
} from "@/features/clients/phoneNumberFormatting";
import { useClient } from "@/features/clients/useClient";
import { formatBirthDateForDisplay } from "@/features/students/birthDateFormatting";
import { studentAgeLabel } from "@/features/students/studentAgeLabel";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";

const telephoneScheme = "tel:";
const emailScheme = "mailto:";
const whatsAppBaseUrl = "https://wa.me/";

interface ClientDetailScreenProps {
  clientId: string;
}

export function ClientDetailScreen({ clientId }: ClientDetailScreenProps) {
  const router = useRouter();
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
      <View className="gap-2">
        <Text className="text-lg font-semibold text-gray-900">
          {translate("clients.detail.students")}
        </Text>
        {client.students.length === 0 ? (
          <Text className="text-base text-gray-600">{translate("clients.detail.noStudents")}</Text>
        ) : (
          client.students.map((student) => {
            const ageLabel = studentAgeLabel(student.birthDate);
            return (
              <View key={student.id} className="gap-1 rounded-xl bg-white p-3">
                <Text className="text-base font-semibold text-gray-900">{student.fullName}</Text>
                {student.birthDate && ageLabel ? (
                  <Text className="text-sm text-gray-600">
                    {`${formatBirthDateForDisplay(student.birthDate)} · ${ageLabel}`}
                  </Text>
                ) : null}
                {student.notes ? (
                  <Text className="text-sm text-gray-500">{student.notes}</Text>
                ) : null}
              </View>
            );
          })
        )}
        <Button
          variant="secondary"
          label={translate("clients.detail.addStudent")}
          onPress={() => router.push(routes.addStudent(client.id))}
        />
      </View>
    </ScrollView>
  );
}
