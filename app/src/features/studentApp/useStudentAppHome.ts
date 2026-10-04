import { useQuery } from "@tanstack/react-query";
import { getStudentAppHome } from "@/features/studentApp/studentAppApi";
import { studentAppQueryKeys } from "@/features/studentApp/studentAppQueryKeys";

export function useStudentAppHome() {
  return useQuery({ queryKey: studentAppQueryKeys.home(), queryFn: getStudentAppHome });
}
