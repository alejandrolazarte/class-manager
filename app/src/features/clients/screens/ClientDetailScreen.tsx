import { useRouter } from "expo-router";
import { useState } from "react";
import { Linking, Pressable, View } from "react-native";
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
import { Card } from "@/ui/Card";
import { Icon, IconName } from "@/ui/Icon";
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

interface ContactActionProps {
  icon: IconName;
  label: string;
  onPress: () => void;
}

function ContactAction({ icon, label, onPress }: ContactActionProps) {
  return (
    <Pressable
      accessibilityRole="button"
      accessibilityLabel={label}
      onPress={onPress}
      className="min-h-[60px] flex-1 items-center justify-center gap-1 rounded-2xl border-[1.5px] border-border bg-surface px-2 py-2.5 active:bg-muted"
    >
      <Icon name={icon} size="medium" tone="primary" />
      <AppText variant="link" numberOfLines={1}>
        {label}
      </AppText>
    </Pressable>
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
        {hasTitle ? <SectionTitle title={translate("clients.detail.classes")} /> : null}
        <Card className="gap-2.5 px-4 py-3.5">
          {client.students[0].notes ? (
            <AppText variant="caption" tone="subtle">
              {client.students[0].notes}
            </AppText>
          ) : null}
          <StudentClasses studentId={client.students[0].id} />
        </Card>
      </>
    );
  }
  return (
    <>
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
              </View>
              <StudentClasses studentId={student.id} />
              {canInviteStudents &&
              normalizedName(student.fullName) !== normalizedName(client.fullName) ? (
                <InviteStudentAppSection client={client} student={student} />
              ) : null}
            </Card>
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
              <Pressable
                accessibilityRole="button"
                accessibilityLabel={translate("clients.detail.editAccessibility", {
                  name: client.fullName,
                })}
                onPress={() => router.push(routes.editClient(clientTab, client.id))}
                className="flex-row items-center gap-1 rounded-full px-3 py-2.5 active:bg-muted"
              >
                <Icon name="edit" size="medium" tone="primary" />
                <AppText variant="bodyStrong" tone="primary">
                  {translate("clients.detail.edit")}
                </AppText>
              </Pressable>
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
      <View className="flex-row gap-2.5">
        <ContactAction
          icon="call"
          label={translate("clients.detail.call")}
          onPress={() => Linking.openURL(`${telephoneScheme}${client.phoneNumber}`)}
        />
        <ContactAction
          icon="whatsApp"
          label={translate("clients.detail.whatsApp")}
          onPress={() =>
            Linking.openURL(`${whatsAppBaseUrl}${toDialableDigits(client.phoneNumber)}`)
          }
        />
        {email ? (
          <ContactAction
            icon="email"
            label={translate("clients.detail.email")}
            onPress={() => Linking.openURL(`${emailScheme}${email}`)}
          />
        ) : null}
      </View>
      {client.notes ? (
        <View className="gap-0.5 rounded-2xl bg-muted px-3.5 py-3">
          <AppText variant="overline" tone="subtle">
            {translate("clients.detail.notes")}
          </AppText>
          <AppText variant="body">{client.notes}</AppText>
        </View>
      ) : null}
      {canManageStudents ? <InviteStudentAppSection client={client} /> : null}
      {canViewPayments ? (
        <View className="-mx-5">
          <Tabs
            options={[
              { value: "classes", label: translate("clients.detail.classes"), icon: "classes" },
              { value: "billing", label: translate("fees.client.title"), icon: "fees" },
            ]}
            selectedValue={visibleSection}
            onChange={setSection}
          />
        </View>
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
