import { useLocalSearchParams } from "expo-router";
import { AnnouncementFormScreen } from "@/features/announcements/screens/AnnouncementFormScreen";

export default function EditAnnouncementRoute() {
  const { announcementId } = useLocalSearchParams<{ announcementId: string }>();
  return <AnnouncementFormScreen announcementId={announcementId} />;
}
