import { useRouter } from "expo-router";
import { useState } from "react";
import { View } from "react-native";
import { formatDuration } from "@/forms/duration";
import { DurationPickerSheet } from "@/forms/DurationPickerSheet";
import {
  formatMeasurementValue,
  measurementTrend,
  parseMeasurementNumber,
} from "@/features/measurements/measurementFormatting";
import { Measurement, MeasurementRecord } from "@/features/measurements/types";
import {
  useClassMeasurementRecords,
  useMeasurementDefinitions,
  useRecordClassMeasurements,
  useStudentMeasurements,
} from "@/features/measurements/useMeasurements";
import { formatLongDate } from "@/features/sessions/dates";
import { SessionStudent } from "@/features/sessions/types";
import { useSessionDetails } from "@/features/sessions/useSessionDetails";
import { SettingsFormScreenLayout } from "@/features/settings/components/SettingsFormScreenLayout";
import { SettingsItemState } from "@/features/settings/components/SettingsItemState";
import { SubmissionFailure, toSubmissionFailure } from "@/features/settings/submissionFailure";
import { translate, translateCount } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Avatar } from "@/ui/Avatar";
import { Button } from "@/ui/Button";
import { Chip } from "@/ui/Chip";
import { EmptyState } from "@/ui/EmptyState";
import { HairlineRow } from "@/ui/HairlineRow";
import { SectionTitle } from "@/ui/SectionTitle";
import { SelectField } from "@/ui/SelectField";
import { TextField } from "@/ui/TextField";
import { useToast } from "@/ui/ToastProvider";

const decimalComma = ",";
const decimalPoint = ".";

function toTypedValue(measurement: Measurement, value: number): string {
  return measurement.kind === "Time"
    ? String(value)
    : String(value).replace(decimalPoint, decimalComma);
}

function savedValueOf(
  savedRecords: readonly MeasurementRecord[],
  measurementId: string,
  studentId: string,
): number | undefined {
  return savedRecords.find(
    (record) => record.measurementId === measurementId && record.studentId === studentId,
  )?.value;
}

interface ClassMeasurementRowProps {
  student: SessionStudent;
  measurement: Measurement;
  sessionDate: string;
  typedValue: string;
  onChange: (typedValue: string) => void;
}

function ClassMeasurementRow({
  student,
  measurement,
  sessionDate,
  typedValue,
  onChange,
}: ClassMeasurementRowProps) {
  const { data: studentMeasurements = [] } = useStudentMeasurements(student.studentId);
  const records = (
    studentMeasurements.find((item) => item.measurement.id === measurement.id)?.records ?? []
  ).filter((record) => record.measuredOn < sessionDate);
  const lastRecord = measurementTrend(measurement, records)?.last;
  const [isPickerOpen, setIsPickerOpen] = useState(false);
  const valueLabel = translate("measurements.class.valueOf", {
    measurement: measurement.name,
    name: student.studentFullName,
  });
  const timeInSeconds = parseMeasurementNumber(typedValue);
  return (
    <HairlineRow>
      <Avatar name={student.studentFullName} />
      <View className="min-w-0 flex-1">
        <AppText variant="bodyStrong">{student.studentFullName}</AppText>
        <AppText variant="caption" tone="subtle">
          {lastRecord === undefined
            ? translate("measurements.class.firstTime")
            : translate("measurements.class.previous", {
                value: formatMeasurementValue(measurement, lastRecord.value),
              })}
        </AppText>
      </View>
      <View className="w-32">
        {measurement.kind === "Time" ? (
          <SelectField
            label={measurement.unit}
            icon="pickTime"
            accessibilityLabel={valueLabel}
            accessibilityHint={translate("durationField.openPicker")}
            value={timeInSeconds === null ? null : formatDuration(timeInSeconds)}
            placeholder={translate("durationField.placeholder")}
            onPress={() => setIsPickerOpen(true)}
          />
        ) : (
          <TextField
            label={measurement.unit}
            accessibilityLabel={valueLabel}
            keyboardType="numbers-and-punctuation"
            value={typedValue}
            onChangeText={onChange}
          />
        )}
      </View>
      {isPickerOpen ? (
        <DurationPickerSheet
          initialValueInSeconds={timeInSeconds}
          onCancel={() => setIsPickerOpen(false)}
          onDone={(pickedValueInSeconds) => {
            onChange(String(pickedValueInSeconds));
            setIsPickerOpen(false);
          }}
        />
      ) : null}
    </HairlineRow>
  );
}

interface ClassMeasurementsScreenProps {
  classGroupId: string;
  sessionDate: string;
}

