import { useState } from "react";
import { View } from "react-native";
import { Announcement } from "@/features/announcements/types";
import {
  useAnnouncements,
  useCreateAnnouncement,
  useDeleteAnnouncement,
} from "@/features/announcements/useAnnouncements";
import { formatLongDate, toIsoDate } from "@/features/sessions/dates";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { Card } from "@/ui/Card";
import { DeleteConfirmation } from "@/ui/DeleteConfirmation";
import { EmptyState } from "@/ui/EmptyState";
import { IconButton } from "@/ui/IconButton";
import { ScrollScreen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";
import { SectionTitle } from "@/ui/SectionTitle";
import { Spinner } from "@/ui/Spinner";
import { TextField } from "@/ui/TextField";
import { useToast } from "@/ui/ToastProvider";
import { RequiredFieldsLegend } from "@/ui/RequiredFieldsLegend";

const titleMaxLength = 80;
const bodyMaxLength = 500;

function AnnouncementCard({ announcement }: { announcement: Announcement }) {
  const { showToast } = useToast();
  const deleteAnnouncementMutation = useDeleteAnnouncement();
  const [isConfirmingDelete, setIsConfirmingDelete] = useState(false);
  const remove = async () => {
    try {
      await deleteAnnouncementMutation.mutateAsync(announcement.id);
      showToast(translate("announcements.deleted"));
    } catch {
      showToast(translate("common.unexpectedError"));
    }
    setIsConfirmingDelete(false);
  };
  return (
    <View className="gap-2">
      <Card className="flex-row items-start gap-3 px-4 py-3.5">
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
          disabled={deleteAnnouncementMutation.isPending}
          onPress={() => setIsConfirmingDelete(true)}
        />
      </Card>
      {isConfirmingDelete ? (
        <DeleteConfirmation
          question={translate("announcements.deleteQuestion", { title: announcement.title })}
          onCancel={() => setIsConfirmingDelete(false)}
          onConfirm={remove}
          isDeleting={deleteAnnouncementMutation.isPending}
        />
      ) : null}
    </View>
  );
}

export function AnnouncementsScreen() {
  const { showToast } = useToast();
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
      <Card className="gap-3 p-4">
        <SectionTitle title={translate("announcements.newTitle")} />
        <RequiredFieldsLegend />
        <TextField
          label={translate("announcements.form.title")}
          isRequired
          placeholder={translate("announcements.form.titlePlaceholder")}
          value={title}
          onChangeText={setTitle}
          maxLength={titleMaxLength}
          fieldSurface="background"
        />
        <TextField
          label={translate("announcements.form.body")}
          placeholder={translate("announcements.form.bodyPlaceholder")}
          value={body}
          onChangeText={setBody}
          maxLength={bodyMaxLength}
          multiline
          fieldSurface="background"
        />
        <Button
          icon="announcement"
          label={translate("announcements.publish")}
          disabled={title.trim().length === 0}
          isLoading={createAnnouncementMutation.isPending}
          onPress={publish}
        />
      </Card>
      {isPending ? <Spinner className="mt-2" /> : null}
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
      {announcements.map((announcement) => (
        <AnnouncementCard key={announcement.id} announcement={announcement} />
      ))}
    </ScrollScreen>
  );
}
