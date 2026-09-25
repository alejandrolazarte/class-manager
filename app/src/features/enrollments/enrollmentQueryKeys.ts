export const enrollmentQueryKeys = {
  all: ["enrollments"] as const,
  roster: (classGroupId: string) => [...enrollmentQueryKeys.all, "roster", classGroupId] as const,
  student: (studentId: string) => [...enrollmentQueryKeys.all, "student", studentId] as const,
};
