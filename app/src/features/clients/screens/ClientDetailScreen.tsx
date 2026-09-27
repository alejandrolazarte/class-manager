import { useRouter } from "expo-router";
import { Linking, View } from "react-native";
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
import { Avatar } from "@/ui/Avatar";
import { Card } from "@/ui/Card";
import { Screen, ScrollScreen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";
import { SectionTitle } from "@/ui/SectionTitle";

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
    return (
      <Screen header={<ScreenHeader navigation="back" title="" />}>
        <Spinner className="mt-6" />
      </Screen>
    );
  }
  if (isError || client === undefined) {
    return (
      <ScrollScreen header={<ScreenHeader navigation="back" title="" />}>
        <Banner message={translate("clients.detail.notFound")}>
          <Button
            variant="secondary"
            size="medium"
            label={translate("common.retry")}
            onPress={() => refetch()}
          />
        </Banner>
      </ScrollScreen>
    );
  }

  return (
    <ScrollScreen
      header={
        <ScreenHeader
          navigation="back"
          leading={<Avatar name={client.fullName} tone="primary" size="large" />}
          eyebrow={translate("clients.detail.title")}
          title={client.fullName}
          subtitle={formatPhoneNumberForDisplay(client.phoneNumber)}
        />
      }
    >
      <View className="flex-row gap-2.5">
        <View className="flex-1">
          <Button
            variant="secondary"
            size="medium"
            icon="call"
            label={translate("clients.detail.call")}
            onPress={() => Linking.openURL(`${telephoneScheme}${client.phoneNumber}`)}
          />
        </View>
        <View className="flex-1">
          <Button
            variant="secondary"
            size="medium"
            icon="whatsApp"
            label={translate("clients.detail.whatsApp")}
            onPress={() =>
              Linking.openURL(`${whatsAppBaseUrl}${toDialableDigits(client.phoneNumber)}`)
            }
          />
        </View>
      </View>
      {client.email || client.notes ? (
        <View className="gap-3 rounded-2xl bg-muted px-3.5 py-3">
          {client.email ? (
            <View className="gap-0.5">
              <AppText variant="overline" tone="subtle">
                {translate("clients.detail.email")}
              </AppText>
              <AppText
                variant="bodyStrong"
                tone="primary"
                accessibilityRole="link"
                onPress={() => Linking.openURL(`${emailScheme}${client.email}`)}
              >
                {client.email}
              </AppText>
            </View>
          ) : null}
          {client.notes ? (
            <View className="gap-0.5">
              <AppText variant="overline" tone="subtle">
                {translate("clients.detail.notes")}
              </AppText>
              <AppText variant="body">{client.notes}</AppText>
            </View>
          ) : null}
        </View>
      ) : null}
      <SectionTitle title={translate("clients.detail.students")} />
      {client.students.length === 0 ? (
        <AppText variant="body" tone="muted">
          {translate("clients.detail.noStudents")}
        </AppText>
      ) : (
        client.students.map((student) => {
          const ageLabel = studentAgeLabel(student.birthDate);
          return (
            <Card key={student.id} className="gap-2.5 px-4 py-3.5">
              <View className="gap-0.5">
                <AppText variant="heading" className="text-base">
                  {student.fullName}
                </AppText>
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
              </View>
              <StudentClasses studentId={student.id} />
            </Card>
          );
        })
      )}
      <Button
        variant="dashed"
        size="medium"
        icon="add"
        label={translate("clients.detail.addStudent")}
        onPress={() => router.push(routes.addStudent(client.id))}
      />
      <ClientFeeSection client={client} />
    </ScrollScreen>
  );
}
