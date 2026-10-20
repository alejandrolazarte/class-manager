import { ReactNode, useState } from "react";
import { View } from "react-native";
import { DateField } from "@/forms/DateField";
import { DurationFields } from "@/forms/DurationFields";
import { isFilled } from "@/forms/requiredFields";
import {
  formatMeasurementDate,
  formatMeasurementValue,
  kindLabel,
  measurementTrend,
  parseMeasurementNumber,
} from "@/features/measurements/measurementFormatting";
import {
  Measurement,
  RecordMeasurementRequest,
  StudentMeasurement,
} from "@/features/measurements/types";
import { todayIsoDate } from "@/dates/dates";
import { StudentChips } from "@/features/studentApp/components/StudentChips";
import {
  formatBirthDateForDisplay,
  isValidBirthDate,
  parseBirthDate,
} from "@/dates/birthDateFormatting";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Button } from "@/ui/Button";
import { Chip } from "@/ui/Chip";
import { RequiredFieldsLegend, withRequiredMark } from "@/ui/RequiredFieldsLegend";
import { TextField } from "@/ui/TextField";

const noteMaxLength = 200;

export interface MeasurementRecordStudent {
  id: string;
  fullName: string;
}

interface MeasurementRecordFormProps {
  students: readonly MeasurementRecordStudent[];
  initialStudentId: string;
  measurements: readonly Measurement[];
  initialMeasurementId?: string;
  studentMeasurementsOf: (studentId: string) => readonly StudentMeasurement[];
  onStudentChange?: (studentId: string) => void;
  isSaving: boolean;
  notice?: ReactNode;
  measurementHint?: string;
  onSubmit: (studentId: string, measurementId: string, request: RecordMeasurementRequest) => void;
}

function valueOf(
  measurement: Measurement | undefined,
  timeInSeconds: number | null,
  amount: string,
): number | null {
  if (measurement === undefined) {
    return null;
  }
  return measurement.kind === "Time" ? timeInSeconds : parseMeasurementNumber(amount);
}

export function MeasurementRecordForm({
  students,
  initialStudentId,
  measurements,
  initialMeasurementId,
  studentMeasurementsOf,
  onStudentChange,
  isSaving,
  notice,
  measurementHint,
  onSubmit,
}: MeasurementRecordFormProps) {
  const [studentId, setStudentId] = useState(initialStudentId);
  const [measurementId, setMeasurementId] = useState(
    initialMeasurementId ?? measurements[0]?.id ?? "",
  );
  const [typedDate, setTypedDate] = useState(formatBirthDateForDisplay(todayIsoDate()));
  const [timeInSeconds, setTimeInSeconds] = useState<number | null>(null);
  const [amount, setAmount] = useState("");
  const [note, setNote] = useState("");
  const [dateError, setDateError] = useState<string | undefined>(undefined);

  const measurement = measurements.find((candidate) => candidate.id === measurementId);
  const value = valueOf(measurement, timeInSeconds, amount);
  const isTime = measurement?.kind === "Time";
  const isReady = measurement !== undefined && value !== null && isFilled(typedDate);
  const lastRecord = measurementTrend(
    { improvesWhen: measurement?.improvesWhen ?? "Neither" },
    studentMeasurementsOf(studentId).find((item) => item.measurement.id === measurementId)
      ?.records ?? [],
  )?.last;

  const chooseStudent = (chosenStudentId: string) => {
    setStudentId(chosenStudentId);
    onStudentChange?.(chosenStudentId);
  };
  const chooseMeasurement = (chosenMeasurementId: string) => {
    setMeasurementId(chosenMeasurementId);
    setTimeInSeconds(null);
    setAmount("");
  };

  const submit = () => {
    const measuredOn = parseBirthDate(typedDate);
    if (measuredOn === null || !isValidBirthDate(typedDate)) {
      setDateError(translate("measurements.recordForm.invalidDate"));
      return;
    }
    if (measurement === undefined || value === null) {
      return;
    }
    setDateError(undefined);
    onSubmit(studentId, measurement.id, {
      measuredOn,
      value,
      note: note.trim().length === 0 ? null : note.trim(),
    });
  };

  const lastHint =
    lastRecord && measurement
      ? translate("measurements.recordForm.last", {
          value: formatMeasurementValue(measurement, lastRecord.value),
          date: formatMeasurementDate(lastRecord.measuredOn),
        })
      : undefined;

  return (
    <>
      <RequiredFieldsLegend />
      {students.length > 1 ? (
        <View className="gap-1.5">
          <AppText variant="label" tone="muted">
            {withRequiredMark(translate("measurements.recordForm.student"))}
          </AppText>
          <StudentChips
            students={students}
            selectedStudentId={studentId}
            onSelect={chooseStudent}
          />
        </View>
      ) : null}
      <View className="gap-1.5">
        <AppText variant="label" tone="muted">
          {withRequiredMark(translate("measurements.recordForm.measurement"))}
        </AppText>
        <View className="flex-row flex-wrap gap-1.5">
          {measurements.map((candidate) => (
            <Chip
              key={candidate.id}
              label={candidate.name}
              isSelected={candidate.id === measurementId}
              onPress={() => chooseMeasurement(candidate.id)}
            />
          ))}
        </View>
        {measurementHint ? (
          <AppText variant="caption" tone="subtle">
            {measurementHint}
          </AppText>
        ) : null}
      </View>
      <DateField
        label={translate("measurements.recordForm.date")}
        isRequired
        value={typedDate}
        onChangeText={setTypedDate}
        errorMessage={dateError}
        latestIsoDate={todayIsoDate()}
      />
      {measurement === undefined ? null : isTime ? (
        <View className="gap-1.5">
          <DurationFields
            minutesLabel={translate("measurements.recordForm.minutes")}
            secondsLabel={translate("measurements.recordForm.seconds")}
            isRequired
            valueInSeconds={timeInSeconds}
            onChange={setTimeInSeconds}
          />
          {lastHint ? (
            <AppText variant="caption" tone="subtle">
              {lastHint}
            </AppText>
          ) : null}
        </View>
      ) : (
        <TextField
          label={translate("measurements.recordForm.value", { unit: measurement.unit })}
          accessibilityHint={kindLabel(measurement.kind)}
          isRequired
          keyboardType="numbers-and-punctuation"
          value={amount}
          onChangeText={setAmount}
          hint={lastHint}
        />
      )}
      <TextField
        label={translate("measurements.recordForm.note")}
        placeholder={translate("measurements.recordForm.notePlaceholder")}
        value={note}
        onChangeText={setNote}
        maxLength={noteMaxLength}
      />
      {notice}
      <Button
        label={translate("common.save")}
        disabled={!isReady}
        isLoading={isSaving}
        onPress={submit}
      />
    </>
  );
}
