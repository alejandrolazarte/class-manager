import { View } from "react-native";
import {
  formatMeasurementChange,
  formatMeasurementDate,
  formatMeasurementValue,
  measurementTrend,
  MeasurementTone,
} from "@/features/measurements/measurementFormatting";
import { StudentMeasurement } from "@/features/measurements/types";
import { translate, translateCount } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { HairlineRow } from "@/ui/HairlineRow";
import { Icon } from "@/ui/Icon";
import { StatusPill, StatusTone } from "@/ui/StatusPill";
import { TrendBars } from "@/ui/TrendBars";

const pillTones: Record<MeasurementTone, StatusTone> = {
  good: "success",
  bad: "danger",
  neutral: "neutral",
};

interface StudentMeasurementRowProps {
  studentMeasurement: StudentMeasurement;
  onPress: () => void;
}

export function StudentMeasurementRow({ studentMeasurement, onPress }: StudentMeasurementRowProps) {
  const { measurement, records } = studentMeasurement;
  const trend = measurementTrend(measurement, records);
  const lastRecord = trend?.last;
  const details =
    lastRecord === undefined
      ? [translate("measurements.student.noRecords")]
      : [
          translateCount("measurements.student.records", records.length),
          translate("measurements.student.lastOn", {
            date: formatMeasurementDate(lastRecord.measuredOn),
          }),
          ...(lastRecord.recordedByStudent
            ? [translate("measurements.detail.byStudent")]
            : lastRecord.recordedByFullName
              ? [
                  translate("measurements.student.recordedBy", {
                    name: lastRecord.recordedByFullName,
                  }),
                ]
              : []),
        ];
  return (
    <HairlineRow onPress={onPress} accessibilityLabel={measurement.name}>
      <View className="min-w-0 flex-1 gap-1">
        <View className="flex-row flex-wrap items-center gap-1.5">
          <AppText variant="bodyStrong">{measurement.name}</AppText>
          {measurement.isVisibleToStudents ? null : (
            <StatusPill tone="neutral" isSmall label={translate("measurements.teamOnly")} />
          )}
        </View>
        <AppText variant="caption" tone="subtle">
          {details.join(" · ")}
        </AppText>
        {trend !== null && records.length > 1 ? (
          <StatusPill
            tone={pillTones[trend.tone]}
            isSmall
            label={formatMeasurementChange(measurement, trend.change)}
          />
        ) : null}
      </View>
      {records.length > 1 ? <TrendBars values={records.map((record) => record.value)} /> : null}
      <AppText variant="bodyStrong" tone={lastRecord === undefined ? "subtle" : "default"}>
        {lastRecord === undefined ? "—" : formatMeasurementValue(measurement, lastRecord.value)}
      </AppText>
      <Icon name="next" size="medium" tone="subtle-foreground" />
    </HairlineRow>
  );
}
