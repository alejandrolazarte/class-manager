import { httpClient } from "@/api/httpClient";
import {
  ClassMeasurementEntry,
  CreateMeasurementRequest,
  Measurement,
  MeasurementRecord,
  RecordMeasurementRequest,
  StudentAppMeasurements,
  StudentMeasurement,
  UpdateMeasurementRequest,
} from "@/features/measurements/types";

const measurementsPath = "/api/measurements";
const measurementRecordsPath = "/api/measurement-records";
const studentsPath = "/api/students";
const studentAppPath = "/api/student-app";
const measurementsSegment = "measurements";
const recordsSegment = "records";
const goalSegment = "goal";
const restoreSegment = "restore";
const measuredOnParameter = "measuredOn";
const studentIdsParameter = "studentIds";

function studentMeasurementsPath(studentId: string): string {
  return `${studentsPath}/${encodeURIComponent(studentId)}/${measurementsSegment}`;
}

export function listMeasurements(): Promise<Measurement[]> {
  return httpClient.get<Measurement[]>(measurementsPath);
}

export function createMeasurement(request: CreateMeasurementRequest): Promise<Measurement> {
  return httpClient.post<Measurement>(measurementsPath, request);
}

export function updateMeasurement(
  measurementId: string,
  request: UpdateMeasurementRequest,
): Promise<Measurement> {
  return httpClient.put<Measurement>(
    `${measurementsPath}/${encodeURIComponent(measurementId)}`,
    request,
  );
}

export function deleteMeasurement(measurementId: string): Promise<void> {
  return httpClient.delete<void>(`${measurementsPath}/${encodeURIComponent(measurementId)}`);
}

export function restoreMeasurement(measurementId: string): Promise<void> {
  return httpClient.post<void>(
    `${measurementsPath}/${encodeURIComponent(measurementId)}/${restoreSegment}`,
    {},
  );
}

export function getStudentMeasurements(studentId: string): Promise<StudentMeasurement[]> {
  return httpClient.get<StudentMeasurement[]>(studentMeasurementsPath(studentId));
}

export function recordMeasurement(
  studentId: string,
  measurementId: string,
  request: RecordMeasurementRequest,
): Promise<MeasurementRecord> {
  return httpClient.post<MeasurementRecord>(
    `${studentMeasurementsPath(studentId)}/${encodeURIComponent(measurementId)}/${recordsSegment}`,
    request,
  );
}

export function setMeasurementGoal(
  studentId: string,
  measurementId: string,
  value: number | null,
): Promise<void> {
  return httpClient.put<void>(
    `${studentMeasurementsPath(studentId)}/${encodeURIComponent(measurementId)}/${goalSegment}`,
    { value },
  );
}

export function listClassMeasurementRecords(
  measuredOn: string,
  studentIds: readonly string[],
): Promise<MeasurementRecord[]> {
  const studentIdsQuery = studentIds
    .map((studentId) => `&${studentIdsParameter}=${encodeURIComponent(studentId)}`)
    .join("");
  return httpClient.get<MeasurementRecord[]>(
    `${measurementRecordsPath}?${measuredOnParameter}=${encodeURIComponent(measuredOn)}${studentIdsQuery}`,
  );
}

export function recordClassMeasurements(
  measurementId: string,
  measuredOn: string,
  entries: ClassMeasurementEntry[],
): Promise<MeasurementRecord[]> {
  return httpClient.post<MeasurementRecord[]>(measurementRecordsPath, {
    measurementId,
    measuredOn,
    entries,
  });
}

export function deleteMeasurementRecord(recordId: string): Promise<void> {
  return httpClient.delete<void>(`${measurementRecordsPath}/${encodeURIComponent(recordId)}`);
}

export function restoreMeasurementRecord(recordId: string): Promise<void> {
  return httpClient.post<void>(
    `${measurementRecordsPath}/${encodeURIComponent(recordId)}/${restoreSegment}`,
    {},
  );
}

export function getStudentAppMeasurements(): Promise<StudentAppMeasurements> {
  return httpClient.get<StudentAppMeasurements>(`${studentAppPath}/${measurementsSegment}`);
}

export function recordStudentAppMeasurement(
  studentId: string,
  measurementId: string,
  request: RecordMeasurementRequest,
): Promise<MeasurementRecord> {
  return httpClient.post<MeasurementRecord>(
    `${studentAppPath}/students/${encodeURIComponent(studentId)}/${measurementsSegment}/${encodeURIComponent(measurementId)}/${recordsSegment}`,
    request,
  );
}
