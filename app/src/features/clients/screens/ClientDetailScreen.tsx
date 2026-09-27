import { useRouter } from "expo-router";
import { Linking, ScrollView, View } from "react-native";
import {
  formatPhoneNumberForDisplay,
  toDialableDigits,
} from "@/features/clients/phoneNumberFormatting";
import { useClient } from "@/features/clients/useClient";
import { StudentClasses } from "@/features/enrollments/components/StudentClasses";
import { ClientFeeSection } from "@/features/fees/components/ClientFeeSection";
import { formatBirthDateForDisplay } from "@/features/students/birthDateFormatting";
import { studentAgeLabel } from "@/features/students/studentAgeLabel";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { AppText } from "@/ui/AppText";
import { Spinner } from "@/ui/Spinner";

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
    return <Spinner className="mt-6" />;
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
    <ScrollView className="flex-1 bg-background" contentContainerClassName="gap-4 p-4">
      <AppText variant="display">{client.fullName}</AppText>
      <AppText tone="muted" variant="lead">
        {formatPhoneNumberForDisplay(client.phoneNumber)}
      </AppText>
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
          <AppText variant="label" tone="subtle">
            {translate("clients.detail.email")}
          </AppText>
          <AppText
            variant="body"
            tone="primary"
            accessibilityRole="link"

            onPress={() => Linking.openURL(`${emailScheme}${client.email}`)}
          >
            {client.email}
          </AppText>
        </View>
      ) : null}
      {client.notes ? (
        <View className="gap-1">
          <AppText variant="label" tone="subtle">
            {translate("clients.detail.notes")}
          </AppText>
          <AppText variant="body">{client.notes}</AppText>
        </View>
      ) : null}
      <View className="gap-2">
        <AppText variant="heading">{translate("clients.detail.students")}</AppText>
        {client.students.length === 0 ? (
          <AppText variant="body" tone="muted">
            {translate("clients.detail.noStudents")}
          </AppText>
        ) : (
          client.students.map((student) => {
            const ageLabel = studentAgeLabel(student.birthDate);
            return (
              <View key={student.id} className="gap-1 rounded-xl bg-surface p-3">
                <AppText variant="bodyStrong">{student.fullName}</AppText>
                {student.birthDate && ageLabel ? (
                  <AppText variant="caption" tone="muted">
                    {`${formatBirthDateForDisplay(student.birthDate)} · ${ageLabel}`}
                  </AppText>
                ) : null}
                {student.notes ? (
                  <AppText variant="caption" tone="subtle">
                    {student.notes}
                  </AppText>
                ) : null}
                <StudentClasses studentId={student.id} />
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
      <ClientFeeSection client={client} />
    </ScrollView>
  );
}
