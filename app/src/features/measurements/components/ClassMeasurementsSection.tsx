import { useRouter } from "expo-router";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { Button } from "@/ui/Button";

interface ClassMeasurementsSectionProps {
  classGroupId: string;
  sessionDate: string;
  hasRecordedMeasurements: boolean;
}

export function ClassMeasurementsSection({
  classGroupId,
  sessionDate,
  hasRecordedMeasurements,
}: ClassMeasurementsSectionProps) {
  const router = useRouter();
  return (
    <Button
      variant="secondary"
      size="medium"
      icon="trendingUp"
      label={translate(
        hasRecordedMeasurements ? "measurements.class.change" : "measurements.class.open",
      )}
      onPress={() => router.push(routes.classMeasurements(classGroupId, sessionDate))}
    />
  );
}
