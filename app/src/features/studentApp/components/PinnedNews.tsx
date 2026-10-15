import { View } from "react-native";
import { announcementCategoryPresentations } from "@/features/announcements/announcementCategories";
import { CatalogImage } from "@/features/catalogImages/components/CatalogImage";
import { EventReplyButtons } from "@/features/studentApp/components/EventReplyButtons";
import { presentNews } from "@/features/studentApp/studentAppNewsPresentation";
import { StudentAppNewsItem } from "@/features/studentApp/types";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Icon } from "@/ui/Icon";
import { TonalBlock } from "@/ui/TonalBlock";

interface PinnedNewsProps {
  item: StudentAppNewsItem;
}

export function PinnedNews({ item }: PinnedNewsProps) {
  const { title, body, audience } = presentNews(item);
  const tone =
    item.category === null ? "primary" : announcementCategoryPresentations[item.category].tone;
  return (
    <TonalBlock tone={tone} testID="pinned-news">
      {item.imageUrl === null ? null : (
        <CatalogImage
          imageUri={item.imageUrl}
          placeholderIcon="photo"
          className="h-40 w-full rounded-[12px]"
        />
      )}
      <View className="gap-1">
        <View className="flex-row items-center gap-1.5">
          <Icon name="pin" size="small" tone={tone} />
          <AppText variant="label" tone="muted">
            {[translate("student.news.pinned"), audience].filter(Boolean).join(" · ")}
          </AppText>
        </View>
        <AppText variant="heading">{title}</AppText>
        {body === null ? null : (
          <AppText variant="caption" tone="muted">
            {body}
          </AppText>
        )}
        {item.category === "Event" && item.announcementId !== null ? (
          <EventReplyButtons announcementId={item.announcementId} isGoing={item.isGoing} />
        ) : null}
      </View>
    </TonalBlock>
  );
}
