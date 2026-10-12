import { useRouter } from "expo-router";
import { useState } from "react";
import { View } from "react-native";
import { useClient } from "@/features/clients/useClient";
import {
  formatMeasurementChange,
  formatMeasurementDate,
  formatMeasurementValue,
  measurementSummary,
  measurementTrend,
  MeasurementTone,
  monthOfMeasurement,
  parseMeasurementNumber,
} from "@/features/measurements/measurementFormatting";
import { MeasurementRecord, StudentMeasurement } from "@/features/measurements/types";
import {
  useDeleteMeasurementRecord,
  useRestoreMeasurementRecord,
  useSetMeasurementGoal,
  useStudentMeasurements,
} from "@/features/measurements/useMeasurements";
import { useCan } from "@/features/members/CurrentMemberProvider";
import { permissions } from "@/features/members/permissions";
import { translate } from "@/i18n/translate";
import { clientTabs, routes } from "@/navigation/routes";
import { useCurrentTab } from "@/navigation/useCurrentTab";
import { AppText } from "@/ui/AppText";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { EmptyState } from "@/ui/EmptyState";
import { FloatingActionButton } from "@/ui/FloatingActionButton";
import { HairlineRow } from "@/ui/HairlineRow";
import { IconButton } from "@/ui/IconButton";
import { ScrollScreen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";
import { SectionTitle } from "@/ui/SectionTitle";
import { SkeletonDetail } from "@/ui/motion/SkeletonDetail";
import { StatusPill, StatusTone } from "@/ui/StatusPill";
import { TextField } from "@/ui/TextField";
import { TrendBars } from "@/ui/TrendBars";
import { useToast } from "@/ui/ToastProvider";

const pillTones: Record<MeasurementTone, StatusTone> = {
  good: "success",
  bad: "danger",
  neutral: "neutral",
};

interface StudentMeasurementScreenProps {
  clientId: string;
  studentId: string;
  measurementId: string;
}

function recorderOf(record: MeasurementRecord, studentFullName: string): string | null {
  return record.recordedByStudent ? studentFullName : record.recordedByFullName;
}

interface GoalRowProps {
  studentId: string;
  studentMeasurement: StudentMeasurement;
  canChange: boolean;
}

function GoalRow({ studentId, studentMeasurement, canChange }: GoalRowProps) {
  const { showToast } = useToast();
  const setGoalMutation = useSetMeasurementGoal();
  const { measurement, goal } = studentMeasurement;
  const [draft, setDraft] = useState<string | null>(null);
  const parsedDraft = draft === null ? null : parseMeasurementNumber(draft);

  const save = async (value: number | null) => {
    try {
      await setGoalMutation.mutateAsync({ studentId, measurementId: measurement.id, value });
      setDraft(null);
      showToast(translate("measurements.detail.goalSaved"));
    } catch {
      showToast(translate("common.unexpectedError"));
    }
  };

  if (draft !== null) {
    return (
      <View className="gap-2 border-y border-border-subtle py-3">
        <TextField
          label={translate("measurements.detail.goalValue", { unit: measurement.unit })}
          keyboardType="decimal-pad"
          value={draft}
          onChangeText={setDraft}
        />
        <View className="flex-row justify-end gap-2">
          <Button
            variant="ghost"
            size="medium"
            label={translate("common.cancel")}
            onPress={() => setDraft(null)}
          />
          <Button
            size="medium"
            label={translate("common.save")}
            disabled={draft.trim().length > 0 && parsedDraft === null}
            isLoading={setGoalMutation.isPending}
            onPress={() => save(draft.trim().length === 0 ? null : parsedDraft)}
          />
        </View>
      </View>
    );
  }
  return (
    <View className="flex-row items-center justify-between border-y border-border-subtle py-2.5">
      <View>
        <AppText variant="label" tone="muted">
          {translate("measurements.detail.goal")}
        </AppText>
        <AppText variant="body">
          {goal === null
            ? translate("measurements.detail.noGoal")
            : formatMeasurementValue(measurement, goal)}
        </AppText>
      </View>
      {canChange ? (
        <Button
          variant="ghost"
          size="medium"
          label={translate(
            goal === null ? "measurements.detail.setGoal" : "measurements.detail.changeGoal",
          )}
          onPress={() => setDraft(goal === null ? "" : String(goal).replace(".", ","))}
        />
      ) : null}
    </View>
  );
}

export function StudentMeasurementScreen({
  clientId,
  studentId,
  measurementId,
}: StudentMeasurementScreenProps) {
  const router = useRouter();
  const clientTab = useCurrentTab(clientTabs);
  const { showToast } = useToast();
  const canRecord = useCan(permissions.attendanceRecordAll, permissions.attendanceRecordOwn);
  const { data: client } = useClient(clientId);
  const { data: measurements, isPending, isError, refetch } = useStudentMeasurements(studentId);
  const deleteMutation = useDeleteMeasurementRecord();
  const restoreMutation = useRestoreMeasurementRecord();
  const studentFullName =
    client?.students.find((student) => student.id === studentId)?.fullName ?? "";
  const studentMeasurement = measurements?.find(
    (candidate) => candidate.measurement.id === measurementId,
  );

  if (isPending) {
    return (
      <ScrollScreen header={<ScreenHeader navigation="back" title="" />}>
        <SkeletonDetail />
      </ScrollScreen>
    );
  }
  if (isError || studentMeasurement === undefined) {
    return (
      <ScrollScreen header={<ScreenHeader navigation="back" title="" />}>
        <Banner message={translate("common.unexpectedError")}>
          <Button
            variant="secondary"
            size="medium"
            label={translate("common.retry")}
            onPress={() => refetch()}
          />
        </Banner>
      </ScrollScreen>
    );
  }

  const { measurement, records } = studentMeasurement;
  const trend = measurementTrend(measurement, records);
  const newestFirst = [...records].reverse();

  const deleteRecord = async (record: MeasurementRecord) => {
    try {
      await deleteMutation.mutateAsync(record.id);
      showToast(translate("measurements.detail.recordDeleted"), {
        label: translate("common.undo"),
        onPress: () => {
          restoreMutation
            .mutateAsync(record.id)
            .catch(() => showToast(translate("common.undoFailed")));
        },
      });
    } catch {
      showToast(translate("common.unexpectedError"));
    }
  };

  return (
    <ScrollScreen
      header={
        <ScreenHeader
          navigation="back"
          eyebrow={studentFullName || undefined}
          title={measurement.name}
          subtitle={measurementSummary(measurement)}
        />
      }
      hasFloatingAction={canRecord}
      overlay={
        canRecord ? (
          <FloatingActionButton
            label={translate("measurements.detail.recordAction")}
            onPress={() =>
              router.push(routes.recordMeasurement(clientTab, clientId, studentId, measurementId))
            }
          />
        ) : undefined
      }
    >
      {trend === null ? (
        <EmptyState
          icon="trendingUp"
          message={translate("measurements.detail.empty")}
          createActionLabel={canRecord ? translate("measurements.detail.recordAction") : undefined}
        />
      ) : (
        <>
          <View className="flex-row items-end justify-between gap-3">
            <View>
              <AppText variant="display">
                {formatMeasurementValue(measurement, trend.last.value)}
              </AppText>
              <AppText variant="caption" tone="subtle">
                {formatMeasurementDate(trend.last.measuredOn)}
              </AppText>
            </View>
            <View className="items-end gap-1">
              {trend.isPersonalBest ? (
                <StatusPill
                  tone="primary"
                  isSmall
                  label={translate("measurements.detail.personalBest")}
                />
              ) : null}
              {records.length > 1 ? (
                <StatusPill
                  tone={pillTones[trend.tone]}
                  isSmall
                  label={translate("measurements.detail.since", {
                    change: formatMeasurementChange(measurement, trend.change),
                    month: monthOfMeasurement(trend.first.measuredOn),
                  })}
                />
              ) : null}
            </View>
          </View>
          {records.length > 1 ? (
            <View className="gap-1">
              <TrendBars
                size="full"
                values={records.map((record) => record.value)}
                accessibilityLabel={translate("measurements.detail.trend", {
                  name: measurement.name,
                })}
              />
              <View className="flex-row justify-between">
                <AppText variant="footnote" tone="subtle">
                  {formatMeasurementDate(trend.first.measuredOn)}
                </AppText>
                <AppText variant="footnote" tone="subtle">
                  {formatMeasurementDate(trend.last.measuredOn)}
                </AppText>
              </View>
            </View>
          ) : null}
        </>
      )}
      <GoalRow
        studentId={studentId}
        studentMeasurement={studentMeasurement}
        canChange={canRecord}
      />
      {newestFirst.length > 0 ? (
        <View>
          <SectionTitle title={translate("measurements.detail.history")} isOverline />
          {newestFirst.map((record) => {
            const recorder = recorderOf(record, studentFullName);
            const details = [
              formatMeasurementDate(record.measuredOn),
              ...(recorder ? [recorder] : []),
              ...(record.note ? [record.note] : []),
            ];
            return (
              <HairlineRow key={record.id}>
                <View className="min-w-0 flex-1">
                  <AppText variant="bodyStrong">
                    {formatMeasurementValue(measurement, record.value)}
                  </AppText>
                  <AppText variant="caption" tone="subtle">
                    {details.join(" · ")}
                  </AppText>
                </View>
                {canRecord ? (
                  <IconButton
                    icon="delete"
                    tone="danger"
                    accessibilityLabel={translate("measurements.detail.deleteRecord", {
                      date: formatMeasurementDate(record.measuredOn),
                    })}
                    onPress={() => deleteRecord(record)}
                  />
                ) : null}
              </HairlineRow>
            );
          })}
        </View>
      ) : null}
    </ScrollScreen>
  );
}
