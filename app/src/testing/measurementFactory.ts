import { Measurement, MeasurementRecord, StudentMeasurement } from "@/features/measurements/types";

export function buildTimeMeasurement(overrides: Partial<Measurement> = {}): Measurement {
  return {
    id: "measurement-crol",
    name: "25 m crol",
    kind: "Time",
    unit: "s",
    improvesWhen: "Decreases",
    isVisibleToStudents: true,
    isRecordedByStudents: false,
    ...overrides,
  };
}

export function buildDistanceMeasurement(overrides: Partial<Measurement> = {}): Measurement {
  return {
    id: "measurement-distance",
    name: "Nado continuo",
    kind: "Distance",
    unit: "m",
    improvesWhen: "Increases",
    isVisibleToStudents: true,
    isRecordedByStudents: true,
    ...overrides,
  };
}

export function buildMeasurementRecord(
  measuredOn: string,
  value: number,
  overrides: Partial<MeasurementRecord> = {},
): MeasurementRecord {
  return {
    id: `record-${measuredOn}`,
    studentId: "student-1",
    measurementId: "measurement-crol",
    measuredOn,
    value,
    note: null,
    recordedByStudent: false,
    recordedByFullName: "Caro Díaz",
    ...overrides,
  };
}

export function buildStudentMeasurement(
  measurement: Measurement,
  records: MeasurementRecord[],
  goal: number | null = null,
): StudentMeasurement {
  return { measurement, goal, records };
}
