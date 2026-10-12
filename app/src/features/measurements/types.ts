export type MeasurementKind = "Time" | "Weight" | "Distance" | "Length" | "Repetitions" | "Number";

export type ImprovementDirection = "Increases" | "Decreases" | "Neither";

export interface Measurement {
  id: string;
  name: string;
  kind: MeasurementKind;
  unit: string;
  improvesWhen: ImprovementDirection;
  isVisibleToStudents: boolean;
  isRecordedByStudents: boolean;
}

export interface MeasurementRecord {
  id: string;
  studentId: string;
  measurementId: string;
  measuredOn: string;
  value: number;
  note: string | null;
  recordedByStudent: boolean;
  recordedByFullName: string | null;
}

export interface StudentMeasurement {
  measurement: Measurement;
  goal: number | null;
  records: MeasurementRecord[];
}

export interface CreateMeasurementRequest {
  name: string;
  kind: MeasurementKind;
  unit: string;
  improvesWhen: ImprovementDirection;
  isVisibleToStudents: boolean;
  isRecordedByStudents: boolean;
}

export interface UpdateMeasurementRequest {
  name: string;
  improvesWhen: ImprovementDirection;
  isVisibleToStudents: boolean;
  isRecordedByStudents: boolean;
}

export interface RecordMeasurementRequest {
  measuredOn: string;
  value: number;
  note: string | null;
}

export interface ClassMeasurementEntry {
  studentId: string;
  value: number;
}

export interface StudentAppStudentMeasurements {
  studentId: string;
  fullName: string;
  measurements: StudentMeasurement[];
}

export interface StudentAppMeasurements {
  students: StudentAppStudentMeasurements[];
}
