import { View } from "react-native";
import { localDateOf } from "@/features/studentApp/studentAppNewsPresentation";
import { shortDayLabel } from "@/features/studentApp/studentAppSchedule";
import { TeamNotification } from "@/features/teamNotifications/types";
import { addDays, todayIsoDate } from "@/features/sessions/dates";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Card } from "@/ui/Card";
import { Icon } from "@/ui/Icon";

const timeDigits = 2;

function whenLabel(createdAt: string, today: string = todayIsoDate()): string {
  const date = localDateOf(createdAt);
  if (date === today) {
    const created = new Date(createdAt);
    return `${String(created.getHours()).padStart(timeDigits, "0")}:${String(created.getMinutes()).padStart(timeDigits, "0")}`;
  }
  return date === addDays(today, -1) ? translate("student.news.yesterday") : shortDayLabel(date);
}

interface TeamNotificationCardProps {
  notification: TeamNotification;
  onPress: () => void;
}

export function TeamNotificationCard({ notification, onPress }: TeamNotificationCardProps) {
  return (
    <Card
      onPress={onPress}
      accessibilityLabel={notification.title}
      className="flex-row gap-3 px-4 py-3.5"
    >
      <View className="h-10 w-10 items-center justify-center rounded-xl bg-primary-soft">
        <Icon name="notifications" tone="primary-soft-foreground" />
      </View>
      <View className="min-w-0 flex-1 gap-0.5 pr-3">
        <AppText variant="bodyStrong">{notification.title}</AppText>
        <AppText variant="caption" tone="muted">
          {notification.body}
        </AppText>
        <AppText variant="footnote" tone="subtle" className="mt-1 font-label">
          {whenLabel(notification.createdAt)}
        </AppText>
      </View>
      {notification.isUnread ? (
        <View
          accessibilityLabel={translate("student.news.unread")}
          className="absolute right-4 top-[18px] h-2.5 w-2.5 rounded-full bg-primary"
        />
      ) : null}
    </Card>
  );
}
