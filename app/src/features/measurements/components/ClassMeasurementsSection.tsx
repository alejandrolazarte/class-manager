import { useRouter } from "expo-router";
import { View } from "react-native";
import {
  useClassMeasurementRecords,
  useMeasurementDefinitions,
} from "@/features/measurements/useMeasurements";
import { translate, translateCount } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { AppText } from "@/ui/AppText";
import { Button } from "@/ui/Button";
import { HairlineRow } from "@/ui/HairlineRow";
import { SectionTitle } from "@/ui/SectionTitle";

interface ClassMeasurementsSectionProps {
  classGroupId: string;
  sessionDate: string;
  studentIds: readonly string[];
}

export function ClassMeasurementsSection({
  classGroupId,
  sessionDate,
  studentIds,
}: ClassMeasurementsSectionProps) {
  const router = useRouter();
  const { data: measurements = [] } = useMeasurementDefinitions();
  const { data: savedRecords = [] } = useClassMeasurementRecords(sessionDate, studentIds);
  const recordedMeasurements = measurements.flatMap((measurement) => {
    const recordedCount = savedRecords.filter(
      (record) => record.measurementId === measurement.id,
    ).length;
    return recordedCount === 0 ? [] : [{ measurement, recordedCount }];
  });

  return (
    <View className="gap-1.5">
      {recordedMeasurements.length > 0 ? (
        <View>
          <SectionTitle title={translate("measurements.class.recorded")} isOverline />
          <View className="mt-1.5 border-t border-border-subtle">
            {recordedMeasurements.map(({ measurement, recordedCount }) => (
              <HairlineRow key={measurement.id} className="flex-row items-center gap-3 py-2.5">
                <AppText variant="bodyStrong" className="min-w-0 flex-1">
                  {measurement.name}
                </AppText>
                <AppText variant="caption" tone="subtle">
                  {translateCount("measurements.class.recordedStudents", recordedCount)}
                </AppText>
              </HairlineRow>
            ))}
          </View>
        </View>
      ) : null}
      <Button
        variant="secondary"
        size="medium"
        icon="trendingUp"
        label={translate(
          recordedMeasurements.length === 0
            ? "measurements.class.open"
            : "measurements.class.change",
        )}
        onPress={() => router.push(routes.classMeasurements(classGroupId, sessionDate))}
      />
    </View>
  );
}
