import { View } from "react-native";
import { feeStatusLabel, feeStatusTones } from "@/features/fees/components/feeStatus";
import { formatMoney } from "@/features/fees/money";
import { useSession } from "@/features/authentication/useSession";
import { useFamilyHome } from "@/features/family/useFamilyHome";
import { useRouter } from "expo-router";
import { FamilyBilling, FamilyNextClass, FamilyStudent } from "@/features/family/types";
import { formatLongDate } from "@/features/sessions/dates";
import { translate, translateCount } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { AppText } from "@/ui/AppText";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { Card } from "@/ui/Card";
import { Icon } from "@/ui/Icon";
import { Screen, ScrollScreen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";
import { SectionTitle } from "@/ui/SectionTitle";
import { Spinner } from "@/ui/Spinner";
import { StatusPill } from "@/ui/StatusPill";

const detailSeparator = " · ";

function NextClassRow({ nextClass }: { nextClass: FamilyNextClass }) {
  const details = [
    `${nextClass.startTime}–${nextClass.endTime}`,
    nextClass.isPrivateLesson ? translate("family.student.privateLesson") : nextClass.name,
    ...(nextClass.instructorFullName
      ? [translate("family.student.with", { instructor: nextClass.instructorFullName })]
      : []),
  ];
  return (
    <View className={`gap-0.5 ${nextClass.isCancelled ? "opacity-60" : ""}`}>
      <AppText variant="label" tone="muted">
        {formatLongDate(nextClass.date)}
      </AppText>
      <AppText variant="bodyStrong">{details.join(detailSeparator)}</AppText>
      {nextClass.isCancelled ? (
        <View className="flex-row items-center gap-1">
          <Icon name="cancelled" size="small" tone="danger" />
          <AppText variant="label" tone="danger">
            {translate("family.student.cancelled")}
          </AppText>
        </View>
      ) : null}
    </View>
  );
}

function StudentCard({ student }: { student: FamilyStudent }) {
  return (
    <Card className="gap-3 px-4 py-3.5">
      <AppText variant="heading">{student.fullName}</AppText>
      {student.nextClasses.length === 0 ? (
        <AppText variant="body" tone="muted">
          {translate("family.student.noClasses")}
        </AppText>
      ) : (
        student.nextClasses.map((nextClass) => (
          <NextClassRow
            key={`${nextClass.date}-${nextClass.startTime}-${nextClass.name}`}
            nextClass={nextClass}
          />
        ))
      )}
    </Card>
  );
}

function BillingCard({ billing, currencyCode }: { billing: FamilyBilling; currencyCode: string }) {
  if (billing.monthlyFee !== null) {
    const { monthlyFee } = billing;
    return (
      <Card className="gap-2 px-4 py-3.5">
        <AppText variant="overline" tone="subtle">
          {translate("family.fee.title")}
        </AppText>
        <View className="flex-row items-center justify-between gap-2">
          <AppText variant="title">
            {monthlyFee.fee === null ? "—" : formatMoney(monthlyFee.fee, currencyCode)}
          </AppText>
          <StatusPill
            label={feeStatusLabel(monthlyFee, currencyCode)}
            tone={feeStatusTones[monthlyFee.status]}
          />
        </View>
        <AppText variant="caption" tone="subtle">
          {translate("family.payAtSchool")}
        </AppText>
      </Card>
    );
  }
  if (billing.classes !== null) {
    return (
      <Card className="gap-2 px-4 py-3.5">
        <AppText variant="overline" tone="subtle">
          {translate("family.classes.title")}
        </AppText>
        <AppText variant="title">
          {translateCount("family.classes.available", billing.classes.availableClasses)}
        </AppText>
        {billing.classes.unpaidClasses > 0 ? (
          <AppText variant="label" tone="danger">
            {translateCount("family.classes.unpaid", billing.classes.unpaidClasses)}
          </AppText>
        ) : null}
        <AppText variant="caption" tone="subtle">
          {translate("family.payAtSchool")}
        </AppText>
      </Card>
    );
  }
  return null;
}

export function FamilyHomeScreen() {
  const router = useRouter();
  const { signOut } = useSession();
  const { data: home, isPending, isError, refetch } = useFamilyHome();

  if (isPending) {
    return (
      <Screen header={<ScreenHeader title="" />}>
        <Spinner className="mt-6" />
      </Screen>
    );
  }
  if (isError || home === undefined) {
    return (
      <ScrollScreen header={<ScreenHeader title={translate("family.title")} />}>
        <Banner message={translate("family.loadError")}>
          <Button
            variant="secondary"
            size="medium"
            label={translate("common.retry")}
            onPress={() => refetch()}
          />
        </Banner>
        <Button
          variant="dangerOutline"
          size="medium"
          icon="signOut"
          label={translate("family.signOut")}
          onPress={signOut}
        />
      </ScrollScreen>
    );
  }

  return (
    <ScrollScreen
      header={
        <ScreenHeader
          eyebrow={translate("family.title")}
          title={home.businessName}
          subtitle={home.clientFullName}
        />
      }
    >
      <BillingCard billing={home.billing} currencyCode={home.currencyCode} />
      <View className="flex-row gap-2">
        <View className="flex-1">
          <Button
            size="medium"
            icon="products"
            label={translate("family.shop.title")}
            onPress={() => router.push(routes.familyShop)}
          />
        </View>
        <View className="flex-1">
          <Button
            size="medium"
            variant="secondary"
            icon="orders"
            label={translate("family.orders.title")}
            onPress={() => router.push(routes.familyOrders)}
          />
        </View>
      </View>
      <SectionTitle title={translate("family.students.title")} />
      {home.students.length === 0 ? (
        <AppText variant="body" tone="muted">
          {translate("family.students.empty")}
        </AppText>
      ) : (
        home.students.map((student) => <StudentCard key={student.id} student={student} />)
      )}
      <Button
        variant="dangerOutline"
        size="medium"
        icon="signOut"
        label={translate("family.signOut")}
        onPress={signOut}
      />
    </ScrollScreen>
  );
}
