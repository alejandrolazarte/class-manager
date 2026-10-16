import { useRouter } from "expo-router";
import { Linking, View } from "react-native";
import { announcementActionButtonKeys } from "@/features/announcements/announcementActions";
import { StudentAppNewsItem } from "@/features/studentApp/types";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { Button } from "@/ui/Button";

interface AnnouncementActionButtonProps {
  item: StudentAppNewsItem;
}

export function AnnouncementActionButton({ item }: AnnouncementActionButtonProps) {
  const router = useRouter();
  const { action, actionUrl } = item;
  if (action === null || action === "None") {
    return null;
  }
  const isPreview = item.announcementId === null;

  const open = () => {
    if (isPreview) {
      return;
    }
    if (action === "Shop") {
      router.navigate(routes.studentAppShop);
    } else if (action === "Classes") {
      router.navigate(routes.studentAppClasses);
    } else if (actionUrl !== null) {
      void Linking.openURL(actionUrl);
    }
  };

  return (
    <View className="mt-2 self-start">
      <Button
        size="medium"
        icon={action === "Link" ? "openExternal" : undefined}
        label={translate(announcementActionButtonKeys[action])}
        onPress={open}
      />
    </View>
  );
}
