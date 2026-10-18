export const measurementQueryKeys = {
  all: ["measurements"] as const,
  definitions: () => [...measurementQueryKeys.all, "definitions"] as const,
  student: (studentId: string) => [...measurementQueryKeys.all, "student", studentId] as const,
  classRecords: (measuredOn: string, studentIds: readonly string[]) =>
    [...measurementQueryKeys.all, "class", measuredOn, ...studentIds] as const,
  studentApp: () => [...measurementQueryKeys.all, "studentApp"] as const,
};
