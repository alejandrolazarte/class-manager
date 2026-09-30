import { useRouter } from "expo-router";
import { useState } from "react";
import { View } from "react-native";
import { LevelDraft, LevelRow } from "@/features/achievements/components/LevelRow";
import { levelErrorOf, maxLevels, minLevels } from "@/features/achievements/levelRules";
import { AchievementLevel, AchievementSettings } from "@/features/achievements/types";
import {
  useAchievementSettings,
  useUpdateAchievementSettings,
} from "@/features/achievements/useAchievementSettings";
import { SettingsFormScreenLayout } from "@/features/settings/components/SettingsFormScreenLayout";
import { SubmissionFailure, toSubmissionFailure } from "@/features/settings/submissionFailure";
import { translate, TranslationKey } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Button } from "@/ui/Button";
import { Card } from "@/ui/Card";
import { SectionTitle } from "@/ui/SectionTitle";
import { Spinner } from "@/ui/Spinner";
import { ToggleSwitch } from "@/ui/ToggleSwitch";
import { useToast } from "@/ui/ToastProvider";

const emptyLevel: LevelDraft = { name: "", requiredClasses: "" };

function toDraft(level: AchievementLevel): LevelDraft {
  return { name: level.name, requiredClasses: String(level.requiredClasses) };
}

function toLevel(draft: LevelDraft): AchievementLevel {
  const requiredClasses = draft.requiredClasses.trim();
  return {
    name: draft.name.trim(),
    requiredClasses: requiredClasses === "" ? Number.NaN : Number(requiredClasses),
  };
}

function AchievementSettingsForm({ settings }: { settings: AchievementSettings }) {
  const router = useRouter();
  const { showToast } = useToast();
  const updateMutation = useUpdateAchievementSettings();
  const [keepsStreak, setKeepsStreak] = useState(settings.noticedAbsencesKeepStreak);
  const [levels, setLevels] = useState<LevelDraft[]>(settings.levels.map(toDraft));
  const [levelError, setLevelError] = useState<TranslationKey | null>(null);
  const [submissionFailure, setSubmissionFailure] = useState<SubmissionFailure | null>(null);

  const changeLevel = (index: number, level: LevelDraft) =>
    setLevels((current) => current.map((existing, at) => (at === index ? level : existing)));
  const removeLevel = (index: number) =>
    setLevels((current) => current.filter((_, at) => at !== index));

  const save = async () => {
    const parsedLevels = levels.map(toLevel);
    const error = levelErrorOf(parsedLevels);
    setLevelError(error);
    setSubmissionFailure(null);
    if (error !== null) {
      return;
    }
    try {
      await updateMutation.mutateAsync({
        noticedAbsencesKeepStreak: keepsStreak,
        levels: parsedLevels,
      });
      showToast(translate("achievements.settings.saved"));
      router.back();
    } catch (saveError) {
      setSubmissionFailure(toSubmissionFailure(saveError));
    }
  };

  return (
    <SettingsFormScreenLayout
      title={translate("achievements.settings.title")}
      subtitle={translate("achievements.settings.subtitle")}
      submissionFailure={submissionFailure}
      onRetry={save}
    >
      <SectionTitle title={translate("achievements.settings.streakSection")} />
      <View className="gap-2">
        <ToggleSwitch
          label={translate("achievements.settings.noticedAbsencesKeepStreak")}
          value={keepsStreak}
          onValueChange={setKeepsStreak}
        />
        <AppText variant="caption" tone="muted">
          {translate(
            keepsStreak
              ? "achievements.settings.noticedAbsencesKeepStreakHint"
              : "achievements.settings.noticedAbsencesBreakStreakHint",
          )}
        </AppText>
      </View>
      <SectionTitle title={translate("achievements.settings.levelsSection")} />
      <AppText variant="body" tone="muted">
        {translate("achievements.settings.levelsHint")}
      </AppText>
      <Card className="gap-3 p-4">
        {levels.map((level, index) => (
          <LevelRow
            key={index}
            number={index + 1}
            level={level}
            isFirst={index === 0}
            canRemove={index > 0 && levels.length > minLevels}
            onChange={(changed) => changeLevel(index, changed)}
            onRemove={() => removeLevel(index)}
          />
        ))}
        {levels.length < maxLevels ? (
          <Button
            variant="secondary"
            size="medium"
            icon="add"
            label={translate("achievements.settings.addLevel")}
            onPress={() => setLevels((current) => [...current, emptyLevel])}
          />
        ) : null}
      </Card>
      {levelError === null ? null : (
        <AppText variant="body" tone="danger">
          {translate(levelError)}
        </AppText>
      )}
      <Button
        label={translate("common.save")}
        onPress={save}
        isLoading={updateMutation.isPending}
      />
    </SettingsFormScreenLayout>
  );
}

export function AchievementSettingsScreen() {
  const { data: settings } = useAchievementSettings();
  if (settings === undefined) {
    return <Spinner className="mt-6" />;
  }
  return <AchievementSettingsForm settings={settings} />;
}
