import { useLocalSearchParams } from "expo-router";
import { AnnouncementDetailScreen } from "@/features/announcements/screens/AnnouncementDetailScreen";

export default function AnnouncementRoute() {
  const { announcementId } = useLocalSearchParams<{ announcementId: string }>();
  return <AnnouncementDetailScreen announcementId={announcementId} />;
}
