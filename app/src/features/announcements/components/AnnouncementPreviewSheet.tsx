import { View } from "react-native";
import { useCurrentBusiness } from "@/features/business/CurrentBusinessProvider";
import { NewsRow } from "@/features/studentApp/components/NewsRow";
import { PinnedNews } from "@/features/studentApp/components/PinnedNews";
import { StudentAppNewsItem } from "@/features/studentApp/types";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { BottomSheet } from "@/ui/BottomSheet";
import { Button } from "@/ui/Button";
import { PushNotificationPreview } from "@/ui/PushNotificationPreview";

const pushBodyMaxLength = 160;
const ellipsis = "…";

interface AnnouncementPreviewSheetProps {
  item: StudentAppNewsItem;
  onClose: () => void;
}

function pushBodyOf(body: string | null): string | null {
  if (body === null || body.length <= pushBodyMaxLength) {
    return body;
  }
  return `${body.slice(0, pushBodyMaxLength - ellipsis.length)}${ellipsis}`;
}

export function AnnouncementPreviewSheet({ item, onClose }: AnnouncementPreviewSheetProps) {
  const business = useCurrentBusiness();
  return (
    <BottomSheet onClose={onClose}>
      <AppText variant="heading">{translate("announcements.preview.title")}</AppText>
      <View className="gap-2">
        <AppText variant="label" tone="muted">
          {translate("announcements.preview.push")}
        </AppText>
        <PushNotificationPreview
          appName={business.name}
          title={item.title ?? ""}
          body={pushBodyOf(item.body)}
        />
      </View>
      <View className="gap-2">
        <AppText variant="label" tone="muted">
          {translate("announcements.preview.app")}
        </AppText>
        {item.isPinned ? <PinnedNews item={item} /> : <NewsRow item={item} />}
      </View>
      <Button variant="secondary" label={translate("common.close")} onPress={onClose} />
    </BottomSheet>
  );
}
