import { useRouter } from "expo-router";
import { useCallback } from "react";
import { View } from "react-native";
import { AccountSwitcher } from "@/features/authentication/components/AccountSwitcher";
import { useCurrentBrand } from "@/features/brand/BrandProvider";
import { BillingTile, hasBillingTile } from "@/features/studentApp/components/BillingTile";
import { FeedbackNote } from "@/features/studentApp/components/FeedbackNote";
import { GuardianConsentCard } from "@/features/studentApp/components/GuardianConsentCard";
import { PhotoApprovalCards } from "@/features/userPhotos/components/PhotoApprovalCards";
import { MyAvatarButton } from "@/features/userPhotos/components/MyAvatarButton";
import { NewsBell } from "@/features/studentApp/components/NewsBell";
import { NewsRow } from "@/features/studentApp/components/NewsRow";
import { PinnedNews } from "@/features/studentApp/components/PinnedNews";
import { isPinnedAnnouncement } from "@/features/studentApp/studentAppNewsPresentation";
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
import { OutlinedBlock } from "@/ui/OutlinedBlock";
import { Screen, ScrollScreen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";
import { SectionTitle } from "@/ui/SectionTitle";
import { TextLink } from "@/ui/TextLink";
import { AnimatedListItem } from "@/ui/motion/AnimatedListItem";
import { Entrance } from "@/ui/motion/Entrance";
import { SkeletonDetail } from "@/ui/motion/SkeletonDetail";

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
  const pinnedAnnouncement = (news?.items ?? []).find(isPinnedAnnouncement);
  const announcements = (news?.items ?? [])
    .filter((item) => item.kind === "Announcement" && item !== pinnedAnnouncement)
    .slice(0, homeAnnouncementLimit);
  const { student, selectStudent } = useSelectedStudent(home?.students ?? []);

  if (isPending) {
    return (
      <Screen header={<ScreenHeader title="" />}>
        <SkeletonDetail className="w-full max-w-2xl self-center px-5 pt-4" />
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
          leading={<AccountSwitcher displayName={home.businessName} />}
          eyebrow={brandName ?? home.businessName}
          title={translate("student.greeting", { name: firstNameOf(home.signedInFullName) })}
          accessory={
            <View className="flex-row items-center gap-1">
              {news === undefined ? null : (
                <NewsBell
                  unreadCount={news.unreadCount}
                  onPress={() => router.navigate(routes.studentAppNews)}
                />
              )}
              <MyAvatarButton profileRoute={routes.studentAppProfile} />
            </View>
          }
        />
      }
    >
      {home.pendingGuardianConsents.map((consent) => (
        <GuardianConsentCard key={consent.invitationId} consent={consent} />
      ))}
      <PhotoApprovalCards />
      {student === null ? (
        <AppText variant="body" tone="muted">
          {translate("student.students.empty")}
        </AppText>
      ) : (
        <AnimatedListItem index={0} className="gap-4">
          <StudentChips
            students={home.students}
            selectedStudentId={student.id}
            onSelect={selectStudent}
          />
          <Entrance key={student.id} kind="fade">
            <NextClassHero
              student={student}
              onSeeWeek={() => router.navigate(routes.studentAppClasses)}
              classBalance={home.billing.classes}
              onBuyClasses={() => router.navigate(routes.studentAppShop)}
            />
          </Entrance>
        </AnimatedListItem>
      )}
      {student === null && !hasBillingTile(home.billing) ? null : (
        <AnimatedListItem index={1}>
          <View className="flex-row gap-3">
            {student === null ? null : (
              <OutlinedBlock className="min-w-0 flex-1 gap-1">
                <StreakTile attendance={student.attendance} />
              </OutlinedBlock>
            )}
            {hasBillingTile(home.billing) ? (
              <OutlinedBlock
                className="min-w-0 flex-1 gap-1"
                onPress={
                  home.billing.monthlyFee !== null || home.billing.classes === null
                    ? undefined
                    : () => router.navigate(routes.studentAppClassPacks)
                }
              >
                <BillingTile billing={home.billing} currencyCode={home.currencyCode} />
              </OutlinedBlock>
            ) : null}
          </View>
        </AnimatedListItem>
      )}
      {student?.latestFeedback ? (
        <AnimatedListItem index={2}>
          <OutlinedBlock>
            <FeedbackNote feedback={student.latestFeedback} />
          </OutlinedBlock>
        </AnimatedListItem>
      ) : null}
      {pinnedAnnouncement === undefined && announcements.length === 0 ? null : (
        <AnimatedListItem index={3} className="mt-1.5">
          <View className="flex-row items-center justify-between">
            <SectionTitle title={translate("student.news.homeTitle")} />
            <TextLink
              label={translate("student.news.seeAll")}
              onPress={() => router.navigate(routes.studentAppNews)}
            />
          </View>
          {pinnedAnnouncement === undefined ? null : (
            <View className="mt-1">
              <PinnedNews item={pinnedAnnouncement} />
            </View>
          )}
          {announcements.map((item) => (
            <NewsRow key={item.id} item={item} />
          ))}
        </AnimatedListItem>
      )}
      {shop === undefined ? null : (
        <AnimatedListItem index={4}>
          <ShopPreview
            shop={shop}
            onSeeAll={() => router.navigate(routes.studentAppShop)}
            onOpenProduct={(productId) => router.navigate(routes.studentAppProduct(productId))}
          />
        </AnimatedListItem>
      )}
    </ScrollScreen>
  );
}
