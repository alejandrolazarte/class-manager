import { View } from "react-native";
import { CatalogImage } from "@/features/catalogImages/components/CatalogImage";
import { AnnouncementActionButton } from "@/features/studentApp/components/AnnouncementActionButton";
import { EventReplyButtons } from "@/features/studentApp/components/EventReplyButtons";
import { newsWhenLabel, presentNews } from "@/features/studentApp/studentAppNewsPresentation";
import { StudentAppNewsItem, StudentAppNewsKind } from "@/features/studentApp/types";
import { translate } from "@/i18n/translate";
import { ThemeColorToken } from "@/theme/themeColorTokens";
import { AppText } from "@/ui/AppText";
import { Avatar } from "@/ui/Avatar";
import { Button } from "@/ui/Button";
import { HairlineRow } from "@/ui/HairlineRow";
import { IconCircle } from "@/ui/IconCircle";
import { StatusPill } from "@/ui/StatusPill";

interface NewsRowProps {
  item: StudentAppNewsItem;
  action?: { label: string; onPress: () => void };
}

const iconTones: Partial<Record<StudentAppNewsKind, ThemeColorToken>> = {
  ClassCancelled: "danger",
  ClassChanged: "warning",
};

export function NewsRow({ item, action }: NewsRowProps) {
  const { icon, iconTone, title, body, categoryPill, audience } = presentNews(item);
  return (
    <HairlineRow className={`gap-3 py-3 ${item.isUnread ? "" : "opacity-85"}`}>
      {item.imageUrl === null ? null : (
        <CatalogImage
          imageUri={item.imageUrl}
          placeholderIcon="photo"
          className="h-44 w-full rounded-[14px]"
        />
      )}
      <View className="flex-row gap-3">
        {item.kind === "InstructorFeedback" && item.instructorFullName !== null ? (
          <Avatar name={item.instructorFullName} photoUrl={item.instructorPhotoUrl} />
        ) : (
          <IconCircle icon={icon} tone={iconTone ?? iconTones[item.kind] ?? "primary"} />
        )}
        <View className="min-w-0 flex-1 gap-0.5">
          {categoryPill === undefined && audience === undefined ? null : (
            <View className="mb-1 flex-row flex-wrap items-center gap-1.5">
              {categoryPill === undefined ? null : (
                <StatusPill label={categoryPill.label} tone={categoryPill.tone} isSmall />
              )}
              {audience === undefined ? null : (
                <AppText variant="footnote" tone="muted">
                  {audience}
                </AppText>
              )}
            </View>
          )}
          <AppText variant="bodyStrong">{title}</AppText>
          {body === null ? null : (
            <AppText variant="caption" tone="muted">
              {body}
            </AppText>
          )}
          {item.category === "Event" ? (
            <EventReplyButtons announcementId={item.announcementId} isGoing={item.isGoing} />
          ) : null}
          <AnnouncementActionButton item={item} />
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
      </View>
    </HairlineRow>
  );
}
