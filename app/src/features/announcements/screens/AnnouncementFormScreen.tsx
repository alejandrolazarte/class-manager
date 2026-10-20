import { useRouter } from "expo-router";
import { useState } from "react";
import { View } from "react-native";
import {
  announcementCategories,
  announcementCategoryPresentations,
  defaultAnnouncementCategory,
} from "@/features/announcements/announcementCategories";
import {
  announcementActionOptionKeys,
  announcementActions,
  announcementActionUrlMaxLength,
  isHttpsLink,
} from "@/features/announcements/announcementActions";
import { AnnouncementPreviewSheet } from "@/features/announcements/components/AnnouncementPreviewSheet";
import {
  Announcement,
  AnnouncementAction,
  AnnouncementCategory,
  AnnouncementRequest,
} from "@/features/announcements/types";
import {
  useAnnouncements,
  useApplyAnnouncementImages,
  useCreateAnnouncement,
  useUpdateAnnouncement,
} from "@/features/announcements/useAnnouncements";
import { CatalogImageSaveOutcome, previewUriOf } from "@/features/catalogImages/catalogImageDraft";
import { catalogImageSaveMessageKey } from "@/features/catalogImages/catalogImageSaveMessage";
import { CatalogImageField } from "@/features/catalogImages/components/CatalogImageField";
import { useCatalogImageDraft } from "@/features/catalogImages/useCatalogImageDraft";
import { ClassGroupsField } from "@/features/classGroups/components/ClassGroupsField";
import { useActiveClassGroups } from "@/features/classGroups/useClassGroups";
import { formatClockTime, parseIsoDate, todayIsoDate, toIsoDate } from "@/dates/dates";
import { SettingsFormScreenLayout } from "@/features/settings/components/SettingsFormScreenLayout";
import { SettingsItemState } from "@/features/settings/components/SettingsItemState";
import { SubmissionFailure, toSubmissionFailure } from "@/features/settings/submissionFailure";
import { StudentAppNewsItem } from "@/features/studentApp/types";
import { formatBirthDateForDisplay, parseBirthDate } from "@/dates/birthDateFormatting";
import { DateField } from "@/forms/DateField";
import { TimeField } from "@/forms/TimeField";
import { isFilled } from "@/forms/requiredFields";
import { translate, TranslationKey } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Button } from "@/ui/Button";
import { ChipGroup } from "@/ui/ChipGroup";
import { RequiredFieldsLegend, withRequiredMark } from "@/ui/RequiredFieldsLegend";
import { SegmentedControl } from "@/ui/SegmentedControl";
import { TextField } from "@/ui/TextField";
import { ToggleSwitch } from "@/ui/ToggleSwitch";
import { useToast } from "@/ui/ToastProvider";

const titleMaxLength = 80;
const bodyMaxLength = 1000;
const maximumImageCount = 1;
const typedDateLength = 10;

type Audience = "everyone" | "classes";

type PublishTime = "now" | "scheduled";

const publishedMessageKeys: Record<CatalogImageSaveOutcome, TranslationKey> = {
  saved: "announcements.published",
  limitReached: "announcements.publishedWithLimitReached",
  failed: "announcements.publishedWithoutPhoto",
};

const previewItemId = "announcement-preview";

interface AnnouncementFormScreenProps {
  announcementId?: string;
}

interface AnnouncementEditorProps {
  announcement?: Announcement;
}

const timePattern = /^(\d{2}):(\d{2})$/;

function savedMessageKeyOf(
  isEditing: boolean,
  isScheduled: boolean,
  imagesOutcome: CatalogImageSaveOutcome,
): TranslationKey {
  if (isEditing) {
    return catalogImageSaveMessageKey(imagesOutcome, "announcements.saved");
  }
  return isScheduled && imagesOutcome === "saved"
    ? "announcements.scheduled"
    : publishedMessageKeys[imagesOutcome];
}

function scheduledInstantOf(typedDate: string, typedTime: string): Date | null {
  const isoDate = parseBirthDate(typedDate);
  const timeMatch = timePattern.exec(typedTime.trim());
  if (isoDate === null || timeMatch === null) {
    return null;
  }
  const date = parseIsoDate(isoDate);
  date.setHours(Number(timeMatch[1]), Number(timeMatch[2]), 0, 0);
  return date;
}

