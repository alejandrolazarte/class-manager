import { useQuery } from "@tanstack/react-query";
import { getStudentAppPackClasses } from "@/features/studentApp/studentAppApi";
import { studentAppQueryKeys } from "@/features/studentApp/studentAppQueryKeys";

export function useStudentAppPackClasses(studentId: string | null) {
  return useQuery({
    queryKey: studentAppQueryKeys.packClasses(studentId ?? ""),
    queryFn: () => getStudentAppPackClasses(studentId ?? ""),
    enabled: studentId !== null,
  });
}
