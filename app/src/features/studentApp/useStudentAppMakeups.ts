import { useQuery } from "@tanstack/react-query";
import { getStudentAppMakeups } from "@/features/studentApp/studentAppApi";
import { studentAppQueryKeys } from "@/features/studentApp/studentAppQueryKeys";

export function useStudentAppMakeups(studentId: string | null) {
  return useQuery({
    queryKey: studentAppQueryKeys.makeups(studentId ?? ""),
    queryFn: () => getStudentAppMakeups(studentId ?? ""),
    enabled: studentId !== null,
  });
}
