import { Href, useRouter } from "expo-router";
import { useEffect } from "react";
import { TeamNotificationCard } from "@/features/teamNotifications/components/TeamNotificationCard";
import {
  useMarkTeamNotificationsSeen,
  useTeamNotifications,
} from "@/features/teamNotifications/useTeamNotifications";
import { useRefetchOnFocus } from "@/hooks/useRefetchOnFocus";
import { translate } from "@/i18n/translate";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { Card } from "@/ui/Card";
import { EmptyState } from "@/ui/EmptyState";
import { ScrollScreen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";
import { Spinner } from "@/ui/Spinner";

export function TeamNotificationsScreen() {
  const router = useRouter();
  const { data: notifications, isPending, isError, refetch } = useTeamNotifications();
  useRefetchOnFocus(refetch);
  const { mutate: markSeen } = useMarkTeamNotificationsSeen();
  const hasUnread = (notifications?.unreadCount ?? 0) > 0;

  useEffect(() => {
    if (hasUnread) {
      markSeen();
    }
  }, [hasUnread, markSeen]);

  return (
    <ScrollScreen
      header={<ScreenHeader navigation="back" title={translate("teamNotifications.title")} />}
    >
      {isPending ? <Spinner className="mt-6" /> : null}
      {isError ? (
        <Banner message={translate("common.unexpectedError")}>
          <Button
            variant="secondary"
            size="medium"
            label={translate("common.retry")}
            onPress={() => refetch()}
          />
        </Banner>
      ) : null}
      {notifications !== undefined && notifications.items.length === 0 ? (
        <Card>
          <EmptyState
            icon="notifications"
            iconTone="disabled-foreground"
            message={translate("teamNotifications.empty")}
          />
        </Card>
      ) : null}
      {notifications?.items.map((notification) => (
        <TeamNotificationCard
          key={notification.id}
          notification={notification}
          onPress={() => router.push(notification.url as Href)}
        />
      ))}
    </ScrollScreen>
  );
}
