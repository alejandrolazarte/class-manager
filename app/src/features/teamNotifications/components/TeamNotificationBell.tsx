import { useRouter } from "expo-router";
import { NewsBell } from "@/features/family/components/NewsBell";
import { useTeamNotifications } from "@/features/teamNotifications/useTeamNotifications";
import { useRefetchOnFocus } from "@/hooks/useRefetchOnFocus";
import { routes } from "@/navigation/routes";

export function TeamNotificationBell() {
  const router = useRouter();
  const { data: notifications, refetch } = useTeamNotifications();
  useRefetchOnFocus(refetch);
  return (
    <NewsBell
      unreadCount={notifications?.unreadCount ?? 0}
      onPress={() => router.push(routes.teamNotifications)}
    />
  );
}
