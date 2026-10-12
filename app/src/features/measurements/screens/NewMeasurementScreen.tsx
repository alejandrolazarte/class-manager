import { useRouter } from "expo-router";
import { useState } from "react";
import { View } from "react-native";
import { isApiError } from "@/api/apiErrors";
import { areFilled } from "@/forms/requiredFields";
import {
  directionLabel,
  formatMeasurementChange,
  formatMeasurementValue,
  improvementDirections,
  kindLabel,
  measurementKinds,
  unitsByKind,
} from "@/features/measurements/measurementFormatting";
import { measurementErrorCodes } from "@/features/measurements/measurementErrorCodes";
import { ImprovementDirection, MeasurementKind } from "@/features/measurements/types";
import { useCreateMeasurement } from "@/features/measurements/useMeasurements";
import { SettingsFormScreenLayout } from "@/features/settings/components/SettingsFormScreenLayout";
import { SubmissionFailure, toSubmissionFailure } from "@/features/settings/submissionFailure";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Button } from "@/ui/Button";
import { Chip } from "@/ui/Chip";
import { RequiredFieldsLegend, withRequiredMark } from "@/ui/RequiredFieldsLegend";
import { StatusPill } from "@/ui/StatusPill";
import { TextField } from "@/ui/TextField";
import { TonalBlock } from "@/ui/TonalBlock";
import { ToggleSwitch } from "@/ui/ToggleSwitch";
import { useToast } from "@/ui/ToastProvider";

const nameMaxLength = 60;
const unitMaxLength = 12;
const bodyMeasurementKinds: readonly MeasurementKind[] = ["Weight", "Length"];
const previewSamples: Record<MeasurementKind, readonly [number, number]> = {
  Time: [42.5, 38.1],
  Weight: [50, 55],
  Distance: [200, 300],
  Length: [120, 124],
  Repetitions: [8, 12],
  Number: [6, 7],
};

interface ChoiceOption<TValue extends string> {
  value: TValue;
  label: string;
  isDisabled?: boolean;
}

interface ChoiceRowProps<TValue extends string> {
  label: string;
  options: readonly ChoiceOption<TValue>[];
  selectedValue: TValue | null;
  onChange: (value: TValue) => void;
  hint?: string;
}

function ChoiceRow<TValue extends string>({
  label,
  options,
  selectedValue,
  onChange,
  hint,
}: ChoiceRowProps<TValue>) {
  return (
    <View className="gap-1.5">
      <AppText variant="label" tone="muted">
        {withRequiredMark(label)}
      </AppText>
      <View className="flex-row flex-wrap gap-1.5">
        {options.map((option) => (
          <Chip
            key={option.value}
            label={option.label}
            isSelected={option.value === selectedValue}
            disabled={option.isDisabled}
            onPress={() => onChange(option.value)}
          />
        ))}
      </View>
      {hint ? (
        <AppText variant="caption" tone="subtle">
          {hint}
        </AppText>
      ) : null}
    </View>
  );
}

type Recorder = "team" | "students";

