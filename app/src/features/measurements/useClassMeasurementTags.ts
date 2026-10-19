import { formatMeasurementValue } from "@/features/measurements/measurementFormatting";
import {
  useClassMeasurementRecords,
  useMeasurementDefinitions,
} from "@/features/measurements/useMeasurements";

export interface ClassMeasurementTag {
  measurementId: string;
  label: string;
  value: string;
}

const noTags: ClassMeasurementTag[] = [];

export function useClassMeasurementTags(sessionDate: string, studentIds: readonly string[]) {
  const { data: measurements = [] } = useMeasurementDefinitions();
  const { data: savedRecords = [] } = useClassMeasurementRecords(sessionDate, studentIds);
  const tagsByStudent = new Map<string, ClassMeasurementTag[]>();
  measurements.forEach((measurement) =>
    savedRecords
      .filter((record) => record.measurementId === measurement.id)
      .forEach((record) =>
        tagsByStudent.set(record.studentId, [
          ...(tagsByStudent.get(record.studentId) ?? []),
          {
            measurementId: measurement.id,
            label: measurement.name,
            value: formatMeasurementValue(measurement, record.value),
          },
        ]),
      ),
  );
  return {
    tagsOf: (studentId: string) => tagsByStudent.get(studentId) ?? noTags,
    hasTags: tagsByStudent.size > 0,
  };
}