export function ClassMeasurementsScreen({
  classGroupId,
  sessionDate,
}: ClassMeasurementsScreenProps) {
  const router = useRouter();
  const { showToast } = useToast();
  const session = useSessionDetails(classGroupId, sessionDate);
  const definitions = useMeasurementDefinitions();
  const classStudentIds = session.data?.students.map((student) => student.studentId) ?? [];
  const savedRecordsQuery = useClassMeasurementRecords(sessionDate, classStudentIds);
  const savedRecords = classStudentIds.length === 0 ? [] : savedRecordsQuery.data;
  const recordMutation = useRecordClassMeasurements();
  const [chosenMeasurementId, setChosenMeasurementId] = useState<string | null>(null);
  const [typedValues, setTypedValues] = useState<Record<string, string>>({});
  const [submissionFailure, setSubmissionFailure] = useState<SubmissionFailure | null>(null);

  if (session.data === undefined || definitions.data === undefined || savedRecords === undefined) {
    return (
      <SettingsItemState
        navigation="close"
        isPending={session.isPending || definitions.isPending || savedRecordsQuery.isPending}
        isError={session.isError || definitions.isError || savedRecordsQuery.isError}
        notFoundMessage={translate("common.unexpectedError")}
        onRetry={() => {
          session.refetch();
          definitions.refetch();
          savedRecordsQuery.refetch();
        }}
      />
    );
  }

  const measurements = definitions.data;
  const measurement =
    measurements.find((candidate) => candidate.id === chosenMeasurementId) ?? measurements[0];
  const savedValueFor = (studentId: string) =>
    measurement === undefined ? undefined : savedValueOf(savedRecords, measurement.id, studentId);
  const students = session.data.students.filter(
    (student) => student.status !== "Absent" || savedValueFor(student.studentId) !== undefined,
  );
  const shownValueOf = (studentId: string) => {
    const savedValue = savedValueFor(studentId);
    return (
      typedValues[studentId] ??
      (measurement === undefined || savedValue === undefined
        ? ""
        : toTypedValue(measurement, savedValue))
    );
  };
  const entries =
    measurement === undefined
      ? []
      : students.flatMap((student) => {
          const typedValue = typedValues[student.studentId];
          const value = typedValue === undefined ? null : parseMeasurementNumber(typedValue);
          return value === null || value === savedValueFor(student.studentId)
            ? []
            : [{ studentId: student.studentId, value }];
        });

  const save = async () => {
    if (measurement === undefined || entries.length === 0) {
      return;
    }
    setSubmissionFailure(null);
    try {
      await recordMutation.mutateAsync({
        measurementId: measurement.id,
        measuredOn: sessionDate,
        entries,
      });
      showToast(translate("measurements.class.saved"));
      router.back();
    } catch (saveError) {
      setSubmissionFailure(toSubmissionFailure(saveError));
    }
  };

  return (
    <SettingsFormScreenLayout
      title={translate("measurements.class.title")}
      eyebrow={`${session.data.classGroupName} · ${formatLongDate(sessionDate)}`}
      submissionFailure={submissionFailure}
      onRetry={save}
    >
      {measurement === undefined ? (
        <EmptyState icon="trendingUp" message={translate("measurements.class.noMeasurements")} />
      ) : (
        <>
          <View className="flex-row flex-wrap gap-1.5">
            {measurements.map((candidate) => (
              <Chip
                key={candidate.id}
                label={candidate.name}
                count={
                  savedRecords.filter((record) => record.measurementId === candidate.id).length ||
                  undefined
                }
                isSelected={candidate.id === measurement.id}
                onPress={() => {
                  setChosenMeasurementId(candidate.id);
                  setTypedValues({});
                }}
              />
            ))}
          </View>
          <View>
            <SectionTitle
              title={`${translate("measurements.class.present")} · ${students.length}`}
              isOverline
            />
            {students.length === 0 ? (
              <AppText variant="body" tone="muted" className="py-2">
                {translate("measurements.class.noStudents")}
              </AppText>
            ) : null}
            {students.map((student) => (
              <ClassMeasurementRow
                key={student.studentId}
                student={student}
                measurement={measurement}
                sessionDate={sessionDate}
                typedValue={shownValueOf(student.studentId)}
                onChange={(typedValue) =>
                  setTypedValues((current) => ({ ...current, [student.studentId]: typedValue }))
                }
              />
            ))}
          </View>
          <AppText variant="caption" tone="subtle">
            {translate("measurements.class.hint")}
          </AppText>
          <Button
            label={
              entries.length === 0
                ? translate("common.save")
                : translateCount("measurements.class.save", entries.length)
            }
            disabled={entries.length === 0}
            isLoading={recordMutation.isPending}
            onPress={save}
          />
        </>
      )}
    </SettingsFormScreenLayout>
  );
}
