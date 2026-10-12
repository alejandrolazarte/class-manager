import { View } from "react-native";
import {
  formatMeasurementDate,
  formatMeasurementValue,
  goalProgress,
  measurementTrend,
  studentChangeText,
} from "@/features/measurements/measurementFormatting";
import { useStudentAppMeasurements } from "@/features/measurements/useMeasurements";
import { firstNameOf } from "@/features/studentApp/studentAppSchedule";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { EmptyState } from "@/ui/EmptyState";
import { HairlineRow } from "@/ui/HairlineRow";
import { ProgressBar } from "@/ui/ProgressBar";
import { ScrollScreen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";
import { SectionTitle } from "@/ui/SectionTitle";
import { SkeletonDetail } from "@/ui/motion/SkeletonDetail";
import { TrendBars } from "@/ui/TrendBars";

interface StudentAppMeasurementScreenProps {
  studentId: string;
  measurementId: string;
}

export function StudentAppMeasurementScreen({
  studentId,
  measurementId,
}: StudentAppMeasurementScreenProps) {
  const { data, isPending, isError, refetch } = useStudentAppMeasurements();
  const student = data?.students.find((candidate) => candidate.studentId === studentId);
  const studentMeasurement = student?.measurements.find(
    (candidate) => candidate.measurement.id === measurementId,
  );

  if (isPending) {
    return (
      <ScrollScreen header={<ScreenHeader navigation="back" title="" />}>
        <SkeletonDetail />
      </ScrollScreen>
    );
  }
  if (isError || student === undefined || studentMeasurement === undefined) {
    return (
      <ScrollScreen header={<ScreenHeader navigation="back" title="" />}>
        <Banner message={translate("student.loadError")}>
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

  const { measurement, records, goal } = studentMeasurement;
  const trend = measurementTrend(measurement, records);
  const change = trend === null ? null : studentChangeText(measurement, trend);
  const progress = trend === null || goal === null ? null : goalProgress(measurement, trend, goal);

  return (
    <ScrollScreen
      header={
        <ScreenHeader
          navigation="back"
          eyebrow={firstNameOf(student.fullName)}
          title={measurement.name}
        />
      }
    >
      {trend === null ? (
        <EmptyState icon="trendingUp" message={translate("measurements.detail.empty")} />
      ) : (
        <>
          {trend.isPersonalBest ? (
            <Banner
              tone="success"
              icon="celebration"
              message={translate("student.measurements.newBest")}
            />
          ) : null}
          <View>
            <AppText variant="display">
              {formatMeasurementValue(measurement, trend.last.value)}
            </AppText>
            <AppText variant="body" tone={change === null ? "subtle" : "success"}>
              {change ??
                translate("student.measurements.measuredOn", {
                  date: formatMeasurementDate(trend.last.measuredOn),
                })}
            </AppText>
          </View>
          {progress !== null && goal !== null ? (
            <View className="gap-2 border-y border-border-subtle py-3">
              <View className="flex-row justify-between gap-2">
                <AppText variant="label" tone="muted">
                  {translate("student.measurements.goal", {
                    value: formatMeasurementValue(measurement, goal),
                  })}
                </AppText>
                <AppText variant="label" tone="primarySoft">
                  {progress.isReached
                    ? translate("student.measurements.goalReached")
                    : translate("student.measurements.missing", {
                        value: formatMeasurementValue(measurement, progress.remaining),
                      })}
                </AppText>
              </View>
              <View className="flex-row">
                <ProgressBar ratio={progress.ratio} />
              </View>
            </View>
          ) : null}
          {records.length > 1 ? (
            <TrendBars
              size="full"
              values={records.map((record) => record.value)}
              accessibilityLabel={translate("measurements.detail.trend", {
                name: measurement.name,
              })}
            />
          ) : null}
          <View>
            <SectionTitle title={translate("student.measurements.history")} isOverline />
            {[...records].reverse().map((record) => (
              <HairlineRow key={record.id}>
                <View className="min-w-0 flex-1">
                  <AppText variant="bodyStrong">
                    {formatMeasurementValue(measurement, record.value)}
                  </AppText>
                  <AppText variant="caption" tone="subtle">
                    {record.note
                      ? `${formatMeasurementDate(record.measuredOn)} · “${record.note}”`
                      : formatMeasurementDate(record.measuredOn)}
                  </AppText>
                </View>
              </HairlineRow>
            ))}
          </View>
        </>
      )}
    </ScrollScreen>
  );
}
