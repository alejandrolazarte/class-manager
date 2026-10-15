import { useRouter } from "expo-router";
import { useState } from "react";
import { AnnouncementRow } from "@/features/announcements/components/AnnouncementRow";
import { Announcement } from "@/features/announcements/types";
import {
  useAnnouncements,
  useDeleteAnnouncement,
  useRestoreAnnouncement,
  useSetAnnouncementPinned,
} from "@/features/announcements/useAnnouncements";
import { SettingsListScreenLayout } from "@/features/settings/components/SettingsListScreenLayout";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { ChipGroup } from "@/ui/ChipGroup";
import { useToast } from "@/ui/ToastProvider";

type AnnouncementFilter = "published" | "scheduled";

export function AnnouncementsScreen() {
  const router = useRouter();
  const { showToast } = useToast();
  const {
    data: announcements = [],
    isPending,
    isError,
    isRefetching,
    refetch,
  } = useAnnouncements();
  const deleteAnnouncementMutation = useDeleteAnnouncement();
  const restoreAnnouncementMutation = useRestoreAnnouncement();
  const setPinnedMutation = useSetAnnouncementPinned();
  const [filter, setFilter] = useState<AnnouncementFilter>("published");
  const scheduledCount = announcements.filter((announcement) => announcement.isScheduled).length;
  const shownFilter = scheduledCount === 0 ? "published" : filter;
  const shownAnnouncements = announcements.filter(
    (announcement) => announcement.isScheduled === (shownFilter === "scheduled"),
  );

  const deleteAnnouncement = async (announcement: Announcement) => {
    try {
      await deleteAnnouncementMutation.mutateAsync(announcement.id);
      showToast(translate("announcements.deleted"), {
        label: translate("common.undo"),
        onPress: () => {
          restoreAnnouncementMutation
            .mutateAsync(announcement.id)
            .catch(() => showToast(translate("common.undoFailed")));
        },
      });
    } catch {
      showToast(translate("common.unexpectedError"));
    }
  };

  const togglePinned = async (announcement: Announcement) => {
    const isPinned = !announcement.isPinned;
    try {
      await setPinnedMutation.mutateAsync({ announcementId: announcement.id, isPinned });
      showToast(translate(isPinned ? "announcements.pinnedToast" : "announcements.unpinnedToast"));
    } catch {
      showToast(translate("common.unexpectedError"));
    }
  };

  return (
    <SettingsListScreenLayout
      title={translate("announcements.title")}
      subtitle={translate("announcements.hint")}
      items={shownAnnouncements}
      filters={
        scheduledCount === 0 ? undefined : (
          <ChipGroup
            options={[
              {
                value: "published",
                label: translate("announcements.filter.published"),
                count: announcements.length - scheduledCount,
              },
              {
                value: "scheduled",
                label: translate("announcements.filter.scheduled"),
                count: scheduledCount,
              },
            ]}
            selectedValue={shownFilter}
            onChange={setFilter}
          />
        )
      }
      isPending={isPending}
      isError={isError}
      isRefetching={isRefetching}
      onRefetch={() => refetch()}
      keyExtractor={(announcement) => announcement.id}
      renderItem={(announcement) => (
        <AnnouncementRow
          announcement={announcement}
          isBusy={deleteAnnouncementMutation.isPending || setPinnedMutation.isPending}
          onOpen={(opened) => router.push(routes.announcement(opened.id))}
          onEdit={(edited) => router.push(routes.editAnnouncement(edited.id))}
          onTogglePinned={togglePinned}
          onDelete={deleteAnnouncement}
        />
      )}
      emptyIcon="announcement"
      emptyMessage={translate("announcements.empty")}
      newItemLabel={translate("announcements.newTitle")}
      onNewItem={() => router.push(routes.newAnnouncement)}
    />
  );
}
