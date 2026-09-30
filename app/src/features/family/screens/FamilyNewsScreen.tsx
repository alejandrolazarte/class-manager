import { useRouter } from "expo-router";
import { useEffect } from "react";
import { View } from "react-native";
import { NewsCard } from "@/features/family/components/NewsCard";
import {
  NewsGroup,
  newsGroupLabels,
  newsGroupOf,
  newsGroups,
} from "@/features/family/familyNewsPresentation";
import { FamilyNewsItem } from "@/features/family/types";
import { useFamilyNews, useMarkFamilyNewsSeen } from "@/features/family/useFamilyNews";
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

export function FamilyNewsScreen() {
  const router = useRouter();
  const { data: news, isPending, isError, refetch } = useFamilyNews();
  const { mutate: markSeen } = useMarkFamilyNewsSeen();
  const hasUnread = (news?.unreadCount ?? 0) > 0;

  useEffect(() => {
    if (hasUnread) {
      markSeen();
    }
  }, [hasUnread, markSeen]);

  const actionFor = (item: FamilyNewsItem) => {
    if (item.kind === "OrderReady") {
      return {
        label: translate("family.news.orderReady.cta"),
        onPress: () => router.navigate(routes.familyOrders),
      };
    }
    if (item.kind === "ClassCancelled") {
      return {
        label: translate("family.news.cancelled.cta"),
        onPress: () => router.navigate(routes.familyClasses),
      };
    }
    return undefined;
  };

  const itemsByGroup = (group: NewsGroup) =>
    (news?.items ?? []).filter((item) => newsGroupOf(item) === group);

  return (
    <ScrollScreen
      header={<ScreenHeader navigation="back" title={translate("family.news.title")} />}
    >
      {isPending ? <Spinner className="mt-6" /> : null}
      {isError ? (
        <Banner message={translate("family.loadError")}>
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
            message={translate("family.news.empty")}
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
