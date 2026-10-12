import { useRouter } from "expo-router";
import { View } from "react-native";
import {
  formatMeasurementDate,
  formatMeasurementValue,
  measurementTrend,
  studentChangeText,
} from "@/features/measurements/measurementFormatting";
import { StudentMeasurement } from "@/features/measurements/types";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { AppText } from "@/ui/AppText";
import { EmptyState } from "@/ui/EmptyState";
import { HairlineRow } from "@/ui/HairlineRow";
import { Icon } from "@/ui/Icon";
import { StatusPill } from "@/ui/StatusPill";
import { TrendBars } from "@/ui/TrendBars";

interface StudentAppMeasurementRowProps {
  studentMeasurement: StudentMeasurement;
  onPress: () => void;
}

function StudentAppMeasurementRow({ studentMeasurement, onPress }: StudentAppMeasurementRowProps) {
  const { measurement, records } = studentMeasurement;
  const trend = measurementTrend(measurement, records);
  const change = trend === null ? null : studentChangeText(measurement, trend);
  return (
    <HairlineRow onPress={onPress} accessibilityLabel={measurement.name}>
      <View className="min-w-0 flex-1 gap-1">
        <View className="flex-row flex-wrap items-center gap-1.5">
          <AppText variant="bodyStrong">{measurement.name}</AppText>
          {measurement.isRecordedByStudents ? (
            <StatusPill
              tone="neutral"
              isSmall
              label={translate("student.measurements.youRecord")}
            />
          ) : null}
          {trend?.isPersonalBest ? (
            <StatusPill
              tone="primary"
              isSmall
              label={translate("measurements.detail.personalBest")}
            />
          ) : null}
        </View>
        {change !== null ? (
          <AppText variant="caption" tone="success">
            {change}
          </AppText>
        ) : trend !== null ? (
          <AppText variant="caption" tone="subtle">
            {translate("student.measurements.measuredOn", {
              date: formatMeasurementDate(trend.last.measuredOn),
            })}
          </AppText>
        ) : null}
      </View>
      {records.length > 1 ? (
        <TrendBars
          isLowerBetter={measurement.improvesWhen === "Decreases"}
          values={records.map((record) => record.value)}
        />
      ) : null}
      <AppText variant="bodyStrong" tone={trend === null ? "subtle" : "default"}>
        {trend === null ? "—" : formatMeasurementValue(measurement, trend.last.value)}
      </AppText>
      <Icon name="next" size="medium" tone="subtle-foreground" />
    </HairlineRow>
  );
}

interface StudentAppMeasurementsSectionProps {
  studentId: string;
  measurements: readonly StudentMeasurement[];
}

export function StudentAppMeasurementsSection({
  studentId,
  measurements,
}: StudentAppMeasurementsSectionProps) {
  const router = useRouter();
  if (measurements.length === 0) {
    return <EmptyState icon="trendingUp" message={translate("student.measurements.empty")} />;
  }
  return (
    <View className="gap-1">
      <AppText variant="caption" tone="subtle">
        {translate("student.measurements.hint")}
      </AppText>
      <View>
        {measurements.map((studentMeasurement) => (
          <StudentAppMeasurementRow
            key={studentMeasurement.measurement.id}
            studentMeasurement={studentMeasurement}
            onPress={() =>
              router.push(
                routes.studentAppMeasurement(studentId, studentMeasurement.measurement.id),
              )
            }
          />
        ))}
      </View>
    </View>
  );
}
