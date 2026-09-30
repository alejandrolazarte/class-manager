import { useRouter } from "expo-router";
import { Pressable, View } from "react-native";
import { BillingTile } from "@/features/family/components/BillingTile";
import { FeedbackCard } from "@/features/family/components/FeedbackCard";
import { NewsBell } from "@/features/family/components/NewsBell";
import { NewsCard } from "@/features/family/components/NewsCard";
import { NextClassHero } from "@/features/family/components/NextClassHero";
import { ShopPreview } from "@/features/family/components/ShopPreview";
import { StreakTile } from "@/features/family/components/StreakTile";
import { StudentChips } from "@/features/family/components/StudentChips";
import { useSelectedStudent } from "@/features/family/FamilyStudentProvider";
import { firstNameOf } from "@/features/family/familySchedule";
import { useFamilyHome } from "@/features/family/useFamilyHome";
import { useFamilyNews } from "@/features/family/useFamilyNews";
import { useFamilyShop } from "@/features/family/useFamilyShop";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { AppText } from "@/ui/AppText";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { Screen, ScrollScreen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";
import { SectionTitle } from "@/ui/SectionTitle";
import { Spinner } from "@/ui/Spinner";

const homeAnnouncementLimit = 2;

export function FamilyHomeScreen() {
  const router = useRouter();
  const { data: home, isPending, isError, refetch } = useFamilyHome();
  const { data: shop } = useFamilyShop();
  const { data: news } = useFamilyNews();
  const announcements = (news?.items ?? [])
    .filter((item) => item.kind === "Announcement")
    .slice(0, homeAnnouncementLimit);
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
          accessory={
            news === undefined ? undefined : (
              <NewsBell
                unreadCount={news.unreadCount}
                onPress={() => router.navigate(routes.familyNews)}
              />
            )
          }
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
      {announcements.length === 0 ? null : (
        <View className="gap-3">
          <View className="flex-row items-baseline justify-between">
            <SectionTitle title={translate("family.news.homeTitle")} />
            <Pressable
              accessibilityRole="button"
              onPress={() => router.navigate(routes.familyNews)}
              className="active:opacity-70"
            >
              <AppText variant="link" tone="primary">
                {translate("family.news.seeAll")}
              </AppText>
            </Pressable>
          </View>
          {announcements.map((item) => (
            <NewsCard key={item.id} item={item} />
          ))}
        </View>
      )}
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
