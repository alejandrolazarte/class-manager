import { useRouter } from "expo-router";
import { useState } from "react";
import { View } from "react-native";
import {
  announcementCategories,
  announcementCategoryPresentations,
} from "@/features/announcements/announcementCategories";
import { AnnouncementCategory } from "@/features/announcements/types";
import {
  useApplyAnnouncementImages,
  useCreateAnnouncement,
} from "@/features/announcements/useAnnouncements";
import { CatalogImageSaveOutcome } from "@/features/catalogImages/catalogImageDraft";
import { CatalogImageField } from "@/features/catalogImages/components/CatalogImageField";
import { useCatalogImageDraft } from "@/features/catalogImages/useCatalogImageDraft";
import { ClassGroupsField } from "@/features/classGroups/components/ClassGroupsField";
import { useActiveClassGroups } from "@/features/classGroups/useClassGroups";
import { parseIsoDate, todayIsoDate } from "@/features/sessions/dates";
import { SettingsFormScreenLayout } from "@/features/settings/components/SettingsFormScreenLayout";
import { SubmissionFailure, toSubmissionFailure } from "@/features/settings/submissionFailure";
import { parseBirthDate } from "@/features/students/birthDateFormatting";
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

const timePattern = /^(\d{2}):(\d{2})$/;

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

export function NewAnnouncementScreen() {
  const router = useRouter();
  const { showToast } = useToast();
  const createAnnouncementMutation = useCreateAnnouncement();
  const applyImagesMutation = useApplyAnnouncementImages();
  const images = useCatalogImageDraft([]);
  const { data: classGroups = [] } = useActiveClassGroups();
  const [category, setCategory] = useState<AnnouncementCategory>("News");
  const [title, setTitle] = useState("");
  const [body, setBody] = useState("");
  const [audience, setAudience] = useState<Audience>("everyone");
  const [classGroupIds, setClassGroupIds] = useState<string[]>([]);
  const [visibleUntil, setVisibleUntil] = useState("");
  const [isPinned, setIsPinned] = useState(false);
  const [publishTime, setPublishTime] = useState<PublishTime>("now");
  const [publishDate, setPublishDate] = useState("");
  const [publishClockTime, setPublishClockTime] = useState("");
  const [openedAt] = useState(() => Date.now());
  const [submissionFailure, setSubmissionFailure] = useState<SubmissionFailure | null>(null);

  const visibleUntilIsoDate = parseBirthDate(visibleUntil);
  const hasInvalidVisibleUntil =
    isFilled(visibleUntil) &&
    (visibleUntilIsoDate === null || visibleUntilIsoDate < todayIsoDate());
  const isScheduled = publishTime === "scheduled";
  const publishOn = scheduledInstantOf(publishDate, publishClockTime);
  const hasInvalidPublishOn = isScheduled && publishOn !== null && publishOn.getTime() <= openedAt;
  const canPublish =
    isFilled(title) &&
    (audience === "everyone" || classGroupIds.length > 0) &&
    !hasInvalidVisibleUntil &&
    (!isScheduled || (publishOn !== null && !hasInvalidPublishOn));

  const publish = async () => {
    setSubmissionFailure(null);
    try {
      const announcement = await createAnnouncementMutation.mutateAsync({
        title: title.trim(),
        body: isFilled(body) ? body.trim() : null,
        category,
        classGroupIds: audience === "classes" ? classGroupIds : [],
        isPinned: isPinned && !isScheduled,
        visibleUntil: visibleUntilIsoDate,
        publishOn: isScheduled && publishOn !== null ? publishOn.toISOString() : null,
      });
      const imagesOutcome = await applyImagesMutation.mutateAsync({
        announcementId: announcement.id,
        draft: images.draft,
      });
      showToast(
        translate(
          isScheduled && imagesOutcome === "saved"
            ? "announcements.scheduled"
            : publishedMessageKeys[imagesOutcome],
        ),
      );
      router.back();
    } catch (publishError) {
      setSubmissionFailure(toSubmissionFailure(publishError));
    }
  };

  return (
    <SettingsFormScreenLayout
      title={translate("announcements.newTitle")}
      subtitle={translate("announcements.hint")}
      submissionFailure={submissionFailure}
      onRetry={publish}
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
        icon="announcement"
        label={translate(isScheduled ? "announcements.schedule" : "announcements.publish")}
        disabled={!canPublish}
        isLoading={createAnnouncementMutation.isPending || applyImagesMutation.isPending}
        onPress={publish}
      />
    </SettingsFormScreenLayout>
  );
}
