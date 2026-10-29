import { useQuery } from "@tanstack/react-query";
import { getStudentAppFamily } from "@/features/studentApp/studentAppApi";
import { studentAppQueryKeys } from "@/features/studentApp/studentAppQueryKeys";

export function useStudentAppFamily() {
  return useQuery({ queryKey: studentAppQueryKeys.family(), queryFn: getStudentAppFamily });
}
