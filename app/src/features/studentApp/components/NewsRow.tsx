import { View } from "react-native";
import { newsWhenLabel, presentNews } from "@/features/studentApp/studentAppNewsPresentation";
import { StudentAppNewsItem, StudentAppNewsKind } from "@/features/studentApp/types";
import { translate } from "@/i18n/translate";
import { ThemeColorToken } from "@/theme/themeColorTokens";
import { AppText } from "@/ui/AppText";
import { Button } from "@/ui/Button";
import { HairlineRow } from "@/ui/HairlineRow";
import { Icon } from "@/ui/Icon";

interface NewsRowProps {
  item: StudentAppNewsItem;
  action?: { label: string; onPress: () => void };
}

interface NewsIconTone {
  background: string;
  icon: ThemeColorToken;
}

const primaryIconTone: NewsIconTone = {
  background: "bg-primary-soft",
  icon: "primary-soft-foreground",
};

const iconTones: Partial<Record<StudentAppNewsKind, NewsIconTone>> = {
  ClassCancelled: { background: "bg-danger-soft", icon: "danger-soft-foreground" },
  ClassChanged: { background: "bg-warning-soft", icon: "warning-soft-foreground" },
};

export function NewsRow({ item, action }: NewsRowProps) {
  const { icon, title, body } = presentNews(item);
  const iconTone = iconTones[item.kind] ?? primaryIconTone;
  return (
    <HairlineRow className={`flex-row gap-3 ${item.isUnread ? "" : "opacity-85"}`}>
      <View className={`h-9 w-9 items-center justify-center rounded-full ${iconTone.background}`}>
        <Icon name={icon} size="medium" tone={iconTone.icon} />
      </View>
      <View className="min-w-0 flex-1 gap-0.5">
        <AppText variant="bodyStrong">{title}</AppText>
        {body === null ? null : (
          <AppText variant="caption" tone="muted">
            {body}
          </AppText>
        )}
        {action === undefined ? null : (
          <View className="mt-2 self-start">
            <Button size="medium" label={action.label} onPress={action.onPress} />
          </View>
        )}
        <AppText variant="footnote" tone="subtle" className="mt-1">
          {newsWhenLabel(item)}
        </AppText>
      </View>
      {item.isUnread ? (
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
