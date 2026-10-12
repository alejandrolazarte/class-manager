import { useState } from "react";
import { View } from "react-native";
import { Announcement } from "@/features/announcements/types";
import {
  useAnnouncements,
  useCreateAnnouncement,
  useDeleteAnnouncement,
  useRestoreAnnouncement,
} from "@/features/announcements/useAnnouncements";
import { formatLongDate, toIsoDate } from "@/features/sessions/dates";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { EmptyState } from "@/ui/EmptyState";
import { HairlineRow } from "@/ui/HairlineRow";
import { IconButton } from "@/ui/IconButton";
import { ScrollScreen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";
import { SectionTitle } from "@/ui/SectionTitle";
import { AnimatedListItem } from "@/ui/motion/AnimatedListItem";
import { SkeletonList } from "@/ui/motion/SkeletonList";
import { TextField } from "@/ui/TextField";
import { useToast } from "@/ui/ToastProvider";
import { RequiredFieldsLegend } from "@/ui/RequiredFieldsLegend";

const titleMaxLength = 80;
const bodyMaxLength = 500;

interface AnnouncementRowProps {
  announcement: Announcement;
  isDeleting: boolean;
  onDelete: (announcement: Announcement) => void;
}

function AnnouncementRow({ announcement, isDeleting, onDelete }: AnnouncementRowProps) {
  return (
    <HairlineRow className="flex-row items-start gap-3 py-3">
      <View className="min-w-0 flex-1 gap-0.5">
        <AppText variant="bodyStrong">{announcement.title}</AppText>
        {announcement.body === null ? null : (
          <AppText variant="caption" tone="muted">
            {announcement.body}
          </AppText>
        )}
        <AppText variant="footnote" tone="subtle" className="mt-1">
          {translate("announcements.publishedOn", {
            date: formatLongDate(toIsoDate(new Date(announcement.publishedAt))),
          })}
        </AppText>
      </View>
      <IconButton
        icon="delete"
        tone="danger"
        accessibilityLabel={translate("announcements.delete", { title: announcement.title })}
        disabled={isDeleting}
        onPress={() => onDelete(announcement)}
      />
    </HairlineRow>
  );
}

export function AnnouncementsScreen() {
  const { showToast } = useToast();
  const deleteAnnouncementMutation = useDeleteAnnouncement();
  const restoreAnnouncementMutation = useRestoreAnnouncement();
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
  const { data: announcements = [], isPending, isError, refetch } = useAnnouncements();
  const createAnnouncementMutation = useCreateAnnouncement();
  const [title, setTitle] = useState("");
  const [body, setBody] = useState("");

  const publish = async () => {
    try {
      await createAnnouncementMutation.mutateAsync({
        title: title.trim(),
        body: body.trim().length === 0 ? null : body.trim(),
      });
      setTitle("");
      setBody("");
      showToast(translate("announcements.published"));
    } catch {
      showToast(translate("common.unexpectedError"));
    }
  };

  return (
    <ScrollScreen
      header={
        <ScreenHeader
          navigation="back"
          title={translate("announcements.title")}
          subtitle={translate("announcements.hint")}
        />
      }
    >
      <View className="gap-3">
        <SectionTitle title={translate("announcements.newTitle")} />
        <RequiredFieldsLegend />
        <TextField
          label={translate("announcements.form.title")}
          isRequired
          placeholder={translate("announcements.form.titlePlaceholder")}
          value={title}
          onChangeText={setTitle}
          maxLength={titleMaxLength}
        />
        <TextField
          label={translate("announcements.form.body")}
          placeholder={translate("announcements.form.bodyPlaceholder")}
          value={body}
          onChangeText={setBody}
          maxLength={bodyMaxLength}
          multiline
        />
        <Button
          icon="announcement"
          label={translate("announcements.publish")}
          disabled={title.trim().length === 0}
          isLoading={createAnnouncementMutation.isPending}
          onPress={publish}
        />
      </View>
      {isPending ? <SkeletonList /> : null}
      {isError ? (
        <Banner message={translate("common.unexpectedError")}>
          <Button
            variant="secondary"
            size="medium"
            label={translate("common.retry")}
            onPress={() => refetch()}
          />
        </Banner>
      ) : null}
      {!isPending && !isError && announcements.length === 0 ? (
        <EmptyState
          icon="announcement"
          iconTone="disabled-foreground"
          message={translate("announcements.empty")}
        />
      ) : null}
      <View>
        {announcements.map((announcement, index) => (
          <AnimatedListItem key={announcement.id} index={index}>
            <AnnouncementRow
              announcement={announcement}
              isDeleting={deleteAnnouncementMutation.isPending}
              onDelete={deleteAnnouncement}
            />
          </AnimatedListItem>
        ))}
      </View>
    </ScrollScreen>
  );
}
