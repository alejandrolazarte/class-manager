import { useRouter } from "expo-router";
import { useCallback } from "react";
import { Pressable, View } from "react-native";
import { useCurrentBrand } from "@/features/brand/BrandProvider";
import { CurrentBrandLogo } from "@/features/brand/components/CurrentBrandLogo";
import { BillingTile } from "@/features/studentApp/components/BillingTile";
import { FeedbackCard } from "@/features/studentApp/components/FeedbackCard";
import { NewsBell } from "@/features/studentApp/components/NewsBell";
import { NewsCard } from "@/features/studentApp/components/NewsCard";
import { NextClassHero } from "@/features/studentApp/components/NextClassHero";
import { ShopPreview } from "@/features/studentApp/components/ShopPreview";
import { StreakTile } from "@/features/studentApp/components/StreakTile";
import { StudentChips } from "@/features/studentApp/components/StudentChips";
import { useSelectedStudent } from "@/features/studentApp/AccountStudentProvider";
import { firstNameOf } from "@/features/studentApp/studentAppSchedule";
import { useStudentAppHome } from "@/features/studentApp/useStudentAppHome";
import { useStudentAppNews } from "@/features/studentApp/useStudentAppNews";
import { useStudentAppShop } from "@/features/studentApp/useStudentAppShop";
import { useRefetchOnFocus } from "@/hooks/useRefetchOnFocus";
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

export function StudentAppHomeScreen() {
  const router = useRouter();
  const brandName = useCurrentBrand()?.brand?.displayName;
  const { data: home, isPending, isError, refetch } = useStudentAppHome();
  const { data: shop, refetch: refetchShop } = useStudentAppShop();
  const { data: news, refetch: refetchNews } = useStudentAppNews();
  useRefetchOnFocus(
    useCallback(() => {
      refetch();
      refetchShop();
      refetchNews();
    }, [refetch, refetchShop, refetchNews]),
  );
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
      <ScrollScreen header={<ScreenHeader title={translate("student.title")} />}>
        <Banner message={translate("student.loadError")}>
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
          leading={<CurrentBrandLogo />}
          eyebrow={brandName ?? home.businessName}
          title={translate("student.greeting", { name: firstNameOf(home.signedInFullName) })}
          accessory={
            news === undefined ? undefined : (
              <NewsBell
                unreadCount={news.unreadCount}
                onPress={() => router.navigate(routes.studentAppNews)}
              />
            )
          }
        />
      }
    >
      {student === null ? (
        <AppText variant="body" tone="muted">
          {translate("student.students.empty")}
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
            onSeeWeek={() => router.navigate(routes.studentAppClasses)}
          />
        </>
      )}
      <View className="flex-row gap-3">
        {student === null ? null : <StreakTile attendance={student.attendance} />}
        <BillingTile
          billing={home.billing}
          currencyCode={home.currencyCode}
          onBuyClasses={() => router.navigate(routes.studentAppShop)}
        />
      </View>
      {student?.latestFeedback ? <FeedbackCard feedback={student.latestFeedback} /> : null}
      {announcements.length === 0 ? null : (
        <View className="gap-3">
          <View className="flex-row items-baseline justify-between">
            <SectionTitle title={translate("student.news.homeTitle")} />
            <Pressable
              accessibilityRole="button"
              onPress={() => router.navigate(routes.studentAppNews)}
              className="active:opacity-70"
            >
              <AppText variant="link" tone="primary">
                {translate("student.news.seeAll")}
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
          onSeeAll={() => router.navigate(routes.studentAppShop)}
          onOpenProduct={(productId) => router.navigate(routes.studentAppProduct(productId))}
        />
      )}
    </ScrollScreen>
  );
}
