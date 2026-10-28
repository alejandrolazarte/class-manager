import { useQueries } from "@tanstack/react-query";
import { getStudentAppClassHistory } from "@/features/studentApp/studentAppApi";
import { studentAppQueryKeys } from "@/features/studentApp/studentAppQueryKeys";
import { StudentAppPastClass } from "@/features/studentApp/types";

export interface StudentAppClassHistoryResult {
  pastClasses: StudentAppPastClass[];
  isPending: boolean;
  isError: boolean;
  refetch: () => void;
}

export function useStudentAppClassHistory(
  studentId: string | null,
  months: readonly string[],
): StudentAppClassHistoryResult {
  const results = useQueries({
    queries: months.map((month) => ({
      queryKey: studentAppQueryKeys.classHistory(studentId ?? "", month),
      queryFn: () => getStudentAppClassHistory(studentId ?? "", month),
      enabled: studentId !== null,
    })),
  });
  return {
    pastClasses: results.flatMap((result) => result.data?.classes ?? []),
    isPending: results.some((result) => result.isPending),
    isError: results.some((result) => result.isError),
    refetch: () => results.forEach((result) => void result.refetch()),
  };
}
