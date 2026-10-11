import { useRouter } from "expo-router";
import { useState } from "react";
import { Linking, View } from "react-native";
import {
  formatPhoneNumberForDisplay,
  toDialableDigits,
} from "@/features/clients/phoneNumberFormatting";
import { ClientDetails } from "@/features/clients/types";
import { useClient } from "@/features/clients/useClient";
import { StudentClasses } from "@/features/enrollments/components/StudentClasses";
import { InviteStudentAppSection } from "@/features/studentApp/components/InviteStudentAppSection";
import { ClientFeeSection } from "@/features/fees/components/ClientFeeSection";
import { useCan } from "@/features/members/CurrentMemberProvider";
import { permissions } from "@/features/members/permissions";
import { formatBirthDateForDisplay } from "@/features/students/birthDateFormatting";
import { studentAgeLabel } from "@/features/students/studentAgeLabel";
import { translate } from "@/i18n/translate";
import { ClientTab, clientTabs, routes } from "@/navigation/routes";
import { useCurrentTab } from "@/navigation/useCurrentTab";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { AppText } from "@/ui/AppText";
import { Spinner } from "@/ui/Spinner";
import { Avatar } from "@/ui/Avatar";
import { ActionTile } from "@/ui/ActionTile";
import { TonalBlock } from "@/ui/TonalBlock";
import { Screen, ScrollScreen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";
import { SectionTitle } from "@/ui/SectionTitle";
import { Tabs } from "@/ui/Tabs";

const telephoneScheme = "tel:";
const emailScheme = "mailto:";
const whatsAppBaseUrl = "https://wa.me/";

type ClientDetailSection = "classes" | "billing";

const initialSections: Record<ClientTab, ClientDetailSection> = {
  students: "classes",
  fees: "billing",
};

function normalizedName(fullName: string): string {
  return fullName.trim().toLocaleLowerCase();
}

function isClientTheOnlyStudent(client: ClientDetails): boolean {
  return (
    client.students.length === 1 &&
    normalizedName(client.students[0].fullName) === normalizedName(client.fullName)
  );
}

interface ClientClassesProps {
  client: ClientDetails;
  clientIsTheOnlyStudent: boolean;
  hasTitle: boolean;
  canInviteStudents: boolean;
}

function ClientClasses({
  client,
  clientIsTheOnlyStudent,
  hasTitle,
  canInviteStudents,
}: ClientClassesProps) {
  if (clientIsTheOnlyStudent) {
    return (
      <>
        {hasTitle ? <SectionTitle title={translate("clients.detail.classes")} isOverline /> : null}
        <View className="gap-2 border-b border-border-subtle pb-3">
          {client.students[0].notes ? (
            <AppText variant="footnote" tone="subtle">
              {client.students[0].notes}
            </AppText>
          ) : null}
          <StudentClasses studentId={client.students[0].id} />
        </View>
      </>
    );
  }
  return (
    <>
      <SectionTitle title={translate("clients.detail.students")} isOverline />
      {client.students.length === 0 ? (
        <AppText variant="body" tone="muted">
          {translate("clients.detail.noStudents")}
        </AppText>
      ) : (
        client.students.map((student) => {
          const ageLabel = studentAgeLabel(student.birthDate);
          return (
            <View key={student.id} className="gap-2 border-b border-border-subtle pb-3">
              <View>
                <AppText variant="bodyStrong">{student.fullName}</AppText>
                {student.birthDate && ageLabel ? (
                  <AppText variant="footnote" tone="subtle">
                    {`${formatBirthDateForDisplay(student.birthDate)} · ${ageLabel}`}
                  </AppText>
                ) : null}
                {student.notes ? (
                  <AppText variant="footnote" tone="subtle">
                    {student.notes}
                  </AppText>
                ) : null}
              </View>
              <StudentClasses studentId={student.id} />
              {canInviteStudents &&
              normalizedName(student.fullName) !== normalizedName(client.fullName) ? (
                <InviteStudentAppSection client={client} student={student} />
              ) : null}
            </View>
          );
        })
      )}
    </>
  );
}

interface ClientDetailScreenProps {
  clientId: string;
}

export function ClientDetailScreen({ clientId }: ClientDetailScreenProps) {
  const router = useRouter();
  const clientTab = useCurrentTab(clientTabs);
  const [section, setSection] = useState<ClientDetailSection>(initialSections[clientTab]);
  const { data: client, isPending, isError, refetch } = useClient(clientId);
  const canManageStudents = useCan(permissions.studentsManage);
  const canViewPayments = useCan(permissions.paymentsViewAll, permissions.paymentsViewOwn);
  const canSellAtCounter = useCan(permissions.ordersManage);

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

  const clientIsTheOnlyStudent = isClientTheOnlyStudent(client);
  const email = client.appAccess.signInEmail ?? client.email;
  const visibleSection: ClientDetailSection = canViewPayments ? section : "classes";
  const counterSaleButton = canSellAtCounter ? (
    <Button
      variant="secondary"
      size="medium"
      icon="products"
      label={translate("orders.counterSale.open")}
      onPress={() => router.push(routes.clientCounterSale(clientTab, client.id))}
    />
  ) : null;

  return (
    <ScrollScreen
      header={
        <ScreenHeader
          navigation="back"
          navigationAction={
            canManageStudents ? (
              <Button
                variant="ghost"
                size="medium"
                icon="edit"
                label={translate("clients.detail.edit")}
                accessibilityLabel={translate("clients.detail.editAccessibility", {
                  name: client.fullName,
                })}
                onPress={() => router.push(routes.editClient(clientTab, client.id))}
              />
            ) : undefined
          }
          leading={<Avatar name={client.fullName} tone="primary" size="large" />}
          eyebrow={clientIsTheOnlyStudent ? undefined : translate("clients.detail.title")}
          title={client.fullName}
          subtitle={formatPhoneNumberForDisplay(client.phoneNumber)}
          caption={email ?? undefined}
        />
      }
    >
      <View className="flex-row gap-2">
        <ActionTile
          icon="call"
          label={translate("clients.detail.call")}
          onPress={() => Linking.openURL(`${telephoneScheme}${client.phoneNumber}`)}
        />
        <ActionTile
          icon="whatsApp"
          label={translate("clients.detail.whatsApp")}
          onPress={() =>
            Linking.openURL(`${whatsAppBaseUrl}${toDialableDigits(client.phoneNumber)}`)
          }
        />
        {email ? (
          <ActionTile
            icon="email"
            label={translate("clients.detail.email")}
            onPress={() => Linking.openURL(`${emailScheme}${email}`)}
          />
        ) : null}
      </View>
      {client.notes ? (
        <TonalBlock tone="muted" isCompact>
          <SectionTitle title={translate("clients.detail.notes")} isOverline />
          <AppText variant="body">{client.notes}</AppText>
        </TonalBlock>
      ) : null}
      {canManageStudents ? <InviteStudentAppSection client={client} /> : null}
      {canViewPayments ? (
        <Tabs
          options={[
            { value: "classes", label: translate("clients.detail.classes") },
            { value: "billing", label: translate("fees.client.title") },
          ]}
          selectedValue={visibleSection}
          onChange={setSection}
        />
      ) : null}
      {visibleSection === "classes" ? (
        <>
          <ClientClasses
            client={client}
            clientIsTheOnlyStudent={clientIsTheOnlyStudent}
            hasTitle={!canViewPayments}
            canInviteStudents={canManageStudents}
          />
          {canManageStudents ? (
            <Button
              variant="dashed"
              size="medium"
              icon="add"
              label={translate(
                clientIsTheOnlyStudent
                  ? "clients.detail.addOtherPerson"
                  : "clients.detail.addStudent",
              )}
              onPress={() => router.push(routes.addStudent(clientTab, client.id))}
            />
          ) : null}
          {canViewPayments ? null : counterSaleButton}
        </>
      ) : (
        <>
          <ClientFeeSection client={client} />
          {counterSaleButton}
        </>
      )}
    </ScrollScreen>
  );
}
