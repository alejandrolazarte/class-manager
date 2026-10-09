import { useRouter } from "expo-router";
import { useEffect } from "react";
import { View } from "react-native";
import { NewsCard } from "@/features/studentApp/components/NewsCard";
import {
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
import { Card } from "@/ui/Card";
import { EmptyState } from "@/ui/EmptyState";
import { ScrollScreen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";
import { SectionTitle } from "@/ui/SectionTitle";
import { Spinner } from "@/ui/Spinner";

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

  const itemsByGroup = (group: NewsGroup) =>
    (news?.items ?? []).filter((item) => newsGroupOf(item) === group);

  return (
    <ScrollScreen
      header={<ScreenHeader navigation="back" title={translate("student.news.title")} />}
    >
      {isPending ? <Spinner className="mt-6" /> : null}
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
        <Card>
          <EmptyState
            icon="notifications"
            iconTone="disabled-foreground"
            message={translate("student.news.empty")}
          />
        </Card>
      ) : null}
      {newsGroups.map((group) => {
        const items = itemsByGroup(group);
        return items.length === 0 ? null : (
          <View key={group} className="gap-2.5">
            <SectionTitle isOverline title={translate(newsGroupLabels[group])} />
            {items.map((item) => (
              <NewsCard key={item.id} item={item} action={actionFor(item)} />
            ))}
          </View>
        );
      })}
    </ScrollScreen>
  );
}
