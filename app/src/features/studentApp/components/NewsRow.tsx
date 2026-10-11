import { View } from "react-native";
import { newsWhenLabel, presentNews } from "@/features/studentApp/studentAppNewsPresentation";
import { StudentAppNewsItem, StudentAppNewsKind } from "@/features/studentApp/types";
import { translate } from "@/i18n/translate";
import { ThemeColorToken } from "@/theme/themeColorTokens";
import { AppText } from "@/ui/AppText";
import { Button } from "@/ui/Button";
import { HairlineRow } from "@/ui/HairlineRow";
import { IconCircle } from "@/ui/IconCircle";

interface NewsRowProps {
  item: StudentAppNewsItem;
  action?: { label: string; onPress: () => void };
}

const iconTones: Partial<Record<StudentAppNewsKind, ThemeColorToken>> = {
  ClassCancelled: "danger",
  ClassChanged: "warning",
};

export function NewsRow({ item, action }: NewsRowProps) {
  const { icon, title, body } = presentNews(item);
  return (
    <HairlineRow className={`flex-row gap-3 py-3 ${item.isUnread ? "" : "opacity-85"}`}>
      <IconCircle icon={icon} tone={iconTones[item.kind] ?? "primary"} />
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
