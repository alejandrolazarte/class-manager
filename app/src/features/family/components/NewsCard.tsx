import { View } from "react-native";
import { newsWhenLabel, presentNews } from "@/features/family/familyNewsPresentation";
import { FamilyNewsItem } from "@/features/family/types";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Button } from "@/ui/Button";
import { Card } from "@/ui/Card";
import { Icon } from "@/ui/Icon";

interface NewsCardProps {
  item: FamilyNewsItem;
  action?: { label: string; onPress: () => void };
}

export function NewsCard({ item, action }: NewsCardProps) {
  const { icon, title, body } = presentNews(item);
  return (
    <Card className={`flex-row gap-3 px-4 py-3.5 ${item.isUnread ? "" : "opacity-90"}`}>
      <View className="h-10 w-10 items-center justify-center rounded-xl bg-primary-soft">
        <Icon name={icon} tone="primary-soft-foreground" />
      </View>
      <View className="min-w-0 flex-1 gap-0.5 pr-3">
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
        <AppText variant="footnote" tone="subtle" className="mt-1 font-label">
          {newsWhenLabel(item)}
        </AppText>
      </View>
      {item.isUnread ? (
        <View
          accessibilityLabel={translate("family.news.unread")}
          className="absolute right-4 top-[18px] h-2.5 w-2.5 rounded-full bg-primary"
        />
      ) : null}
    </Card>
  );
}
