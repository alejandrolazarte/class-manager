import { useRouter } from "expo-router";
import { useEffect } from "react";
import { NewsRow } from "@/features/studentApp/components/NewsRow";
import { PinnedNews } from "@/features/studentApp/components/PinnedNews";
import {
  isPinnedAnnouncement,
  NewsGroup,
  newsGroupLabels,
  newsGroupOf,
  newsGroups,
} from "@/features/studentApp/studentAppNewsPresentation";
import { StudentAppNewsItem } from "@/features/studentApp/types";
import {
  useStudentAppNews,
  useMarkStudentAppNewsSeen,
} from "@/features/studentApp/useStudentAppNews";
import { useRefetchOnFocus } from "@/hooks/useRefetchOnFocus";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { EmptyState } from "@/ui/EmptyState";
import { ScrollScreen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";
import { SectionTitle } from "@/ui/SectionTitle";
import { AnimatedListItem } from "@/ui/motion/AnimatedListItem";
import { SkeletonList } from "@/ui/motion/SkeletonList";

export function StudentAppNewsScreen() {
  const router = useRouter();
  const { data: news, isPending, isError, refetch } = useStudentAppNews();
  useRefetchOnFocus(refetch);
  const { mutate: markSeen } = useMarkStudentAppNewsSeen();
  const hasUnread = (news?.unreadCount ?? 0) > 0;

  useEffect(() => {
    if (hasUnread) {
      markSeen();
    }
  }, [hasUnread, markSeen]);

  const actionFor = (item: StudentAppNewsItem) => {
    if (item.kind === "OrderReady") {
      return {
        label: translate("student.news.orderReady.cta"),
        onPress: () => router.navigate(routes.studentAppOrders),
      };
    }
    if (item.kind === "ClassCancelled" || item.kind === "ClassChanged") {
      return {
        label: translate("student.news.cancelled.cta"),
        onPress: () => router.navigate(routes.studentAppClasses),
      };
    }
    return undefined;
  };

  const pinnedItem = (news?.items ?? []).find(isPinnedAnnouncement);
  const itemsByGroup = (group: NewsGroup) =>
    (news?.items ?? []).filter((item) => item !== pinnedItem && newsGroupOf(item) === group);

  return (
    <ScrollScreen
      header={<ScreenHeader navigation="back" title={translate("student.news.title")} />}
    >
      {isPending ? <SkeletonList /> : null}
      {isError ? (
        <Banner message={translate("student.loadError")}>
          <Button
            variant="secondary"
            size="medium"
            label={translate("common.retry")}
            onPress={() => refetch()}
          />
        </Banner>
      ) : null}
      {news !== undefined && news.items.length === 0 ? (
        <EmptyState
          icon="notifications"
          iconTone="disabled-foreground"
          message={translate("student.news.empty")}
        />
      ) : null}
      {pinnedItem === undefined ? null : (
        <AnimatedListItem index={0}>
          <PinnedNews item={pinnedItem} />
        </AnimatedListItem>
      )}
      {newsGroups
        .filter((group) => itemsByGroup(group).length > 0)
        .map((group, groupIndex) => (
          <AnimatedListItem key={group} index={groupIndex} className="mt-1">
            <SectionTitle isOverline title={translate(newsGroupLabels[group])} />
            {itemsByGroup(group).map((item) => (
              <NewsRow key={item.id} item={item} action={actionFor(item)} />
            ))}
          </AnimatedListItem>
        ))}
    </ScrollScreen>
  );
}
