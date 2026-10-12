export const measurementQueryKeys = {
  all: ["measurements"] as const,
  definitions: () => [...measurementQueryKeys.all, "definitions"] as const,
  student: (studentId: string) => [...measurementQueryKeys.all, "student", studentId] as const,
  studentApp: () => [...measurementQueryKeys.all, "studentApp"] as const,
};
