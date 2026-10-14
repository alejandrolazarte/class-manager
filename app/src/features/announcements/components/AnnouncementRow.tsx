import { View } from "react-native";
import { announcementCategoryPresentations } from "@/features/announcements/announcementCategories";
import { Announcement } from "@/features/announcements/types";
import { CatalogImage } from "@/features/catalogImages/components/CatalogImage";
import { formatLongDate, toIsoDate } from "@/features/sessions/dates";
import { joinNames } from "@/features/studentApp/studentAppNewsPresentation";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { HairlineRow } from "@/ui/HairlineRow";
import { IconButton } from "@/ui/IconButton";
import { IconCircle } from "@/ui/IconCircle";
import { StatusPill } from "@/ui/StatusPill";

interface AnnouncementRowProps {
  announcement: Announcement;
  isBusy: boolean;
  onTogglePinned: (announcement: Announcement) => void;
  onDelete: (announcement: Announcement) => void;
}

function audienceOf(announcement: Announcement): string {
  return announcement.classGroups.length === 0
    ? translate("announcements.audience.everyone")
    : joinNames(announcement.classGroups.map((classGroup) => classGroup.name));
}

export function AnnouncementRow({
  announcement,
  isBusy,
  onTogglePinned,
  onDelete,
}: AnnouncementRowProps) {
  const category = announcementCategoryPresentations[announcement.category];
  const photo = announcement.images[0];
  return (
    <HairlineRow className="flex-row items-start gap-3 py-3.5">
      {photo === undefined ? (
        <IconCircle icon={category.icon} tone={category.tone} />
      ) : (
        <CatalogImage
          imageUri={photo.url}
          placeholderIcon="photo"
          className="h-11 w-11 rounded-[10px]"
        />
      )}
      <View className="min-w-0 flex-1 gap-0.5">
        <View className="mb-1 flex-row flex-wrap gap-1.5">
          <StatusPill label={translate(category.labelKey)} tone={category.tone} isSmall />
          {announcement.isPinned ? (
            <StatusPill label={translate("announcements.pinnedPill")} tone="neutral" isSmall />
          ) : null}
        </View>
        <AppText variant="bodyStrong">{announcement.title}</AppText>
        {announcement.body === null ? null : (
          <AppText variant="caption" tone="muted" numberOfLines={2}>
            {announcement.body}
          </AppText>
        )}
        <AppText variant="caption" tone="muted">
          {audienceOf(announcement)}
        </AppText>
        <AppText variant="footnote" tone="subtle" className="mt-1">
          {translate("announcements.publishedOn", {
            date: formatLongDate(toIsoDate(new Date(announcement.publishedAt))),
          })}
        </AppText>
        {announcement.visibleUntil === null ? null : (
          <AppText variant="footnote" tone="subtle">
            {translate("announcements.visibleUntilOn", {
              date: formatLongDate(announcement.visibleUntil),
            })}
          </AppText>
        )}
      </View>
      <View className="flex-row">
        <IconButton
          icon="pin"
          tone={announcement.isPinned ? "primary" : "muted-foreground"}
          accessibilityLabel={translate(
            announcement.isPinned ? "announcements.unpin" : "announcements.pin",
            { title: announcement.title },
          )}
          disabled={isBusy}
          onPress={() => onTogglePinned(announcement)}
        />
        <IconButton
          icon="delete"
          tone="danger"
          accessibilityLabel={translate("announcements.delete", { title: announcement.title })}
          disabled={isBusy}
          onPress={() => onDelete(announcement)}
        />
      </View>
    </HairlineRow>
  );
}
