import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import {
  createMeasurement,
  deleteMeasurement,
  deleteMeasurementRecord,
  getStudentAppMeasurements,
  getStudentMeasurements,
  listClassMeasurementRecords,
  listMeasurements,
  recordClassMeasurements,
  recordMeasurement,
  recordStudentAppMeasurement,
  restoreMeasurement,
  restoreMeasurementRecord,
  setMeasurementGoal,
  updateMeasurement,
} from "@/features/measurements/measurementsApi";
import { measurementQueryKeys } from "@/features/measurements/measurementQueryKeys";
import {
  ClassMeasurementEntry,
  CreateMeasurementRequest,
  Measurement,
  RecordMeasurementRequest,
} from "@/features/measurements/types";

function useInvalidateMeasurements() {
  const queryClient = useQueryClient();
  return () => queryClient.invalidateQueries({ queryKey: measurementQueryKeys.all });
}

export function useMeasurementDefinitions(options: { enabled?: boolean } = {}) {
  return useQuery({
    queryKey: measurementQueryKeys.definitions(),
    queryFn: listMeasurements,
    enabled: options.enabled ?? true,
  });
}

export function useStudentMeasurements(studentId: string | undefined) {
  return useQuery({
    queryKey: measurementQueryKeys.student(studentId ?? ""),
    queryFn: () => getStudentMeasurements(studentId ?? ""),
    enabled: studentId !== undefined && studentId.length > 0,
  });
}

export function useClassMeasurementRecords(measuredOn: string, studentIds: readonly string[]) {
  return useQuery({
    queryKey: measurementQueryKeys.classRecords(measuredOn, studentIds),
    queryFn: () => listClassMeasurementRecords(measuredOn, studentIds),
    enabled: studentIds.length > 0,
  });
}

export function useStudentAppMeasurements() {
  return useQuery({
    queryKey: measurementQueryKeys.studentApp(),
    queryFn: getStudentAppMeasurements,
  });
}

export function useCreateMeasurement() {
  const invalidate = useInvalidateMeasurements();
  return useMutation({
    mutationFn: (request: CreateMeasurementRequest) => createMeasurement(request),
    onSuccess: invalidate,
  });
}

export function useUpdateMeasurement() {
  const invalidate = useInvalidateMeasurements();
  return useMutation({
    mutationFn: (measurement: Measurement) =>
      updateMeasurement(measurement.id, {
        name: measurement.name,
        improvesWhen: measurement.improvesWhen,
        isVisibleToStudents: measurement.isVisibleToStudents,
        isRecordedByStudents: measurement.isRecordedByStudents,
      }),
    onSuccess: invalidate,
  });
}

export function useDeleteMeasurement() {
  const invalidate = useInvalidateMeasurements();
  return useMutation({
    mutationFn: (measurementId: string) => deleteMeasurement(measurementId),
    onSuccess: invalidate,
  });
}

export function useRestoreMeasurement() {
  const invalidate = useInvalidateMeasurements();
  return useMutation({
    mutationFn: (measurementId: string) => restoreMeasurement(measurementId),
    onSuccess: invalidate,
  });
}

interface RecordMeasurementVariables {
  studentId: string;
  measurementId: string;
  request: RecordMeasurementRequest;
}

export function useRecordMeasurement() {
  const invalidate = useInvalidateMeasurements();
  return useMutation({
    mutationFn: ({ studentId, measurementId, request }: RecordMeasurementVariables) =>
      recordMeasurement(studentId, measurementId, request),
    onSuccess: invalidate,
  });
}

export function useRecordStudentAppMeasurement() {
  const invalidate = useInvalidateMeasurements();
  return useMutation({
    mutationFn: ({ studentId, measurementId, request }: RecordMeasurementVariables) =>
      recordStudentAppMeasurement(studentId, measurementId, request),
    onSuccess: invalidate,
  });
}

interface RecordClassMeasurementsVariables {
  measurementId: string;
  measuredOn: string;
  entries: ClassMeasurementEntry[];
}

export function useRecordClassMeasurements() {
  const invalidate = useInvalidateMeasurements();
  return useMutation({
    mutationFn: ({ measurementId, measuredOn, entries }: RecordClassMeasurementsVariables) =>
      recordClassMeasurements(measurementId, measuredOn, entries),
    onSuccess: invalidate,
  });
}

export function useDeleteMeasurementRecord() {
  const invalidate = useInvalidateMeasurements();
  return useMutation({
    mutationFn: (recordId: string) => deleteMeasurementRecord(recordId),
    onSuccess: invalidate,
  });
}

export function useRestoreMeasurementRecord() {
  const invalidate = useInvalidateMeasurements();
  return useMutation({
    mutationFn: (recordId: string) => restoreMeasurementRecord(recordId),
    onSuccess: invalidate,
  });
}

interface SetGoalVariables {
  studentId: string;
  measurementId: string;
  value: number | null;
}

export function useSetMeasurementGoal() {
  const invalidate = useInvalidateMeasurements();
  return useMutation({
    mutationFn: ({ studentId, measurementId, value }: SetGoalVariables) =>
      setMeasurementGoal(studentId, measurementId, value),
    onSuccess: invalidate,
  });
}
