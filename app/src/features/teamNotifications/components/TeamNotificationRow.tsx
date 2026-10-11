import { View } from "react-native";
import { localDateOf } from "@/features/studentApp/studentAppNewsPresentation";
import { shortDayLabel } from "@/features/studentApp/studentAppSchedule";
import { TeamNotification } from "@/features/teamNotifications/types";
import { addDays, todayIsoDate } from "@/features/sessions/dates";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { HairlineRow } from "@/ui/HairlineRow";
import { IconCircle } from "@/ui/IconCircle";

const timeDigits = 2;

function whenLabel(createdAt: string, today: string = todayIsoDate()): string {
  const date = localDateOf(createdAt);
  if (date === today) {
    const created = new Date(createdAt);
    return `${String(created.getHours()).padStart(timeDigits, "0")}:${String(created.getMinutes()).padStart(timeDigits, "0")}`;
  }
  return date === addDays(today, -1) ? translate("student.news.yesterday") : shortDayLabel(date);
}

interface TeamNotificationRowProps {
  notification: TeamNotification;
  onPress: () => void;
}

export function TeamNotificationRow({ notification, onPress }: TeamNotificationRowProps) {
  return (
    <HairlineRow
      onPress={onPress}
      accessibilityLabel={notification.title}
      className={`flex-row gap-3 py-3 ${notification.isUnread ? "" : "opacity-85"}`}
    >
      <IconCircle icon="notifications" tone="primary" />
      <View className="min-w-0 flex-1 gap-0.5">
        <AppText variant="bodyStrong">{notification.title}</AppText>
        <AppText variant="caption" tone="muted">
          {notification.body}
        </AppText>
        <AppText variant="footnote" tone="subtle" className="mt-1">
          {whenLabel(notification.createdAt)}
        </AppText>
      </View>
      {notification.isUnread ? (
        <View
          accessibilityLabel={translate("student.news.unread")}
          className="mt-1.5 h-2 w-2 rounded-full bg-primary"
        />
      ) : (
        <View className="h-2 w-2" />
      )}
    </HairlineRow>
  );
}
