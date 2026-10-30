import { View } from "react-native";
import { CatalogImage } from "@/features/catalogImages/components/CatalogImage";
import { AnnouncementActionButton } from "@/features/studentApp/components/AnnouncementActionButton";
import { EventReplyButtons } from "@/features/studentApp/components/EventReplyButtons";
import { presentNews } from "@/features/studentApp/studentAppNewsPresentation";
import { StudentAppNewsItem } from "@/features/studentApp/types";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Icon } from "@/ui/Icon";
import { OutlinedBlock } from "@/ui/OutlinedBlock";
import { StatusPill } from "@/ui/StatusPill";

interface PinnedNewsProps {
  item: StudentAppNewsItem;
}

export function PinnedNews({ item }: PinnedNewsProps) {
  const { title, body, audience, categoryPill } = presentNews(item);
  return (
    <View testID="pinned-news">
      <OutlinedBlock className="gap-3">
        {item.imageUrl === null ? null : (
          <CatalogImage
            imageUri={item.imageUrl}
            placeholderIcon="photo"
            className="h-40 w-full rounded-[12px]"
          />
        )}
        <View className="gap-1">
          <View className="flex-row flex-wrap items-center gap-1.5">
            <Icon name="pin" size="small" tone="muted-foreground" />
            <AppText variant="label" tone="muted">
              {[translate("student.news.pinned"), audience].filter(Boolean).join(" · ")}
            </AppText>
            {categoryPill === undefined ? null : (
              <StatusPill
                label={categoryPill.label}
                tone={categoryPill.tone}
                icon={categoryPill.icon}
                isSmall
              />
            )}
          </View>
          <AppText variant="heading">{title}</AppText>
          {body === null ? null : (
            <AppText variant="caption" tone="muted">
              {body}
            </AppText>
          )}
          {item.category === "Event" ? (
            <EventReplyButtons announcementId={item.announcementId} isGoing={item.isGoing} />
          ) : null}
          <AnnouncementActionButton item={item} />
        </View>
      </OutlinedBlock>
    </View>
  );
}
