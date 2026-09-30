import { useRouter } from "expo-router";
import { View } from "react-native";
import { BillingTile } from "@/features/family/components/BillingTile";
import { FeedbackCard } from "@/features/family/components/FeedbackCard";
import { NextClassHero } from "@/features/family/components/NextClassHero";
import { ShopPreview } from "@/features/family/components/ShopPreview";
import { StreakTile } from "@/features/family/components/StreakTile";
import { StudentChips } from "@/features/family/components/StudentChips";
import { useSelectedStudent } from "@/features/family/FamilyStudentProvider";
import { firstNameOf } from "@/features/family/familySchedule";
import { useFamilyHome } from "@/features/family/useFamilyHome";
import { useFamilyShop } from "@/features/family/useFamilyShop";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { AppText } from "@/ui/AppText";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { Screen, ScrollScreen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";
import { Spinner } from "@/ui/Spinner";

export function FamilyHomeScreen() {
  const router = useRouter();
  const { data: home, isPending, isError, refetch } = useFamilyHome();
  const { data: shop } = useFamilyShop();
  const { student, selectStudent } = useSelectedStudent(home?.students ?? []);

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
      </ScrollScreen>
    );
  }

  return (
    <ScrollScreen
      header={
        <ScreenHeader
          eyebrow={home.businessName}
          title={translate("family.greeting", { name: firstNameOf(home.clientFullName) })}
        />
      }
    >
      {student === null ? (
        <AppText variant="body" tone="muted">
          {translate("family.students.empty")}
        </AppText>
      ) : (
        <>
          <StudentChips
            students={home.students}
            selectedStudentId={student.id}
            onSelect={selectStudent}
          />
          <NextClassHero
            student={student}
            onSeeWeek={() => router.navigate(routes.familyClasses)}
          />
        </>
      )}
      <View className="flex-row gap-3">
        {student === null ? null : <StreakTile attendance={student.attendance} />}
        <BillingTile
          billing={home.billing}
          currencyCode={home.currencyCode}
          onBuyClasses={() => router.navigate(routes.familyShop)}
        />
      </View>
      {student?.latestFeedback ? <FeedbackCard feedback={student.latestFeedback} /> : null}
      {shop === undefined ? null : (
        <ShopPreview
          shop={shop}
          onSeeAll={() => router.navigate(routes.familyShop)}
          onOpenProduct={(productId) => router.navigate(routes.familyProduct(productId))}
        />
      )}
    </ScrollScreen>
  );
}
