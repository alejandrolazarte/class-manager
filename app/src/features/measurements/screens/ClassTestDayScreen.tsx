import { useRouter } from "expo-router";
import { useState } from "react";
import { View } from "react-native";
import {
  formatMeasurementValue,
  measurementTrend,
  measurementValueFromTyped,
} from "@/features/measurements/measurementFormatting";
import { Measurement } from "@/features/measurements/types";
import {
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
import { TextField } from "@/ui/TextField";
import { useToast } from "@/ui/ToastProvider";

interface TestDayRowProps {
  student: SessionStudent;
  measurement: Measurement;
  typedValue: string;
  onChange: (typedValue: string) => void;
}

function TestDayRow({ student, measurement, typedValue, onChange }: TestDayRowProps) {
  const { data: studentMeasurements = [] } = useStudentMeasurements(student.studentId);
  const records =
    studentMeasurements.find((item) => item.measurement.id === measurement.id)?.records ?? [];
  const lastRecord = measurementTrend(measurement, records)?.last;
  return (
    <HairlineRow>
      <Avatar name={student.studentFullName} />
      <View className="min-w-0 flex-1">
        <AppText variant="bodyStrong">{student.studentFullName}</AppText>
        <AppText variant="caption" tone="subtle">
          {lastRecord === undefined
            ? translate("measurements.testDay.firstTime")
            : translate("measurements.testDay.previous", {
                value: formatMeasurementValue(measurement, lastRecord.value),
              })}
        </AppText>
      </View>
      <View className="w-32">
        <TextField
          label={measurement.unit}
          accessibilityLabel={translate("measurements.testDay.valueOf", {
            measurement: measurement.name,
            name: student.studentFullName,
          })}
          keyboardType="numbers-and-punctuation"
          value={typedValue}
          onChangeText={onChange}
        />
      </View>
    </HairlineRow>
  );
}

interface ClassTestDayScreenProps {
  classGroupId: string;
  sessionDate: string;
}

export function ClassTestDayScreen({ classGroupId, sessionDate }: ClassTestDayScreenProps) {
  const router = useRouter();
  const { showToast } = useToast();
  const session = useSessionDetails(classGroupId, sessionDate);
  const definitions = useMeasurementDefinitions();
  const recordMutation = useRecordClassMeasurements();
  const [chosenMeasurementId, setChosenMeasurementId] = useState<string | null>(null);
  const [typedValues, setTypedValues] = useState<Record<string, string>>({});
  const [submissionFailure, setSubmissionFailure] = useState<SubmissionFailure | null>(null);

  if (session.data === undefined || definitions.data === undefined) {
    return (
      <SettingsItemState
        navigation="close"
        isPending={session.isPending || definitions.isPending}
        isError={session.isError || definitions.isError}
        notFoundMessage={translate("common.unexpectedError")}
        onRetry={() => {
          session.refetch();
          definitions.refetch();
        }}
      />
    );
  }

  const measurements = definitions.data;
  const measurement =
    measurements.find((candidate) => candidate.id === chosenMeasurementId) ?? measurements[0];
  const students = session.data.students.filter((student) => student.status !== "Absent");
  const entries =
    measurement === undefined
      ? []
      : students.flatMap((student) => {
          const value = measurementValueFromTyped(
            measurement.kind,
            typedValues[student.studentId] ?? "",
          );
          return value === null ? [] : [{ studentId: student.studentId, value }];
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
      showToast(translate("measurements.testDay.saved"));
      router.back();
    } catch (saveError) {
      setSubmissionFailure(toSubmissionFailure(saveError));
    }
  };

  return (
    <SettingsFormScreenLayout
      title={translate("measurements.testDay.title")}
      eyebrow={`${session.data.classGroupName} · ${formatLongDate(sessionDate)}`}
      submissionFailure={submissionFailure}
      onRetry={save}
    >
      {measurement === undefined ? (
        <EmptyState icon="trendingUp" message={translate("measurements.testDay.noMeasurements")} />
      ) : (
        <>
          <View className="flex-row flex-wrap gap-1.5">
            {measurements.map((candidate) => (
              <Chip
                key={candidate.id}
                label={candidate.name}
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
              title={`${translate("measurements.testDay.present")} · ${students.length}`}
              isOverline
            />
            {students.length === 0 ? (
              <AppText variant="body" tone="muted" className="py-2">
                {translate("measurements.testDay.noStudents")}
              </AppText>
            ) : null}
            {students.map((student) => (
              <TestDayRow
                key={student.studentId}
                student={student}
                measurement={measurement}
                typedValue={typedValues[student.studentId] ?? ""}
                onChange={(typedValue) =>
                  setTypedValues((current) => ({ ...current, [student.studentId]: typedValue }))
                }
              />
            ))}
          </View>
          <AppText variant="caption" tone="subtle">
            {translate("measurements.testDay.hint")}
          </AppText>
          <Button
            label={
              entries.length === 0
                ? translate("common.save")
                : translateCount("measurements.testDay.save", entries.length)
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