function AnnouncementEditor({ announcement }: AnnouncementEditorProps) {
  const router = useRouter();
  const { showToast } = useToast();
  const createAnnouncementMutation = useCreateAnnouncement();
  const updateAnnouncementMutation = useUpdateAnnouncement();
  const applyImagesMutation = useApplyAnnouncementImages();
  const images = useCatalogImageDraft(announcement?.images ?? []);
  const { data: classGroups = [] } = useActiveClassGroups();
  const isEditing = announcement !== undefined;
  const savedVisibleUntil = announcement?.visibleUntil ?? null;
  const savedPublishedAt =
    announcement?.isScheduled === true ? new Date(announcement.publishedAt) : null;
  const [category, setCategory] = useState<AnnouncementCategory>(
    announcement?.category ?? defaultAnnouncementCategory,
  );
  const [title, setTitle] = useState(announcement?.title ?? "");
  const [body, setBody] = useState(announcement?.body ?? "");
  const [audience, setAudience] = useState<Audience>(
    announcement !== undefined && announcement.classGroups.length > 0 ? "classes" : "everyone",
  );
  const [classGroupIds, setClassGroupIds] = useState<string[]>(
    announcement?.classGroups.map((classGroup) => classGroup.id) ?? [],
  );
  const [visibleUntil, setVisibleUntil] = useState(
    savedVisibleUntil === null ? "" : formatBirthDateForDisplay(savedVisibleUntil),
  );
  const [action, setAction] = useState<AnnouncementAction>(announcement?.action ?? "None");
  const [actionUrl, setActionUrl] = useState(announcement?.actionUrl ?? "");
  const [isPinned, setIsPinned] = useState(announcement?.isPinned ?? false);
  const [publishTime, setPublishTime] = useState<PublishTime>(
    savedPublishedAt === null ? "now" : "scheduled",
  );
  const [publishDate, setPublishDate] = useState(
    savedPublishedAt === null ? "" : formatBirthDateForDisplay(toIsoDate(savedPublishedAt)),
  );
  const [publishClockTime, setPublishClockTime] = useState(
    savedPublishedAt === null ? "" : formatClockTime(savedPublishedAt),
  );
  const [openedAt] = useState(() => Date.now());
  const [isPreviewOpen, setIsPreviewOpen] = useState(false);
  const [submissionFailure, setSubmissionFailure] = useState<SubmissionFailure | null>(null);

  const visibleUntilIsoDate = parseBirthDate(visibleUntil);
  const hasInvalidVisibleUntil =
    isFilled(visibleUntil) &&
    (visibleUntilIsoDate === null ||
      (visibleUntilIsoDate < todayIsoDate() && visibleUntilIsoDate !== savedVisibleUntil));
  const canChoosePublishTime = !isEditing || announcement.isScheduled;
  const isScheduled = canChoosePublishTime && publishTime === "scheduled";
  const publishOn = scheduledInstantOf(publishDate, publishClockTime);
  const hasInvalidPublishOn = isScheduled && publishOn !== null && publishOn.getTime() <= openedAt;
  const chosenActionUrl = action === "Link" ? actionUrl.trim() : null;
  const hasInvalidActionUrl =
    chosenActionUrl !== null && isFilled(chosenActionUrl) && !isHttpsLink(chosenActionUrl);
  const canPublish =
    isFilled(title) &&
    (chosenActionUrl === null || isHttpsLink(chosenActionUrl)) &&
    (audience === "everyone" || classGroupIds.length > 0) &&
    !hasInvalidVisibleUntil &&
    (!isScheduled || (publishOn !== null && !hasInvalidPublishOn));
  const isSaving =
    createAnnouncementMutation.isPending ||
    updateAnnouncementMutation.isPending ||
    applyImagesMutation.isPending;

  const chosenClassGroupIds = audience === "classes" ? classGroupIds : [];
  const firstImage = images.draft.items[0];
  const previewItem: StudentAppNewsItem = {
    id: previewItemId,
    kind: "Announcement",
    occurredAt: (isScheduled && publishOn !== null ? publishOn : new Date(openedAt)).toISOString(),
    isUnread: true,
    title: title.trim(),
    body: isFilled(body) ? body.trim() : null,
    studentFullNames: [],
    className: null,
    instructorFullName: null,
    classDate: null,
    orderId: null,
    classStartTime: null,
    announcementId: null,
    category,
    isPinned: isPinned && !isScheduled,
    isGoing: null,
    imageUrl: firstImage === undefined ? null : previewUriOf(firstImage),
    action,
    actionUrl: chosenActionUrl,
    audienceClassNames: classGroups
      .filter((classGroup) => chosenClassGroupIds.includes(classGroup.id))
      .map((classGroup) => classGroup.name),
  };

  const save = async () => {
    setSubmissionFailure(null);
    const request: AnnouncementRequest = {
      title: title.trim(),
      body: isFilled(body) ? body.trim() : null,
      category,
      classGroupIds: chosenClassGroupIds,
      isPinned: isPinned && !isScheduled,
      visibleUntil: visibleUntilIsoDate,
      publishOn: isScheduled && publishOn !== null ? publishOn.toISOString() : null,
      action,
      actionUrl: chosenActionUrl,
    };
    try {
      const savedAnnouncement =
        announcement === undefined
          ? await createAnnouncementMutation.mutateAsync(request)
          : await updateAnnouncementMutation.mutateAsync({
              announcementId: announcement.id,
              request,
            });
      const imagesOutcome = await applyImagesMutation.mutateAsync({
        announcementId: savedAnnouncement.id,
        draft: images.draft,
        savedImages: announcement?.images ?? [],
      });
      showToast(translate(savedMessageKeyOf(isEditing, isScheduled, imagesOutcome)));
      router.back();
    } catch (saveError) {
      setSubmissionFailure(toSubmissionFailure(saveError));
    }
  };

  return (
    <SettingsFormScreenLayout
      title={translate(isEditing ? "announcements.editTitle" : "announcements.newTitle")}
      subtitle={translate("announcements.hint")}
      submissionFailure={submissionFailure}
      onRetry={save}
    >
      <RequiredFieldsLegend />
      <View className="gap-2">
        <AppText variant="label" tone="muted">
          {withRequiredMark(translate("announcements.category.label"))}
        </AppText>
        <ChipGroup
          isScrollable
          options={announcementCategories.map((option) => ({
            value: option,
            label: translate(announcementCategoryPresentations[option].labelKey),
          }))}
          selectedValue={category}
          onChange={setCategory}
        />
      </View>
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
      <CatalogImageField
        images={images}
        placeholderIcon="photo"
        maximumImageCount={maximumImageCount}
        hint={translate("announcements.photoHint")}
      />
      <View className="gap-2">
        <AppText variant="label" tone="muted">
          {withRequiredMark(translate("announcements.audience.label"))}
        </AppText>
        <SegmentedControl
          options={[
            { value: "everyone", label: translate("announcements.audience.everyone") },
            { value: "classes", label: translate("announcements.audience.classes") },
          ]}
          selectedValue={audience}
          onChange={setAudience}
        />
        {audience === "classes" ? (
          <ClassGroupsField
            classGroups={classGroups}
            classGroupIds={classGroupIds}
            onChange={setClassGroupIds}
            chooseLabel={translate("announcements.audience.chooseClassGroup")}
            addLabel={translate("announcements.audience.addClassGroup")}
            removeLabel={(name) => translate("announcements.audience.removeClassGroup", { name })}
            hint={translate("announcements.audience.classesHint")}
            pickerTestID="announcement-class-group-picker"
          />
        ) : null}
      </View>
      <DateField
        label={translate("announcements.visibleUntil")}
        hint={translate("announcements.visibleUntilHint")}
        value={visibleUntil}
        onChangeText={setVisibleUntil}
        earliestIsoDate={todayIsoDate()}
        errorMessage={
          hasInvalidVisibleUntil && visibleUntil.trim().length >= typedDateLength
            ? translate("announcements.validation.visibleUntil")
            : undefined
        }
      />
      <View className="gap-2">
        <AppText variant="label" tone="muted">
          {translate("announcements.action.label")}
        </AppText>
        <ChipGroup
          isScrollable
          options={announcementActions.map((option) => ({
            value: option,
            label: translate(announcementActionOptionKeys[option]),
          }))}
          selectedValue={action}
          onChange={setAction}
        />
        {action === "Link" ? (
          <TextField
            label={translate("announcements.action.url")}
            isRequired
            placeholder={translate("announcements.action.urlPlaceholder")}
            value={actionUrl}
            onChangeText={setActionUrl}
            maxLength={announcementActionUrlMaxLength}
            autoCapitalize="none"
            keyboardType="url"
            errorMessage={
              hasInvalidActionUrl ? translate("announcements.validation.actionUrl") : undefined
            }
          />
        ) : null}
        <AppText variant="caption" tone="muted">
          {translate("announcements.action.hint")}
        </AppText>
      </View>
      {canChoosePublishTime ? (
        <View className="gap-2">
          <AppText variant="label" tone="muted">
            {translate("announcements.publishTime.label")}
          </AppText>
          <SegmentedControl
            options={[
              { value: "now", label: translate("announcements.publishTime.now") },
              { value: "scheduled", label: translate("announcements.publishTime.scheduled") },
            ]}
            selectedValue={publishTime}
            onChange={setPublishTime}
          />
          {isScheduled ? (
            <View className="gap-3">
              <DateField
                label={translate("announcements.publishTime.date")}
                isRequired
                value={publishDate}
                onChangeText={setPublishDate}
                earliestIsoDate={todayIsoDate()}
              />
              <TimeField
                label={translate("announcements.publishTime.time")}
                isRequired
                value={publishClockTime}
                onChangeText={setPublishClockTime}
                errorMessage={
                  hasInvalidPublishOn
                    ? translate("announcements.validation.publishTimeInThePast")
                    : undefined
                }
              />
              <AppText variant="caption" tone="muted">
                {translate("announcements.publishTime.hint")}
              </AppText>
            </View>
          ) : null}
        </View>
      ) : null}
      {isScheduled ? null : (
        <ToggleSwitch
          icon="pin"
          label={translate("announcements.pinToggle")}
          hint={translate("announcements.pinToggleHint")}
          value={isPinned}
          onValueChange={setIsPinned}
        />
      )}
      <Button
        variant="secondary"
        icon="preview"
        label={translate("announcements.preview.open")}
        disabled={!isFilled(title)}
        onPress={() => setIsPreviewOpen(true)}
      />
      <Button
        icon={isEditing ? undefined : "announcement"}
        label={translate(saveLabelKeyOf(isEditing, isScheduled))}
        disabled={!canPublish}
        isLoading={isSaving}
        onPress={save}
      />
      {isPreviewOpen ? (
        <AnnouncementPreviewSheet item={previewItem} onClose={() => setIsPreviewOpen(false)} />
      ) : null}
    </SettingsFormScreenLayout>
  );
}

function saveLabelKeyOf(isEditing: boolean, isScheduled: boolean): TranslationKey {
  if (isEditing) {
    return "common.save";
  }
  return isScheduled ? "announcements.schedule" : "announcements.publish";
}

function EditAnnouncement({ announcementId }: { announcementId: string }) {
  const announcementsQuery = useAnnouncements();
  const announcement = announcementsQuery.data?.find(
    (candidate) => candidate.id === announcementId,
  );
  if (announcement === undefined) {
    return (
      <SettingsItemState
        navigation="close"
        isPending={announcementsQuery.isPending}
        isError={announcementsQuery.isError}
        notFoundMessage={translate("announcements.detail.notFound")}
        onRetry={() => announcementsQuery.refetch()}
      />
    );
  }
  return <AnnouncementEditor key={announcement.id} announcement={announcement} />;
}

export function AnnouncementFormScreen({ announcementId }: AnnouncementFormScreenProps) {
  return announcementId === undefined ? (
    <AnnouncementEditor />
  ) : (
    <EditAnnouncement announcementId={announcementId} />
  );
}