export function NewMeasurementScreen() {
  const router = useRouter();
  const { showToast } = useToast();
  const createMutation = useCreateMeasurement();
  const [name, setName] = useState("");
  const [kind, setKind] = useState<MeasurementKind | null>(null);
  const [unit, setUnit] = useState("");
  const [improvesWhen, setImprovesWhen] = useState<ImprovementDirection | null>(null);
  const [isVisibleToStudents, setIsVisibleToStudents] = useState(true);
  const [recorder, setRecorder] = useState<Recorder>("team");
  const [nameError, setNameError] = useState<string | undefined>(undefined);
  const [submissionFailure, setSubmissionFailure] = useState<SubmissionFailure | null>(null);

  const chooseKind = (chosenKind: MeasurementKind) => {
    setKind(chosenKind);
    setUnit(unitsByKind[chosenKind][0] ?? "");
    const isBodyMeasurement = bodyMeasurementKinds.includes(chosenKind);
    setIsVisibleToStudents(!isBodyMeasurement);
    if (isBodyMeasurement) {
      setRecorder("team");
    }
  };
  const changeVisibility = (isVisible: boolean) => {
    setIsVisibleToStudents(isVisible);
    if (!isVisible) {
      setRecorder("team");
    }
  };

  const isReady = areFilled([name, kind, unit, improvesWhen]);

  const save = async () => {
    if (kind === null || improvesWhen === null) {
      return;
    }
    setSubmissionFailure(null);
    setNameError(undefined);
    try {
      await createMutation.mutateAsync({
        name: name.trim(),
        kind,
        unit: unit.trim(),
        improvesWhen,
        isVisibleToStudents,
        isRecordedByStudents: recorder === "students",
      });
      showToast(translate("measurements.created"));
      router.back();
    } catch (saveError) {
      if (isApiError(saveError) && saveError.hasCode(measurementErrorCodes.duplicateName)) {
        setNameError(translate("measurements.duplicateName"));
        return;
      }
      setSubmissionFailure(toSubmissionFailure(saveError));
    }
  };

  const preview =
    kind !== null && improvesWhen !== null && unit.trim().length > 0
      ? previewOf(kind, unit.trim(), improvesWhen)
      : null;

  return (
    <SettingsFormScreenLayout
      title={translate("measurements.new")}
      submissionFailure={submissionFailure}
      onRetry={save}
    >
      <RequiredFieldsLegend />
      <TextField
        label={translate("measurements.form.name")}
        isRequired
        placeholder={translate("measurements.form.namePlaceholder")}
        value={name}
        onChangeText={setName}
        maxLength={nameMaxLength}
        errorMessage={nameError}
      />
      <ChoiceRow
        label={translate("measurements.form.kind")}
        options={measurementKinds.map((value) => ({ value, label: kindLabel(value) }))}
        selectedValue={kind}
        onChange={chooseKind}
      />
      {kind === "Number" ? (
        <TextField
          label={translate("measurements.form.unit")}
          isRequired
          placeholder={translate("measurements.form.customUnitPlaceholder")}
          value={unit}
          onChangeText={setUnit}
          maxLength={unitMaxLength}
        />
      ) : null}
      {kind !== null && unitsByKind[kind].length > 1 ? (
        <ChoiceRow
          label={translate("measurements.form.unit")}
          options={unitsByKind[kind].map((value) => ({ value, label: value }))}
          selectedValue={unit}
          onChange={setUnit}
        />
      ) : null}
      <ChoiceRow
        label={translate("measurements.form.improvesWhen")}
        options={improvementDirections.map((value) => ({ value, label: directionLabel(value) }))}
        selectedValue={improvesWhen}
        onChange={setImprovesWhen}
        hint={translate("measurements.form.improvesWhenHint")}
      />
      <ToggleSwitch
        label={translate("measurements.visibleToStudents")}
        hint={translate("measurements.visibleToStudentsHint")}
        value={isVisibleToStudents}
        onValueChange={changeVisibility}
      />
      <ChoiceRow<Recorder>
        label={translate("measurements.form.recorder")}
        options={[
          { value: "team", label: translate("measurements.form.recorderTeam") },
          {
            value: "students",
            label: translate("measurements.form.recorderStudents"),
            isDisabled: !isVisibleToStudents,
          },
        ]}
        selectedValue={recorder}
        onChange={setRecorder}
        hint={
          isVisibleToStudents ? undefined : translate("measurements.form.recorderNeedsVisibility")
        }
      />
      {preview === null ? null : (
        <View className="gap-1.5">
          <AppText variant="label" tone="muted">
            {translate("measurements.form.previewTitle")}
          </AppText>
          <TonalBlock tone="muted" isCompact>
            <View className="flex-row flex-wrap items-center gap-2">
              <AppText variant="bodyStrong">
                {name.trim() || translate("measurements.form.name")}
              </AppText>
              <AppText variant="bodyStrong">{preview.range}</AppText>
              <StatusPill tone={preview.tone} isSmall label={preview.change} />
            </View>
          </TonalBlock>
        </View>
      )}
      <Button
        label={translate("measurements.form.create")}
        disabled={!isReady}
        isLoading={createMutation.isPending}
        onPress={save}
      />
    </SettingsFormScreenLayout>
  );
}

function previewOf(kind: MeasurementKind, unit: string, improvesWhen: ImprovementDirection) {
  const [lower, higher] = [...previewSamples[kind]].sort((first, second) => first - second);
  const [from, to] = improvesWhen === "Decreases" ? [higher, lower] : [lower, higher];
  const measurement = { kind, unit };
  return {
    range: translate("measurements.form.previewRange", {
      from: formatMeasurementValue(measurement, from),
      to: formatMeasurementValue(measurement, to),
    }),
    change: formatMeasurementChange(measurement, to - from),
    tone: improvesWhen === "Neither" ? ("neutral" as const) : ("success" as const),
  };
}
